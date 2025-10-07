using System.Windows;
using KanbanDesktop.Services;
using KanbanDesktop.Views;

namespace KanbanDesktop;

public partial class App : Application
{
    public static string ApiBaseUrl { get; set; } = "http://localhost:5000";
    public static Guid? CurrentUserId { get; set; }
    public static string? CurrentUsername { get; set; }
    public static bool IsAdmin { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Configurar para não fechar automaticamente
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // Tentar carregar sessão salva
        var session = SessionManager.LoadSession();

        if (session != null)
        {
            // Sessão válida encontrada - auto login
            CurrentUserId = session.UserId;
            CurrentUsername = session.Username;
            IsAdmin = session.IsAdmin;

            Console.WriteLine($"[APP] 🔓 Login automático: {CurrentUsername} (Admin: {IsAdmin})");

            // Verificar se é admin ou não para abrir a janela correta
            Window mainWindow;
            if (IsAdmin)
            {
                // Admin: abre MainWindow (Kanban)
                mainWindow = new MainWindow();
            }
            else
            {
                // Não-admin: abre TicketsWindow
                mainWindow = new TicketsWindow(session.UserId, session.Username);
            }

            mainWindow.Closed += OnMainWindowClosed;
            mainWindow.Show();
            MainWindow = mainWindow;
        }
        else
        {
            // Sem sessão válida - mostrar login
            Console.WriteLine($"[APP] 🔒 Nenhuma sessão válida - mostrando login");

            var loginWindow = new LoginWindow(false);
            loginWindow.Closed += OnLoginWindowClosed;
            loginWindow.Show();
            MainWindow = loginWindow;
        }
    }

    private void OnMainWindowClosed(object? sender, EventArgs e)
    {
        Console.WriteLine("[APP] Janela principal fechada - encerrando aplicação");
        Shutdown();
    }

    private void OnLoginWindowClosed(object? sender, EventArgs e)
    {
        // Se o login foi bem-sucedido, CurrentUserId estará preenchido
        if (CurrentUserId.HasValue)
        {
            Console.WriteLine("[APP] Login bem-sucedido - abrindo janela principal");
            Window mainWindow;
            if (IsAdmin)
            {
                mainWindow = new MainWindow();
            }
            else
            {
                mainWindow = new TicketsWindow(CurrentUserId.Value, CurrentUsername!);
            }

            mainWindow.Closed += OnMainWindowClosed;
            mainWindow.Show();
            MainWindow = mainWindow;
        }
        else
        {
            // Login foi cancelado ou falhou
            Console.WriteLine("[APP] Login cancelado - encerrando aplicação");
            Shutdown();
        }
    }
}
