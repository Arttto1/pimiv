using Microsoft.JSInterop;

namespace KanbanWeb.Services;

public class SessionService
{
    private readonly IJSRuntime _jsRuntime;

    public SessionService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveSessionAsync(Guid userId, string username, bool isAdmin)
    {
        try
        {
            var expiresAt = DateTime.Now.AddDays(1).Ticks;
            
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", userId.ToString());
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "username", username);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "isAdmin", isAdmin.ToString());
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "sessionExpires", expiresAt.ToString());

            Console.WriteLine($"[SESSION] ✅ Sessão salva: {username} (Admin: {isAdmin}, expira em 1 dia)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SESSION] ❌ Erro ao salvar sessão: {ex.Message}");
        }
    }

    public async Task<(Guid? userId, string? username, bool isAdmin)> LoadSessionAsync()
    {
        try
        {
            var userIdStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userId");
            var username = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "username");
            var isAdminStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "isAdmin");
            var expiresStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "sessionExpires");

            if (string.IsNullOrEmpty(userIdStr) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(expiresStr))
            {
                Console.WriteLine($"[SESSION] ℹ️ Nenhuma sessão salva encontrada");
                return (null, null, false);
            }

            var expiresAt = new DateTime(long.Parse(expiresStr));

            if (expiresAt < DateTime.Now)
            {
                Console.WriteLine($"[SESSION] ⏰ Sessão expirada em {expiresAt}");
                await ClearSessionAsync();
                return (null, null, false);
            }

            var userId = Guid.Parse(userIdStr);
            var isAdmin = bool.Parse(isAdminStr ?? "false");
            Console.WriteLine($"[SESSION] ✅ Sessão válida: {username} (Admin: {isAdmin})");
            return (userId, username, isAdmin);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SESSION] ❌ Erro ao carregar sessão: {ex.Message}");
            return (null, null, false);
        }
    }

    public async Task ClearSessionAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userId");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "username");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "isAdmin");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "sessionExpires");
            Console.WriteLine($"[SESSION] 🗑️ Sessão removida");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SESSION] ❌ Erro ao limpar sessão: {ex.Message}");
        }
    }
}
