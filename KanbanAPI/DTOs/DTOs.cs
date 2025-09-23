namespace KanbanAPI.DTOs;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class CreateColumnRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class UpdateColumnRequest
{
    public string? Name { get; set; }
    public string? Color { get; set; }
    public int? Position { get; set; }
}

public class CreateCardRequest
{
    public Guid ColumnId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateCardRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Position { get; set; }
    public Guid? ColumnId { get; set; }
}

public class RewriteRequest
{
    public string Text { get; set; } = string.Empty;
}

public class RewriteTextRequest
{
    public string Text { get; set; } = string.Empty;
}

public class RewriteResponse
{
    public string Text { get; set; } = string.Empty;
}

public class CreateTicketRequest
{
    public Guid AdminId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AdminUserResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
}

public class TicketInfoResponse
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AdminUsername { get; set; } = string.Empty;
    public string CurrentColumnName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
