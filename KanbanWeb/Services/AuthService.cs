namespace KanbanWeb.Services;

public class AuthService
{
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsAuthenticated => UserId.HasValue;

    public void Login(Guid userId, string username, bool isAdmin)
    {
        UserId = userId;
        Username = username;
        IsAdmin = isAdmin;
        Console.WriteLine($"[AUTH] Usuário autenticado: {username} (Admin: {isAdmin})");
    }

    public void Logout()
    {
        UserId = null;
        Username = null;
        IsAdmin = false;
        Console.WriteLine("[AUTH] Usuário deslogado");
    }
}
