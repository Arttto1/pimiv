using System.Windows;
using KanbanDesktop.Services;

namespace KanbanDesktop.Views;

public partial class LoginWindow : Window
{
    private readonly ApiService _apiService;
    private bool _isDialog;
    private bool _isRegisterMode = false;

    public LoginWindow(bool isDialog = true)
    {
        InitializeComponent();
        _apiService = new ApiService();
        _isDialog = isDialog;
        Console.WriteLine("[LOGIN] Janela de login inicializada");
        UpdateUI();
    }

    private void LoginTab_Click(object sender, RoutedEventArgs e)
    {
        _isRegisterMode = false;
        UpdateUI();
    }

    private void RegisterTab_Click(object sender, RoutedEventArgs e)
    {
        _isRegisterMode = true;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_isRegisterMode)
        {
            // Modo Cadastro
            LoginTabButton.Background = (System.Windows.Media.Brush)FindResource("CardBlack");
            RegisterTabButton.Background = (System.Windows.Media.Brush)FindResource("DarkGreen");
            
            ConfirmPasswordLabel.Visibility = Visibility.Visible;
            ConfirmPasswordBox.Visibility = Visibility.Visible;
            
            ActionButton.Content = "CADASTRAR";
        }
        else
        {
            // Modo Login
            LoginTabButton.Background = (System.Windows.Media.Brush)FindResource("DarkGreen");
            RegisterTabButton.Background = (System.Windows.Media.Brush)FindResource("CardBlack");
            
            ConfirmPasswordLabel.Visibility = Visibility.Collapsed;
            ConfirmPasswordBox.Visibility = Visibility.Collapsed;
            
            ActionButton.Content = "ENTRAR";
        }
        
        ErrorMessage.Visibility = Visibility.Collapsed;
        SuccessMessage.Visibility = Visibility.Collapsed;
    }

    private async void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isRegisterMode)
        {
            await RegisterAsync();
        }
        else
        {
            await LoginAsync();
        }
    }

    private async Task LoginAsync()
    {
        try
        {
            ErrorMessage.Visibility = Visibility.Collapsed;

            var username = UsernameTextBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorMessage.Text = "Preencha todos os campos";
                ErrorMessage.Visibility = Visibility.Visible;
                Console.WriteLine("[LOGIN] Campos vazios");
                return;
            }

            Console.WriteLine($"[LOGIN] Tentando login para: {username}");

            var response = await _apiService.LoginAsync(username, password);

            if (response == null)
            {
                ErrorMessage.Text = "Usuário ou senha inválidos";
                ErrorMessage.Visibility = Visibility.Visible;
                Console.WriteLine("[LOGIN] Login falhou");
                return;
            }

            Console.WriteLine($"[LOGIN] Login bem-sucedido: {response.UserId}");

            App.CurrentUserId = response.UserId;
            App.CurrentUsername = response.Username;
            App.IsAdmin = response.IsAdmin;

            // Salvar sessão para auto-login futuro
            SessionManager.SaveSession(response.UserId, response.Username, response.IsAdmin);

            if (_isDialog)
            {
                // Se foi aberto como dialog, apenas define o resultado e fecha
                DialogResult = true;
                Close();
            }
            else
            {
                // Se foi aberto como janela inicial, apenas fecha
                // O App.xaml.cs vai detectar que CurrentUserId foi preenchido e abrir a janela correta
                Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOGIN ERRO] {ex.Message}");
            ErrorMessage.Text = "Erro ao realizar login";
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }

    private async Task RegisterAsync()
    {
        try
        {
            ErrorMessage.Visibility = Visibility.Collapsed;
            SuccessMessage.Visibility = Visibility.Collapsed;

            var username = UsernameTextBox.Text.Trim();
            var password = PasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorMessage.Text = "Preencha todos os campos";
                ErrorMessage.Visibility = Visibility.Visible;
                Console.WriteLine("[REGISTER] Campos vazios");
                return;
            }

            if (password != confirmPassword)
            {
                ErrorMessage.Text = "As senhas não coincidem";
                ErrorMessage.Visibility = Visibility.Visible;
                Console.WriteLine("[REGISTER] Senhas diferentes");
                return;
            }

            if (password.Length < 6)
            {
                ErrorMessage.Text = "A senha deve ter no mínimo 6 caracteres";
                ErrorMessage.Visibility = Visibility.Visible;
                Console.WriteLine("[REGISTER] Senha muito curta");
                return;
            }

            Console.WriteLine($"[REGISTER] Tentando cadastrar: {username}");

            var response = await _apiService.RegisterAsync(username, password);

            if (response == null)
            {
                ErrorMessage.Text = "Erro ao cadastrar usuário";
                ErrorMessage.Visibility = Visibility.Visible;
                Console.WriteLine("[REGISTER] Cadastro falhou");
                return;
            }

            Console.WriteLine($"[REGISTER] Cadastro bem-sucedido: {response.UserId}");

            SuccessMessage.Text = "Cadastro realizado! Fazendo login...";
            SuccessMessage.Visibility = Visibility.Visible;

            await Task.Delay(1500);

            // Fazer login automaticamente
            App.CurrentUserId = response.UserId;
            App.CurrentUsername = response.Username;
            App.IsAdmin = response.IsAdmin;

            SessionManager.SaveSession(response.UserId, response.Username, response.IsAdmin);

            if (_isDialog)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                // Se foi aberto como janela inicial, apenas fecha
                // O App.xaml.cs vai detectar que CurrentUserId foi preenchido e abrir a janela correta
                Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[REGISTER ERRO] {ex.Message}");
            ErrorMessage.Text = "Erro ao cadastrar usuário";
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}
