using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using KanbanDesktop.Models;

namespace KanbanDesktop.Services;

public class TicketService
{
    private readonly HttpClient _httpClient;

    public TicketService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(App.ApiBaseUrl)
        };
        Console.WriteLine($"[TICKET SERVICE] Inicializado com URL: {App.ApiBaseUrl}");
    }

    /// <summary>
    /// Obtém lista de todos os usuários admin
    /// </summary>
    public async Task<List<AdminUserResponse>?> GetAdminUsersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/tickets/admins");
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TICKET SERVICE] Erro ao buscar admins: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<AdminUserResponse>>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TICKET SERVICE] Erro: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Cria um novo ticket para um admin
    /// </summary>
    public async Task<Ticket?> CreateTicketAsync(Guid userId, CreateTicketRequest request)
    {
        try
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"api/tickets/user/{userId}", content);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TICKET SERVICE] Erro ao criar ticket: {response.StatusCode}");
                return null;
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Ticket>(responseJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TICKET SERVICE] Erro: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Obtém todos os tickets do usuário
    /// </summary>
    public async Task<List<TicketInfoResponse>?> GetUserTicketsAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/tickets/user/{userId}");
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TICKET SERVICE] Erro ao buscar tickets: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TicketInfoResponse>>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TICKET SERVICE] Erro: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Reescreve um texto usando IA
    /// </summary>
    public async Task<string?> RewriteTextAsync(Guid ticketId, string text)
    {
        try
        {
            var request = new RewriteTextRequest { Text = text };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"api/tickets/{ticketId}/rewrite", content);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TICKET SERVICE] Erro ao reescrever texto: {response.StatusCode}");
                return null;
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var rewriteResponse = JsonConvert.DeserializeObject<RewriteResponse>(responseJson);
            return rewriteResponse?.Text;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TICKET SERVICE] Erro: {ex.Message}");
            return null;
        }
    }
}
