using Microsoft.Data.SqlClient;

namespace KanbanAPI.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServer") 
            ?? throw new Exception("Connection string 'SqlServer' não configurada!");
        
        Console.WriteLine("[DBSERVICE] DatabaseService inicializado");
        Console.WriteLine($"[DBSERVICE] Connection string encontrada: {_connectionString.Substring(0, Math.Min(50, _connectionString.Length))}...");
    }

    public async Task<SqlConnection> GetConnectionAsync()
    {
        try
        {
            Console.WriteLine($"[DBSERVICE] Criando conexão...");
            var connection = new SqlConnection(_connectionString);
            
            Console.WriteLine($"[DBSERVICE] Abrindo conexão...");
            await connection.OpenAsync();
            
            Console.WriteLine($"[DBSERVICE] ✅ Conexão aberta! Estado: {connection.State}, Database: {connection.Database}");
            return connection;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌❌❌ [DBSERVICE] ERRO AO CONECTAR ❌❌❌");
            Console.WriteLine($"[DBSERVICE] Tipo: {ex.GetType().Name}");
            Console.WriteLine($"[DBSERVICE] Mensagem: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[DBSERVICE] Inner Exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }
}
