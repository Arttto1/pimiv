using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using KanbanAPI.Services;
using KanbanAPI.DTOs;
using KanbanAPI.Models;

namespace KanbanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColumnsController : ControllerBase
{
    private readonly DatabaseService _dbService;

    public ColumnsController(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Column>>> GetUserColumns(Guid userId)
    {
        try
        {
            Console.WriteLine($"[COLUMNS] Buscando colunas do usuário: {userId}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var query = @"SELECT id, user_id, name, color, position, is_deletable, created_at 
                         FROM pim_columns 
                         WHERE user_id = @userId 
                         ORDER BY position";
            
            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@userId", userId);

            var columns = new List<Column>();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                columns.Add(new Column
                {
                    Id = reader.GetGuid(0),
                    UserId = reader.GetGuid(1),
                    Name = reader.GetString(2),
                    Color = reader.GetString(3),
                    Position = reader.GetInt32(4),
                    IsDeletable = reader.GetBoolean(5),
                    CreatedAt = reader.GetDateTime(6)
                });
            }

            Console.WriteLine($"[COLUMNS] {columns.Count} colunas encontradas");
            return Ok(columns);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO COLUMNS] {ex.Message}");
            Console.WriteLine($"[ERRO COLUMNS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao buscar colunas", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<Column>> CreateColumn([FromBody] CreateColumnRequest request)
    {
        try
        {
            Console.WriteLine($"[COLUMNS] Criando coluna: {request.Name} - Cor: {request.Color}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var countQuery = "SELECT COALESCE(MAX(position), -1) + 1 FROM pim_columns WHERE user_id = @userId";
            await using var countCmd = new SqlCommand(countQuery, connection);
            countCmd.Parameters.AddWithValue("@userId", request.UserId);
            var position = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

            var columnId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;

            var insertQuery = @"INSERT INTO pim_columns (id, user_id, name, color, position, is_deletable, created_at) 
                              VALUES (@id, @userId, @name, @color, @position, @isDeletable, @createdAt)";
            
            await using var insertCmd = new SqlCommand(insertQuery, connection);
            insertCmd.Parameters.AddWithValue("@id", columnId);
            insertCmd.Parameters.AddWithValue("@userId", request.UserId);
            insertCmd.Parameters.AddWithValue("@name", request.Name);
            insertCmd.Parameters.AddWithValue("@color", request.Color);
            insertCmd.Parameters.AddWithValue("@position", position);
            insertCmd.Parameters.AddWithValue("@isDeletable", true);
            insertCmd.Parameters.AddWithValue("@createdAt", createdAt);

            await insertCmd.ExecuteNonQueryAsync();

            Console.WriteLine($"[COLUMNS] Coluna criada: {columnId}");

            var column = new Column
            {
                Id = columnId,
                UserId = request.UserId,
                Name = request.Name,
                Color = request.Color,
                Position = position,
                IsDeletable = true,
                CreatedAt = createdAt
            };

            return Ok(column);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO COLUMNS] {ex.Message}");
            Console.WriteLine($"[ERRO COLUMNS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao criar coluna", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Column>> UpdateColumn(Guid id, [FromBody] UpdateColumnRequest request)
    {
        try
        {
            Console.WriteLine($"[COLUMNS] Atualizando coluna: {id}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            var updates = new List<string>();
            var cmd = new SqlCommand { Connection = connection };

            if (request.Name != null)
            {
                updates.Add("name = @name");
                cmd.Parameters.AddWithValue("@name", request.Name);
            }

            if (request.Color != null)
            {
                updates.Add("color = @color");
                cmd.Parameters.AddWithValue("@color", request.Color);
            }

            if (request.Position.HasValue)
            {
                updates.Add("position = @position");
                cmd.Parameters.AddWithValue("@position", request.Position.Value);
            }

            if (updates.Count == 0)
            {
                return BadRequest(new { message = "Nenhum campo para atualizar" });
            }

            cmd.CommandText = $"UPDATE pim_columns SET {string.Join(", ", updates)} WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            Console.WriteLine($"[COLUMNS] Query: {cmd.CommandText}");

            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                Console.WriteLine($"[COLUMNS] Coluna não encontrada: {id}");
                return NotFound(new { message = "Coluna não encontrada" });
            }

            var selectQuery = @"SELECT id, user_id, name, color, position, is_deletable, created_at 
                               FROM pim_columns WHERE id = @id";
            
            await using var selectCmd = new SqlCommand(selectQuery, connection);
            selectCmd.Parameters.AddWithValue("@id", id);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var column = new Column
                {
                    Id = reader.GetGuid(0),
                    UserId = reader.GetGuid(1),
                    Name = reader.GetString(2),
                    Color = reader.GetString(3),
                    Position = reader.GetInt32(4),
                    IsDeletable = reader.GetBoolean(5),
                    CreatedAt = reader.GetDateTime(6)
                };

                Console.WriteLine($"[COLUMNS] Coluna atualizada com sucesso");
                return Ok(column);
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO COLUMNS] {ex.Message}");
            Console.WriteLine($"[ERRO COLUMNS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao atualizar coluna", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteColumn(Guid id)
    {
        try
        {
            Console.WriteLine($"[COLUMNS] Deletando coluna: {id}");

            await using var connection = await _dbService.GetConnectionAsync();
            
            // Verifica se a coluna pode ser deletada
            var checkQuery = "SELECT is_deletable, name FROM pim_columns WHERE id = @id";
            await using var checkCmd = new SqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@id", id);
            
            await using var checkReader = await checkCmd.ExecuteReaderAsync();
            
            if (!await checkReader.ReadAsync())
            {
                Console.WriteLine($"[COLUMNS] Coluna não encontrada: {id}");
                return NotFound(new { message = "Coluna não encontrada" });
            }

            var isDeletable = checkReader.GetBoolean(0);
            var columnName = checkReader.GetString(1);
            await checkReader.CloseAsync();

            if (!isDeletable)
            {
                Console.WriteLine($"[COLUMNS] Tentativa de deletar coluna protegida: {columnName}");
                return BadRequest(new { message = $"A coluna '{columnName}' não pode ser excluída" });
            }

            var deleteCardsQuery = "DELETE FROM pim_cards WHERE column_id = @columnId";
            await using var deleteCardsCmd = new SqlCommand(deleteCardsQuery, connection);
            deleteCardsCmd.Parameters.AddWithValue("@columnId", id);
            var cardsDeleted = await deleteCardsCmd.ExecuteNonQueryAsync();

            Console.WriteLine($"[COLUMNS] {cardsDeleted} cards deletados da coluna");

            var deleteQuery = "DELETE FROM pim_columns WHERE id = @id";
            await using var deleteCmd = new SqlCommand(deleteQuery, connection);
            deleteCmd.Parameters.AddWithValue("@id", id);

            var rowsAffected = await deleteCmd.ExecuteNonQueryAsync();

            Console.WriteLine($"[COLUMNS] Coluna deletada com sucesso");
            return Ok(new { message = "Coluna deletada com sucesso" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO COLUMNS] {ex.Message}");
            Console.WriteLine($"[ERRO COLUMNS] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erro ao deletar coluna", error = ex.Message });
        }
    }
}
