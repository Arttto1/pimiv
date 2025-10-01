using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using KanbanAPI.Services;
using KanbanAPI.DTOs;
using KanbanAPI.Models;

namespace KanbanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly DatabaseService _dbService;
    private readonly AIService _aiService;

    public CardsController(DatabaseService dbService, AIService aiService)
    {
        _dbService = dbService;
        _aiService = aiService;
    }

    [HttpGet("column/{columnId}")]
    public async Task<ActionResult<List<Card>>> GetColumnCards(Guid columnId)
    {
        try
        {
            Console.WriteLine($"[CARDS] Buscando cards da coluna: {columnId}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var query = @"SELECT id, column_id, title, description, position, created_at, updated_at 
                         FROM pim_cards 
                         WHERE column_id = @columnId 
                         ORDER BY position";
            
            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@columnId", columnId);

            var cards = new List<Card>();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                cards.Add(new Card
                {
                    Id = reader.GetGuid(0),
                    ColumnId = reader.GetGuid(1),
                    Title = reader.GetString(2),
                    Description = reader.GetString(3),
                    Position = reader.GetInt32(4),
                    CreatedAt = reader.GetDateTime(5),
                    UpdatedAt = reader.GetDateTime(6)
                });
            }

            Console.WriteLine($"[CARDS] {cards.Count} cards encontrados");
            return Ok(cards);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO CARDS] {ex.Message}");
            Console.WriteLine($"[ERRO CARDS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao buscar cards", error = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Card>>> GetUserCards(Guid userId)
    {
        try
        {
            Console.WriteLine($"[CARDS] Buscando todos os cards do usuário: {userId}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var query = @"SELECT c.id, c.column_id, c.title, c.description, c.position, c.created_at, c.updated_at 
                         FROM pim_cards c
                         INNER JOIN pim_columns col ON c.column_id = col.id
                         WHERE col.user_id = @userId
                         ORDER BY col.position, c.position";
            
            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@userId", userId);

            var cards = new List<Card>();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                cards.Add(new Card
                {
                    Id = reader.GetGuid(0),
                    ColumnId = reader.GetGuid(1),
                    Title = reader.GetString(2),
                    Description = reader.GetString(3),
                    Position = reader.GetInt32(4),
                    CreatedAt = reader.GetDateTime(5),
                    UpdatedAt = reader.GetDateTime(6)
                });
            }

            Console.WriteLine($"[CARDS] {cards.Count} cards encontrados para o usuário");
            return Ok(cards);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO CARDS] {ex.Message}");
            Console.WriteLine($"[ERRO CARDS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao buscar cards", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<Card>> CreateCard([FromBody] CreateCardRequest request)
    {
        try
        {
            Console.WriteLine($"[CARDS] Criando card: {request.Title}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var countQuery = "SELECT COALESCE(MAX(position), -1) + 1 FROM pim_cards WHERE column_id = @columnId";
            await using var countCmd = new SqlCommand(countQuery, connection);
            countCmd.Parameters.AddWithValue("@columnId", request.ColumnId);
            var position = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

            var cardId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var insertQuery = @"INSERT INTO pim_cards (id, column_id, title, description, position, created_at, updated_at) 
                              VALUES (@id, @columnId, @title, @description, @position, @createdAt, @updatedAt)";
            
            await using var insertCmd = new SqlCommand(insertQuery, connection);
            insertCmd.Parameters.AddWithValue("@id", cardId);
            insertCmd.Parameters.AddWithValue("@columnId", request.ColumnId);
            insertCmd.Parameters.AddWithValue("@title", request.Title);
            insertCmd.Parameters.AddWithValue("@description", request.Description);
            insertCmd.Parameters.AddWithValue("@position", position);
            insertCmd.Parameters.AddWithValue("@createdAt", now);
            insertCmd.Parameters.AddWithValue("@updatedAt", now);

            await insertCmd.ExecuteNonQueryAsync();

            Console.WriteLine($"[CARDS] Card criado: {cardId}");

            var card = new Card
            {
                Id = cardId,
                ColumnId = request.ColumnId,
                Title = request.Title,
                Description = request.Description,
                Position = position,
                CreatedAt = now,
                UpdatedAt = now
            };

            return Ok(card);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO CARDS] {ex.Message}");
            Console.WriteLine($"[ERRO CARDS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao criar card", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Card>> UpdateCard(Guid id, [FromBody] UpdateCardRequest request)
    {
        try
        {
            Console.WriteLine($"[CARDS] Atualizando card: {id}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var updates = new List<string> { "updated_at = @updatedAt" };
            var cmd = new SqlCommand { Connection = connection };
            cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);

            if (request.Title != null)
            {
                updates.Add("title = @title");
                cmd.Parameters.AddWithValue("@title", request.Title);
            }

            if (request.Description != null)
            {
                updates.Add("description = @description");
                cmd.Parameters.AddWithValue("@description", request.Description);
            }

            if (request.Position.HasValue)
            {
                updates.Add("position = @position");
                cmd.Parameters.AddWithValue("@position", request.Position.Value);
            }

            if (request.ColumnId.HasValue)
            {
                updates.Add("column_id = @columnId");
                cmd.Parameters.AddWithValue("@columnId", request.ColumnId.Value);
                
                Console.WriteLine($"[CARDS] Movendo card para coluna: {request.ColumnId.Value}");
            }

            cmd.CommandText = $"UPDATE pim_cards SET {string.Join(", ", updates)} WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            Console.WriteLine($"[CARDS] Query: {cmd.CommandText}");

            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                Console.WriteLine($"[CARDS] Card não encontrado: {id}");
                return NotFound(new { message = "Card não encontrado" });
            }

            var selectQuery = @"SELECT id, column_id, title, description, position, created_at, updated_at 
                               FROM pim_cards WHERE id = @id";
            
            await using var selectCmd = new SqlCommand(selectQuery, connection);
            selectCmd.Parameters.AddWithValue("@id", id);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var card = new Card
                {
                    Id = reader.GetGuid(0),
                    ColumnId = reader.GetGuid(1),
                    Title = reader.GetString(2),
                    Description = reader.GetString(3),
                    Position = reader.GetInt32(4),
                    CreatedAt = reader.GetDateTime(5),
                    UpdatedAt = reader.GetDateTime(6)
                };

                Console.WriteLine($"[CARDS] Card atualizado com sucesso");
                return Ok(card);
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO CARDS] {ex.Message}");
            Console.WriteLine($"[ERRO CARDS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao atualizar card", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCard(Guid id)
    {
        try
        {
            Console.WriteLine($"[CARDS] Deletando card: {id}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var deleteQuery = "DELETE FROM pim_cards WHERE id = @id";
            await using var deleteCmd = new SqlCommand(deleteQuery, connection);
            deleteCmd.Parameters.AddWithValue("@id", id);

            var rowsAffected = await deleteCmd.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                Console.WriteLine($"[CARDS] Card não encontrado: {id}");
                return NotFound(new { message = "Card não encontrado" });
            }

            Console.WriteLine($"[CARDS] Card deletado com sucesso");
            return Ok(new { message = "Card deletado com sucesso" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO CARDS] {ex.Message}");
            Console.WriteLine($"[ERRO CARDS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao deletar card", error = ex.Message });
        }
    }

    [HttpPost("{id}/rewrite")]
    public async Task<ActionResult<RewriteResponse>> RewriteDescription(Guid id)
    {
        try
        {
            Console.WriteLine($"[CARDS] Reescrevendo descrição do card: {id}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var selectQuery = "SELECT description FROM pim_cards WHERE id = @id";
            await using var selectCmd = new SqlCommand(selectQuery, connection);
            selectCmd.Parameters.AddWithValue("@id", id);

            var description = await selectCmd.ExecuteScalarAsync() as string;

            if (string.IsNullOrEmpty(description))
            {
                Console.WriteLine($"[CARDS] Card não encontrado ou sem descrição: {id}");
                return NotFound(new { message = "Card não encontrado ou sem descrição" });
            }

            var rewrittenText = await _aiService.RewriteTextAsync(description);

            var updateQuery = "UPDATE pim_cards SET description = @description, updated_at = @updatedAt WHERE id = @id";
            await using var updateCmd = new SqlCommand(updateQuery, connection);
            updateCmd.Parameters.AddWithValue("@description", rewrittenText);
            updateCmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
            updateCmd.Parameters.AddWithValue("@id", id);

            await updateCmd.ExecuteNonQueryAsync();

            Console.WriteLine($"[CARDS] Descrição reescrita e salva com sucesso");

            return Ok(new RewriteResponse { Text = rewrittenText });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO CARDS] {ex.Message}");
            Console.WriteLine($"[ERRO CARDS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao reescrever descrição", error = ex.Message });
        }
    }

    [HttpPost("rewrite-text")]
    public async Task<ActionResult<RewriteResponse>> RewriteText([FromBody] RewriteTextRequest request)
    {
        try
        {
            Console.WriteLine($"[CARDS] Reescrevendo texto direto (sem card)");

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new { message = "Texto não pode ser vazio" });
            }

            var rewrittenText = await _aiService.RewriteTextAsync(request.Text);

            Console.WriteLine($"[CARDS] Texto reescrito com sucesso");

            return Ok(new RewriteResponse { Text = rewrittenText });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO CARDS] {ex.Message}");
            Console.WriteLine($"[ERRO CARDS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao reescrever texto", error = ex.Message });
        }
    }
}
