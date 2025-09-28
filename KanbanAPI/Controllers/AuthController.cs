using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using KanbanAPI.Services;
using KanbanAPI.DTOs;

namespace KanbanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DatabaseService _dbService;

    public AuthController(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            Console.WriteLine($"[AUTH] Tentativa de login: {request.Username}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var query = "SELECT id, username, password, admin FROM pim_users WHERE username = @username";
            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", request.Username);

            Console.WriteLine($"[AUTH] Executando query: {query}");

            await using var reader = await cmd.ExecuteReaderAsync();
            
            if (!await reader.ReadAsync())
            {
                Console.WriteLine($"[AUTH] Usuário não encontrado: {request.Username}");
                return Unauthorized(new { message = "Usuário ou senha inválidos" });
            }

            var userId = reader.GetGuid(0);
            var username = reader.GetString(1);
            var passwordHash = reader.GetString(2);
            var isAdmin = reader.GetBoolean(3);

            Console.WriteLine($"[AUTH] Usuário encontrado: {userId}");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, passwordHash))
            {
                Console.WriteLine($"[AUTH] Senha incorreta para usuário: {request.Username}");
                return Unauthorized(new { message = "Usuário ou senha inválidos" });
            }

            await reader.CloseAsync();

            Console.WriteLine($"[AUTH] Login bem-sucedido: {username} (Admin: {isAdmin})");

            // Se é admin, garantir que tem a coluna "Chamados"
            if (isAdmin)
            {
                await EnsureChamadosColumnExists(connection, userId);
            }

            return Ok(new LoginResponse
            {
                UserId = userId,
                Username = username,
                IsAdmin = isAdmin,
                Message = "Login realizado com sucesso"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO AUTH] {ex.Message}");
            Console.WriteLine($"[ERRO AUTH] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao realizar login", error = ex.Message });
        }
    }

    private async Task EnsureChamadosColumnExists(SqlConnection connection, Guid userId)
    {
        try
        {
            // Verificar se a coluna "Chamados" já existe
            var checkQuery = "SELECT id FROM pim_columns WHERE user_id = @userId AND name = 'Chamados'";
            await using var checkCmd = new SqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@userId", userId);
            
            var existingColumnId = await checkCmd.ExecuteScalarAsync();
            
            if (existingColumnId != null)
            {
                Console.WriteLine($"[AUTH] Coluna Chamados já existe para o usuário");
                return;
            }

            // Criar a coluna "Chamados"
            Console.WriteLine($"[AUTH] Criando coluna Chamados para o admin");
            
            var countQuery = "SELECT COALESCE(MAX(position), -1) + 1 FROM pim_columns WHERE user_id = @userId";
            await using var countCmd = new SqlCommand(countQuery, connection);
            countCmd.Parameters.AddWithValue("@userId", userId);
            var position = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

            var createQuery = @"INSERT INTO pim_columns (id, user_id, name, color, position, is_deletable, created_at) 
                               VALUES (@id, @userId, @name, @color, @position, @isDeletable, @createdAt)";
            await using var createCmd = new SqlCommand(createQuery, connection);
            createCmd.Parameters.AddWithValue("@id", Guid.NewGuid());
            createCmd.Parameters.AddWithValue("@userId", userId);
            createCmd.Parameters.AddWithValue("@name", "Chamados");
            createCmd.Parameters.AddWithValue("@color", "red");
            createCmd.Parameters.AddWithValue("@position", position);
            createCmd.Parameters.AddWithValue("@isDeletable", false);
            createCmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

            await createCmd.ExecuteNonQueryAsync();
            Console.WriteLine($"[AUTH] Coluna Chamados criada com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH] Erro ao criar coluna Chamados: {ex.Message}");
            // Não propagar erro para não bloquear o login
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] LoginRequest request)
    {
        try
        {
            Console.WriteLine($"\n====== [AUTH/REGISTER] INÍCIO ======");
            Console.WriteLine($"[AUTH/REGISTER] Username recebido: {request.Username}");
            Console.WriteLine($"[AUTH/REGISTER] Password length: {request.Password?.Length ?? 0}");

            Console.WriteLine($"[AUTH/REGISTER] Obtendo conexão com banco...");
            await using var connection = await _dbService.GetConnectionAsync();
            Console.WriteLine($"[AUTH/REGISTER] Conexão estabelecida!");
            
            var checkQuery = "SELECT COUNT(*) FROM pim_users WHERE username = @username";
            Console.WriteLine($"[AUTH/REGISTER] Verificando se usuário existe: {checkQuery}");
            await using var checkCmd = new SqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@username", request.Username);

            var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;
            Console.WriteLine($"[AUTH/REGISTER] Usuário existe? {exists}");

            if (exists)
            {
                Console.WriteLine($"[AUTH/REGISTER] ❌ CONFLITO: Usuário já existe: {request.Username}");
                Console.WriteLine($"====== [AUTH/REGISTER] FIM (409) ======\n");
                return Conflict(new { message = "Usuário já existe" });
            }

            Console.WriteLine($"[AUTH/REGISTER] Gerando hash da senha...");
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            Console.WriteLine($"[AUTH/REGISTER] Hash gerado: {passwordHash.Substring(0, 20)}...");
            
            var userId = Guid.NewGuid();
            Console.WriteLine($"[AUTH/REGISTER] UUID gerado: {userId}");

            var insertQuery = @"INSERT INTO pim_users (id, username, password, admin, created_at) 
                              VALUES (@id, @username, @password, @admin, @created_at)";
            
            Console.WriteLine($"[AUTH/REGISTER] Preparando insert...");
            await using var insertCmd = new SqlCommand(insertQuery, connection);
            insertCmd.Parameters.AddWithValue("@id", userId);
            insertCmd.Parameters.AddWithValue("@username", request.Username);
            insertCmd.Parameters.AddWithValue("@password", passwordHash);
            insertCmd.Parameters.AddWithValue("@admin", false); // Por padrão, não é admin
            insertCmd.Parameters.AddWithValue("@created_at", DateTime.UtcNow);

            Console.WriteLine($"[AUTH/REGISTER] Executando INSERT no banco...");
            var rowsAffected = await insertCmd.ExecuteNonQueryAsync();
            Console.WriteLine($"[AUTH/REGISTER] Linhas afetadas: {rowsAffected}");

            Console.WriteLine($"[AUTH/REGISTER] ✅ SUCESSO: Usuário registrado: {userId}");

            var response = new LoginResponse
            {
                UserId = userId,
                Username = request.Username,
                IsAdmin = false,
                Message = "Usuário registrado com sucesso"
            };
            
            Console.WriteLine($"[AUTH/REGISTER] Retornando resposta: {System.Text.Json.JsonSerializer.Serialize(response)}");
            Console.WriteLine($"====== [AUTH/REGISTER] FIM (200) ======\n");
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌❌❌ [ERRO AUTH/REGISTER] EXCEÇÃO CAPTURADA ❌❌❌");
            Console.WriteLine($"[ERRO AUTH/REGISTER] Tipo: {ex.GetType().Name}");
            Console.WriteLine($"[ERRO AUTH/REGISTER] Mensagem: {ex.Message}");
            Console.WriteLine($"[ERRO AUTH/REGISTER] StackTrace:\n{ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[ERRO AUTH/REGISTER] Inner Exception: {ex.InnerException.Message}");
            }
            Console.WriteLine($"====== [AUTH/REGISTER] FIM (500) ======\n");
            return StatusCode(500, new { message = "Erro ao registrar usuário", error = ex.Message });
        }
    }
}
