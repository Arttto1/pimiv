using System.Net.Http.Json;
using KanbanWeb.Models;

namespace KanbanWeb.Services;

public class TicketService
{
    private readonly HttpClient _httpClient;

    public TicketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

            return await response.Content.ReadFromJsonAsync<List<AdminUserResponse>>();
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
            var response = await _httpClient.PostAsJsonAsync($"api/tickets/user/{userId}", request);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TICKET SERVICE] Erro ao criar ticket: {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Ticket>();
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

            return await response.Content.ReadFromJsonAsync<List<TicketInfoResponse>>();
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
            var response = await _httpClient.PostAsJsonAsync($"api/tickets/{ticketId}/rewrite", request);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TICKET SERVICE] Erro ao reescrever texto: {response.StatusCode}");
                return null;
            }

            var rewriteResponse = await response.Content.ReadFromJsonAsync<RewriteResponse>();
            return rewriteResponse?.Text;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TICKET SERVICE] Erro: {ex.Message}");
            return null;
        }
    }
}
