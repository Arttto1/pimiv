using KanbanAPI.Services;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("========================================");
Console.WriteLine("[STARTUP] Iniciando KanbanAPI...");
Console.WriteLine("========================================");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddHttpClient<AIService>();

Console.WriteLine("[STARTUP] Serviços registrados");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

Console.WriteLine("[STARTUP] CORS configurado");

var app = builder.Build();

Console.WriteLine("[STARTUP] Aplicação construída");

// Testar conexão com banco de dados
try
{
    Console.WriteLine("\n========================================");
    Console.WriteLine("[DATABASE] Testando conexão com SQL Server LocalDB...");
    var dbService = app.Services.GetRequiredService<DatabaseService>();
    await using var testConnection = await dbService.GetConnectionAsync();
    Console.WriteLine("[DATABASE] ✅ Conexão estabelecida com sucesso!");
    Console.WriteLine($"[DATABASE] Server: {testConnection.DataSource}");
    Console.WriteLine($"[DATABASE] Database: {testConnection.Database}");
    Console.WriteLine($"[DATABASE] Estado: {testConnection.State}");
    
    // Testar query simples
    await using var cmd = new Microsoft.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM pim_users", testConnection);
    var userCount = await cmd.ExecuteScalarAsync();
    Console.WriteLine($"[DATABASE] Total de usuários na tabela: {userCount}");
    Console.WriteLine("========================================\n");
}
catch (Exception ex)
{
    Console.WriteLine("\n❌❌❌ [DATABASE] ERRO NA CONEXÃO ❌❌❌");
    Console.WriteLine($"[DATABASE] Tipo: {ex.GetType().Name}");
    Console.WriteLine($"[DATABASE] Mensagem: {ex.Message}");
    Console.WriteLine($"[DATABASE] StackTrace:\n{ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"[DATABASE] Inner Exception: {ex.InnerException.Message}");
    }
    Console.WriteLine("========================================\n");
    Console.WriteLine("⚠️  A API continuará rodando, mas operações de banco falharão!");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    Console.WriteLine("[STARTUP] Swagger habilitado");
}

// Log de todas as requisições
app.Use(async (context, next) =>
{
    var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
    Console.WriteLine($"\n>>> [REQUEST {timestamp}] {context.Request.Method} {context.Request.Path}");
    Console.WriteLine($">>> [REQUEST] Full URL: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
    Console.WriteLine($">>> [REQUEST] ContentType: {context.Request.ContentType}");
    Console.WriteLine($">>> [REQUEST] ContentLength: {context.Request.ContentLength}");
    
    await next();
    
    Console.WriteLine($"<<< [RESPONSE {timestamp}] Status: {context.Response.StatusCode}");
    
    // Se for 404, mostrar rotas disponíveis
    if (context.Response.StatusCode == 404)
    {
        Console.WriteLine($"⚠️  ROTA NÃO ENCONTRADA!");
        Console.WriteLine($"⚠️  Certifique-se que a rota começa com '/api/'");
    }
    Console.WriteLine();
});

app.UseCors("AllowAll");
Console.WriteLine("[STARTUP] CORS habilitado");

app.UseAuthorization();
Console.WriteLine("[STARTUP] Autorização configurada");

app.MapControllers();
Console.WriteLine("[STARTUP] Controllers mapeados");

// Listar todas as rotas registradas
var endpointDataSource = app.Services.GetRequiredService<Microsoft.AspNetCore.Routing.EndpointDataSource>();
Console.WriteLine("\n========================================");
Console.WriteLine("[STARTUP] Rotas registradas:");
foreach (var endpoint in endpointDataSource.Endpoints)
{
    if (endpoint is Microsoft.AspNetCore.Routing.RouteEndpoint routeEndpoint)
    {
        Console.WriteLine($"  - {routeEndpoint.RoutePattern.RawText}");
    }
}
Console.WriteLine("========================================");

Console.WriteLine("========================================");
Console.WriteLine("[STARTUP] ✅ API rodando em http://localhost:5000");
Console.WriteLine("[STARTUP] ✅ Swagger: http://localhost:5000/swagger");
Console.WriteLine("========================================\n");

app.Run();
