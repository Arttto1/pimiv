namespace KanbanAPI.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public DateTime CreatedAt { get; set; }
}

public class Column
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool IsDeletable { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class Card
{
    public Guid Id { get; set; }
    public Guid ColumnId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Ticket
{
    public Guid Id { get; set; }
    public Guid RequesterId { get; set; }  // Usuário não-admin que criou o chamado
    public Guid AdminId { get; set; }      // Admin que vai receber o chamado
    public Guid CardId { get; set; }       // Card criado na coluna "Chamados" do admin
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
