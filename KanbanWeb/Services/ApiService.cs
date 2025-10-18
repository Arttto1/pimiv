using System.Text;
using Newtonsoft.Json;
using KanbanWeb.Models;

namespace KanbanWeb.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5000";
        _httpClient.BaseAddress = new Uri(_baseUrl);
        Console.WriteLine($"[API] Inicializado com URL: {_baseUrl}");
    }

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        try
        {
            Console.WriteLine($"[API] Login: {username}");
            var request = new LoginRequest { Username = username, Password = password };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"[API] Payload: {json}");

            var response = await _httpClient.PostAsync("api/auth/login", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");
            Console.WriteLine($"[API] Resposta: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Login falhou");
                return null;
            }

            return JsonConvert.DeserializeObject<LoginResponse>(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção no login: {ex.Message}");
            return null;
        }
    }

    public async Task<(LoginResponse? response, string? error)> RegisterAsync(string username, string password)
    {
        try
        {
            Console.WriteLine($"\n====== [APISERVICE/REGISTER] INÍCIO ======");
            Console.WriteLine($"[APISERVICE] Username: {username}");
            Console.WriteLine($"[APISERVICE] Password length: {password?.Length ?? 0}");
            
            var request = new LoginRequest { Username = username ?? "", Password = password ?? "" };
            var json = JsonConvert.SerializeObject(request);
            Console.WriteLine($"[APISERVICE] JSON Payload: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Console.WriteLine($"[APISERVICE] ContentType: application/json");
            Console.WriteLine($"[APISERVICE] URL: {_httpClient.BaseAddress}api/auth/register");

            Console.WriteLine($"[APISERVICE] Enviando requisição HTTP POST...");
            var response = await _httpClient.PostAsync("api/auth/register", content);
            
            Console.WriteLine($"[APISERVICE] Resposta recebida!");
            Console.WriteLine($"[APISERVICE] Status Code: {(int)response.StatusCode} ({response.StatusCode})");
            Console.WriteLine($"[APISERVICE] Headers: {string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}"))}");
            
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[APISERVICE] Response Body: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[APISERVICE] ❌ ERRO HTTP");
                var errorMsg = $"Erro HTTP {(int)response.StatusCode} ({response.StatusCode})\n\n";
                errorMsg += $"Resposta da API:\n{responseBody}";
                Console.WriteLine($"====== [APISERVICE/REGISTER] FIM (ERRO) ======\n");
                return (null, errorMsg);
            }

            Console.WriteLine($"[APISERVICE] Deserializando resposta...");
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseBody);
            Console.WriteLine($"[APISERVICE] ✅ SUCESSO: UserId={loginResponse?.UserId}");
            Console.WriteLine($"====== [APISERVICE/REGISTER] FIM (SUCESSO) ======\n");
            
            return (loginResponse, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌❌❌ [APISERVICE/REGISTER] EXCEÇÃO ❌❌❌");
            Console.WriteLine($"[APISERVICE] Tipo: {ex.GetType().Name}");
            Console.WriteLine($"[APISERVICE] Mensagem: {ex.Message}");
            Console.WriteLine($"[APISERVICE] StackTrace:\n{ex.StackTrace}");
            
            var errorMsg = $"Exceção ao conectar com a API:\n\n";
            errorMsg += $"Tipo: {ex.GetType().Name}\n";
            errorMsg += $"Mensagem: {ex.Message}\n";
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[APISERVICE] Inner Exception: {ex.InnerException.Message}");
                errorMsg += $"\nErro Interno: {ex.InnerException.Message}";
            }
            Console.WriteLine($"====== [APISERVICE/REGISTER] FIM (EXCEÇÃO) ======\n");
            return (null, errorMsg);
        }
    }

    public async Task<List<Column>> GetColumnsAsync(Guid userId)
    {
        try
        {
            Console.WriteLine($"[API] Buscando colunas para usuário: {userId}");
            var response = await _httpClient.GetAsync($"api/columns/user/{userId}");
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao buscar colunas");
                return new List<Column>();
            }

            var columns = JsonConvert.DeserializeObject<List<Column>>(responseBody) ?? new List<Column>();
            Console.WriteLine($"[API] {columns.Count} colunas recebidas");
            return columns;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção: {ex.Message}");
            return new List<Column>();
        }
    }

    public async Task<Column?> CreateColumnAsync(Guid userId, string name, string color)
    {
        try
        {
            Console.WriteLine($"[API] Criando coluna: {name} - {color}");
            var request = new CreateColumnRequest { UserId = userId, Name = name, Color = color };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"[API] Payload: {json}");

            var response = await _httpClient.PostAsync("api/columns", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao criar coluna");
                return null;
            }

            return JsonConvert.DeserializeObject<Column>(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteColumnAsync(Guid columnId)
    {
        try
        {
            Console.WriteLine($"[API] Deletando coluna: {columnId}");
            var response = await _httpClient.DeleteAsync($"api/columns/{columnId}");

            Console.WriteLine($"[API] Status: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Card>> GetCardsAsync(Guid columnId)
    {
        try
        {
            Console.WriteLine($"[API] Buscando cards da coluna: {columnId}");
            var response = await _httpClient.GetAsync($"api/cards/column/{columnId}");
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao buscar cards");
                return new List<Card>();
            }

            var cards = JsonConvert.DeserializeObject<List<Card>>(responseBody) ?? new List<Card>();
            Console.WriteLine($"[API] {cards.Count} cards recebidos");
            return cards;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção: {ex.Message}");
            return new List<Card>();
        }
    }

    public async Task<Card?> CreateCardAsync(Guid columnId, string title, string description)
    {
        try
        {
            Console.WriteLine($"[API] Criando card: {title}");
            var request = new CreateCardRequest { ColumnId = columnId, Title = title, Description = description };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"[API] Payload: {json}");

            var response = await _httpClient.PostAsync("api/cards", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao criar card");
                return null;
            }

            return JsonConvert.DeserializeObject<Card>(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção: {ex.Message}");
            return null;
        }
    }

    public async Task<Card?> UpdateCardAsync(Guid cardId, UpdateCardRequest request)
    {
        try
        {
            Console.WriteLine($"[API] Atualizando card: {cardId}");
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"[API] Payload: {json}");

            var response = await _httpClient.PutAsync($"api/cards/{cardId}", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao atualizar card");
                return null;
            }

            return JsonConvert.DeserializeObject<Card>(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteCardAsync(Guid cardId)
    {
        try
        {
            Console.WriteLine($"[API] Deletando card: {cardId}");
            var response = await _httpClient.DeleteAsync($"api/cards/{cardId}");

            Console.WriteLine($"[API] Status: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção: {ex.Message}");
            return false;
        }
    }

    public async Task<string?> RewriteDescriptionAsync(Guid cardId)
    {
        try
        {
            Console.WriteLine($"[API] Reescrevendo descrição do card: {cardId}");
            var response = await _httpClient.PostAsync($"api/cards/{cardId}/rewrite", null);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao reescrever");
                return null;
            }

            var result = JsonConvert.DeserializeObject<RewriteResponse>(responseBody);
            return result?.Text;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> RewriteTextAsync(string text)
    {
        try
        {
            Console.WriteLine($"[API] Reescrevendo texto direto (length: {text.Length})");
            
            var request = new { Text = text };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("api/cards/rewrite-text", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao reescrever texto");
                return null;
            }

            var result = JsonConvert.DeserializeObject<RewriteResponse>(responseBody);
            return result?.Text;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção ao reescrever texto: {ex.Message}");
            return null;
        }
    }
}
