using System.Windows;
using KanbanDesktop.Services;

namespace KanbanDesktop.Views;

public partial class AddCardDialog : Window
{
    public string CardTitle { get; private set; } = string.Empty;
    public string CardDescription { get; private set; } = string.Empty;

    public AddCardDialog()
    {
        InitializeComponent();
    }

    private async void Rewrite_Click(object sender, RoutedEventArgs e)
    {
        var description = DescriptionTextBox.Text.Trim();

        if (string.IsNullOrEmpty(description))
        {
            MessageBox.Show("Digite uma descrição para reescrever", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        RewriteButton.IsEnabled = false;
        RewriteButton.Content = "⏳";

        try
        {
            Console.WriteLine($"[AI] Reescrevendo texto: {description.Substring(0, Math.Min(50, description.Length))}...");

            var apiService = new ApiService();
            var rewrittenText = await apiService.RewriteTextAsync(description);

            if (!string.IsNullOrEmpty(rewrittenText))
            {
                DescriptionTextBox.Text = rewrittenText;
                Console.WriteLine($"[AI] ✅ Texto reescrito com sucesso");
            }
            else
            {
                MessageBox.Show("Erro ao reescrever o texto. Tente novamente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"[AI] ❌ Falha ao reescrever texto");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Console.WriteLine($"[AI] ❌ Exceção: {ex.Message}");
        }
        finally
        {
            RewriteButton.IsEnabled = true;
            RewriteButton.Content = "✨";
        }
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        var title = TitleTextBox.Text.Trim();

        if (string.IsNullOrEmpty(title))
        {
            MessageBox.Show("Digite um título para o card", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        CardTitle = title;
        CardDescription = DescriptionTextBox.Text.Trim();

        Console.WriteLine($"[DIALOG] Card criado: {CardTitle}");

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
