using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using KanbanDesktop.Models;

namespace KanbanDesktop.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(App.ApiBaseUrl)
        };
        Console.WriteLine($"[API] Inicializado com URL: {App.ApiBaseUrl}");
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
                Console.WriteLine($"[API ERRO] Login falhou: {response.StatusCode}");
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

    public async Task<LoginResponse?> RegisterAsync(string username, string password)
    {
        try
        {
            Console.WriteLine($"[API] Register: {username}");
            var request = new LoginRequest { Username = username, Password = password };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"[API] Payload: {json}");

            var response = await _httpClient.PostAsync("api/auth/register", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");
            Console.WriteLine($"[API] Resposta: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Cadastro falhou: {response.StatusCode}");
                return null;
            }

            return JsonConvert.DeserializeObject<LoginResponse>(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção no cadastro: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Column>> GetColumnsAsync(Guid userId)
    {
        try
        {
            Console.WriteLine($"\n====== [API/COLUMNS] INÍCIO ======");
            Console.WriteLine($"[API] UserId: {userId}");
            Console.WriteLine($"[API] URL: {_httpClient.BaseAddress}api/columns/user/{userId}");
            
            var response = await _httpClient.GetAsync($"api/columns/user/{userId}");
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {(int)response.StatusCode} ({response.StatusCode})");
            Console.WriteLine($"[API] Response Body: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API] ❌ ERRO ao buscar colunas");
                Console.WriteLine($"====== [API/COLUMNS] FIM (ERRO) ======\n");
                return new List<Column>();
            }

            var columns = JsonConvert.DeserializeObject<List<Column>>(responseBody) ?? new List<Column>();
            Console.WriteLine($"[API] ✅ {columns.Count} colunas recebidas");
            Console.WriteLine($"====== [API/COLUMNS] FIM (SUCESSO) ======\n");
            return columns;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌❌❌ [API/COLUMNS] EXCEÇÃO ❌❌❌");
            Console.WriteLine($"[API] Tipo: {ex.GetType().Name}");
            Console.WriteLine($"[API] Mensagem: {ex.Message}");
            Console.WriteLine($"[API] StackTrace:\n{ex.StackTrace}");
            Console.WriteLine($"====== [API/COLUMNS] FIM (EXCEÇÃO) ======\n");
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

            var response = await _httpClient.PostAsync($"api/columns", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");
            Console.WriteLine($"[API] Resposta: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao criar coluna");
                return null;
            }

            return JsonConvert.DeserializeObject<Column>(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção ao criar coluna: {ex.Message}");
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

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao deletar coluna");
                return false;
            }

            Console.WriteLine($"[API] Coluna deletada com sucesso");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção ao deletar coluna: {ex.Message}");
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
            Console.WriteLine($"[API ERRO] Exceção ao buscar cards: {ex.Message}");
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

            var response = await _httpClient.PostAsync($"api/cards", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");
            Console.WriteLine($"[API] Resposta: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao criar card");
                return null;
            }

            return JsonConvert.DeserializeObject<Card>(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção ao criar card: {ex.Message}");
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
            Console.WriteLine($"[API] Resposta: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao atualizar card");
                return null;
            }

            return JsonConvert.DeserializeObject<Card>(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção ao atualizar card: {ex.Message}");
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

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao deletar card");
                return false;
            }

            Console.WriteLine($"[API] Card deletado com sucesso");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção ao deletar card: {ex.Message}");
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
            Console.WriteLine($"[API] Resposta: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao reescrever descrição");
                return null;
            }

            var result = JsonConvert.DeserializeObject<RewriteResponse>(responseBody);
            Console.WriteLine($"[API] Descrição reescrita com sucesso");
            return result?.Text;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção ao reescrever descrição: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> RewriteTextAsync(string text)
    {
        try
        {
            Console.WriteLine($"[API] Reescrevendo texto: {text.Substring(0, Math.Min(50, text.Length))}...");
            var request = new RewriteTextRequest { Text = text };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"api/cards/rewrite-text", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[API] Status: {response.StatusCode}");
            Console.WriteLine($"[API] Resposta: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[API ERRO] Falha ao reescrever texto");
                return null;
            }

            var result = JsonConvert.DeserializeObject<RewriteResponse>(responseBody);
            Console.WriteLine($"[API] Texto reescrito com sucesso");
            return result?.Text;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[API ERRO] Exceção ao reescrever texto: {ex.Message}");
            return null;
        }
    }
}
