using System.Windows;
using KanbanDesktop.Services;
using KanbanDesktop.Models;

namespace KanbanDesktop.Views;

public partial class TicketsWindow : Window
{
    private readonly TicketService _ticketService;
    private readonly ApiService _apiService;
    private Guid _currentUserId;
    private string _currentUsername;

    public TicketsWindow(Guid userId, string username)
    {
        InitializeComponent();
        _ticketService = new TicketService();
        _apiService = new ApiService();
        _currentUserId = userId;
        _currentUsername = username;

        UserInfoText.Text = $"Usuário: {username}";

        LoadData();
    }

    private async void LoadData()
    {
        try
        {
            // Carregar lista de admins
            var admins = await _ticketService.GetAdminUsersAsync();
            if (admins != null)
            {
                AdminComboBox.ItemsSource = admins;
                if (admins.Count > 0)
                {
                    AdminComboBox.SelectedIndex = 0;
                }
            }

            // Carregar tickets do usuário
            await LoadUserTickets();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TICKETS WINDOW] Erro ao carregar dados: {ex.Message}");
            MessageBox.Show("Erro ao carregar dados", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task LoadUserTickets()
    {
        try
        {
            var tickets = await _ticketService.GetUserTicketsAsync(_currentUserId);
            if (tickets != null)
            {
                TicketsItemsControl.ItemsSource = tickets;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TICKETS WINDOW] Erro ao carregar tickets: {ex.Message}");
        }
    }

    private async void CreateTicketButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            StatusMessage.Visibility = Visibility.Collapsed;

            var title = TitleTextBox.Text.Trim();
            var description = DescriptionTextBox.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Preencha o título e a descrição", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AdminComboBox.SelectedItem == null)
            {
                MessageBox.Show("Selecione um admin", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedAdmin = (AdminUserResponse)AdminComboBox.SelectedItem;

            var request = new CreateTicketRequest
            {
                AdminId = selectedAdmin.Id,
                Title = title,
                Description = description
            };

            var ticket = await _ticketService.CreateTicketAsync(_currentUserId, request);

            if (ticket != null)
            {
                StatusMessage.Text = "✅ Chamado criado com sucesso!";
                StatusMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                StatusMessage.Visibility = Visibility.Visible;

                // Limpar campos
                TitleTextBox.Clear();
                DescriptionTextBox.Clear();

                // Recarregar lista de tickets
                await LoadUserTickets();
            }
            else
            {
                StatusMessage.Text = "❌ Erro ao criar chamado";
                StatusMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                StatusMessage.Visibility = Visibility.Visible;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TICKETS WINDOW] Erro ao criar ticket: {ex.Message}");
            MessageBox.Show("Erro ao criar chamado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void RewriteButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var description = DescriptionTextBox.Text.Trim();

            if (string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Digite uma descrição para melhorar", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            RewriteButton.IsEnabled = false;
            RewriteButton.Content = "⏳ Melhorando...";

            // Usar Guid vazio como ticketId pois ainda não foi criado
            var rewrittenText = await _ticketService.RewriteTextAsync(Guid.Empty, description);

            if (!string.IsNullOrEmpty(rewrittenText))
            {
                DescriptionTextBox.Text = rewrittenText;
                StatusMessage.Text = "✅ Texto melhorado com IA!";
                StatusMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                StatusMessage.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Erro ao melhorar texto", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TICKETS WINDOW] Erro ao reescrever: {ex.Message}");
            MessageBox.Show("Erro ao melhorar texto", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            RewriteButton.IsEnabled = true;
            RewriteButton.Content = "🤖 Melhorar com IA";
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshButton.IsEnabled = false;
        RefreshButton.Content = "⏳ Atualizando...";

        await LoadUserTickets();

        RefreshButton.IsEnabled = true;
        RefreshButton.Content = "🔄 Atualizar";
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Deseja realmente sair?",
            "Confirmar Logout",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );

        if (result == MessageBoxResult.Yes)
        {
            Console.WriteLine($"[TICKETS] 🚪 Logout do usuário: {App.CurrentUsername}");
            
            // Limpar sessão
            SessionManager.ClearSession();
            App.CurrentUserId = null;
            App.CurrentUsername = null;
            App.IsAdmin = false;
            
            // Mostrar tela de login
            var loginWindow = new LoginWindow(false);
            loginWindow.Closed += (s, args) =>
            {
                // Quando o login fechar, se não houver CurrentUserId, encerra a aplicação
                if (!App.CurrentUserId.HasValue)
                {
                    Application.Current.Shutdown();
                }
            };
            loginWindow.Show();
            
            Close();
        }
    }
}
