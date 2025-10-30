using System.Text;
using System.Text.Json;
using KanbanAPI.DTOs;

namespace KanbanAPI.Services;

public class AIService
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;

    public AIService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _endpoint = configuration["AI:N8nEndpoint"] ?? string.Empty;
        Console.WriteLine($"AIService inicializado com endpoint: {_endpoint}");
    }

    public async Task<string> RewriteTextAsync(string text)
    {
        try
        {
            Console.WriteLine($"[AI] Enviando texto para reescrita: {text.Substring(0, Math.Min(50, text.Length))}...");
            
            var request = new { text = text };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"[AI] Payload enviado: {json}");

            var response = await _httpClient.PostAsync(_endpoint, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[AI] Status da resposta: {response.StatusCode}");
            Console.WriteLine($"[AI] Resposta recebida: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ERRO AI] Falha na requisição: {response.StatusCode}");
                throw new Exception($"Erro ao reescrever texto: {response.StatusCode}");
            }

            var result = JsonSerializer.Deserialize<RewriteResponse>(responseBody, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null || string.IsNullOrEmpty(result.Text))
            {
                Console.WriteLine("[ERRO AI] Resposta inválida ou vazia");
                throw new Exception("Resposta da IA inválida");
            }

            Console.WriteLine($"[AI] Texto reescrito com sucesso");
            return result.Text;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO AI] Exceção ao reescrever: {ex.Message}");
            throw;
        }
    }
}
