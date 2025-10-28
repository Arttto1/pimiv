using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using KanbanAPI.Services;
using KanbanAPI.DTOs;
using KanbanAPI.Models;

namespace KanbanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly DatabaseService _dbService;
    private readonly AIService _aiService;

    public TicketsController(DatabaseService dbService, AIService aiService)
    {
        _dbService = dbService;
        _aiService = aiService;
    }

    /// <summary>
    /// Lista todos os usuários admin disponíveis
    /// </summary>
    [HttpGet("admins")]
    public async Task<ActionResult<List<AdminUserResponse>>> GetAdminUsers()
    {
        try
        {
            Console.WriteLine("[TICKETS] Buscando usuários admin");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var query = "SELECT id, username FROM pim_users WHERE admin = 1 ORDER BY username";
            await using var cmd = new SqlCommand(query, connection);

            var admins = new List<AdminUserResponse>();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                admins.Add(new AdminUserResponse
                {
                    Id = reader.GetGuid(0),
                    Username = reader.GetString(1)
                });
            }

            Console.WriteLine($"[TICKETS] {admins.Count} usuários admin encontrados");
            return Ok(admins);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO TICKETS] {ex.Message}");
            Console.WriteLine($"[ERRO TICKETS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao buscar usuários admin", error = ex.Message });
        }
    }

    /// <summary>
    /// Cria um novo ticket (chamado) para um usuário admin
    /// </summary>
    [HttpPost("user/{userId}")]
    public async Task<ActionResult<Ticket>> CreateTicket(Guid userId, [FromBody] CreateTicketRequest request)
    {
        try
        {
            Console.WriteLine($"[TICKETS] Usuário {userId} criando ticket para admin {request.AdminId}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            // Verifica se o admin existe e se realmente é admin
            var adminCheckQuery = "SELECT admin FROM pim_users WHERE id = @adminId";
            await using var adminCheckCmd = new SqlCommand(adminCheckQuery, connection);
            adminCheckCmd.Parameters.AddWithValue("@adminId", request.AdminId);
            var isAdmin = await adminCheckCmd.ExecuteScalarAsync();

            if (isAdmin == null || !(bool)isAdmin)
            {
                Console.WriteLine($"[TICKETS] Usuário {request.AdminId} não é admin ou não existe");
                return BadRequest(new { message = "Usuário admin não encontrado" });
            }

            // Busca ou cria a coluna "Chamados" para o admin
            var columnQuery = "SELECT id FROM pim_columns WHERE user_id = @adminId AND name = 'Chamados'";
            await using var columnCmd = new SqlCommand(columnQuery, connection);
            columnCmd.Parameters.AddWithValue("@adminId", request.AdminId);
            var columnIdObj = await columnCmd.ExecuteScalarAsync();

            Guid chamadosColumnId;
            if (columnIdObj == null)
            {
                // Cria a coluna "Chamados"
                Console.WriteLine($"[TICKETS] Criando coluna Chamados para admin {request.AdminId}");
                chamadosColumnId = Guid.NewGuid();
                
                var countQuery = "SELECT COALESCE(MAX(position), -1) + 1 FROM pim_columns WHERE user_id = @adminId";
                await using var countCmd = new SqlCommand(countQuery, connection);
                countCmd.Parameters.AddWithValue("@adminId", request.AdminId);
                var position = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

                var createColumnQuery = @"INSERT INTO pim_columns (id, user_id, name, color, position, is_deletable, created_at) 
                                         VALUES (@id, @userId, @name, @color, @position, @isDeletable, @createdAt)";
                await using var createColumnCmd = new SqlCommand(createColumnQuery, connection);
                createColumnCmd.Parameters.AddWithValue("@id", chamadosColumnId);
                createColumnCmd.Parameters.AddWithValue("@userId", request.AdminId);
                createColumnCmd.Parameters.AddWithValue("@name", "Chamados");
                createColumnCmd.Parameters.AddWithValue("@color", "#FF6B6B"); // Cor vermelha para chamados
                createColumnCmd.Parameters.AddWithValue("@position", position);
                createColumnCmd.Parameters.AddWithValue("@isDeletable", false);
                createColumnCmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                await createColumnCmd.ExecuteNonQueryAsync();
            }
            else
            {
                chamadosColumnId = (Guid)columnIdObj;
                Console.WriteLine($"[TICKETS] Coluna Chamados já existe: {chamadosColumnId}");
            }

            // Cria o card na coluna "Chamados"
            var cardId = Guid.NewGuid();
            var cardCountQuery = "SELECT COALESCE(MAX(position), -1) + 1 FROM pim_cards WHERE column_id = @columnId";
            await using var cardCountCmd = new SqlCommand(cardCountQuery, connection);
            cardCountCmd.Parameters.AddWithValue("@columnId", chamadosColumnId);
            var cardPosition = Convert.ToInt32(await cardCountCmd.ExecuteScalarAsync());

            var now = DateTime.UtcNow;
            var createCardQuery = @"INSERT INTO pim_cards (id, column_id, title, description, position, created_at, updated_at) 
                                   VALUES (@id, @columnId, @title, @description, @position, @createdAt, @updatedAt)";
            await using var createCardCmd = new SqlCommand(createCardQuery, connection);
            createCardCmd.Parameters.AddWithValue("@id", cardId);
            createCardCmd.Parameters.AddWithValue("@columnId", chamadosColumnId);
            createCardCmd.Parameters.AddWithValue("@title", request.Title);
            createCardCmd.Parameters.AddWithValue("@description", request.Description);
            createCardCmd.Parameters.AddWithValue("@position", cardPosition);
            createCardCmd.Parameters.AddWithValue("@createdAt", now);
            createCardCmd.Parameters.AddWithValue("@updatedAt", now);

            await createCardCmd.ExecuteNonQueryAsync();

            // Cria o registro do ticket
            var ticketId = Guid.NewGuid();
            var createTicketQuery = @"INSERT INTO pim_tickets (id, requester_id, admin_id, card_id, title, description, created_at) 
                                     VALUES (@id, @requesterId, @adminId, @cardId, @title, @description, @createdAt)";
            await using var createTicketCmd = new SqlCommand(createTicketQuery, connection);
            createTicketCmd.Parameters.AddWithValue("@id", ticketId);
            createTicketCmd.Parameters.AddWithValue("@requesterId", userId);
            createTicketCmd.Parameters.AddWithValue("@adminId", request.AdminId);
            createTicketCmd.Parameters.AddWithValue("@cardId", cardId);
            createTicketCmd.Parameters.AddWithValue("@title", request.Title);
            createTicketCmd.Parameters.AddWithValue("@description", request.Description);
            createTicketCmd.Parameters.AddWithValue("@createdAt", now);

            await createTicketCmd.ExecuteNonQueryAsync();

            Console.WriteLine($"[TICKETS] Ticket criado: {ticketId}");

            var ticket = new Ticket
            {
                Id = ticketId,
                RequesterId = userId,
                AdminId = request.AdminId,
                CardId = cardId,
                Title = request.Title,
                Description = request.Description,
                CreatedAt = now
            };

            return Ok(ticket);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO TICKETS] {ex.Message}");
            Console.WriteLine($"[ERRO TICKETS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao criar ticket", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém todos os tickets criados por um usuário não-admin
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<TicketInfoResponse>>> GetUserTickets(Guid userId)
    {
        try
        {
            Console.WriteLine($"[TICKETS] Buscando tickets do usuário: {userId}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var query = @"SELECT 
                            t.id, 
                            t.card_id, 
                            t.title, 
                            t.description, 
                            u.username as admin_username,
                            col.name as column_name,
                            t.created_at
                          FROM pim_tickets t
                          INNER JOIN pim_users u ON t.admin_id = u.id
                          INNER JOIN pim_cards c ON t.card_id = c.id
                          INNER JOIN pim_columns col ON c.column_id = col.id
                          WHERE t.requester_id = @userId
                          ORDER BY t.created_at DESC";
            
            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@userId", userId);

            var tickets = new List<TicketInfoResponse>();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tickets.Add(new TicketInfoResponse
                {
                    Id = reader.GetGuid(0),
                    CardId = reader.GetGuid(1),
                    Title = reader.GetString(2),
                    Description = reader.GetString(3),
                    AdminUsername = reader.GetString(4),
                    CurrentColumnName = reader.GetString(5),
                    CreatedAt = reader.GetDateTime(6)
                });
            }

            Console.WriteLine($"[TICKETS] {tickets.Count} tickets encontrados");
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO TICKETS] {ex.Message}");
            Console.WriteLine($"[ERRO TICKETS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao buscar tickets", error = ex.Message });
        }
    }

    /// <summary>
    /// Reescreve o título e descrição de um ticket usando IA
    /// </summary>
    [HttpPost("{ticketId}/rewrite")]
    public async Task<ActionResult<RewriteResponse>> RewriteTicketText(Guid ticketId, [FromBody] RewriteTextRequest request)
    {
        try
        {
            Console.WriteLine($"[TICKETS] Reescrevendo texto do ticket: {ticketId}");

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new { message = "Texto não pode ser vazio" });
            }

            var rewrittenText = await _aiService.RewriteTextAsync(request.Text);

            Console.WriteLine($"[TICKETS] Texto reescrito com sucesso");

            return Ok(new RewriteResponse { Text = rewrittenText });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO TICKETS] {ex.Message}");
            Console.WriteLine($"[ERRO TICKETS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao reescrever texto", error = ex.Message });
        }
    }
}
