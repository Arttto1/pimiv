using System.Windows;
using KanbanDesktop.Models;
using KanbanDesktop.Services;

namespace KanbanDesktop.Views;

public partial class CardDetailsDialog : Window
{
    private readonly Card _card;
    private readonly ApiService _apiService;
    private readonly List<Column> _columns;

    public CardDetailsDialog(Card card, ApiService apiService, List<Column> columns)
    {
        InitializeComponent();
        _card = card;
        _apiService = apiService;
        _columns = columns;

        TitleTextBox.Text = card.Title;
        DescriptionTextBox.Text = card.Description;

        ColumnComboBox.ItemsSource = _columns;
        ColumnComboBox.SelectedItem = _columns.FirstOrDefault(c => c.Id == card.ColumnId);

        Console.WriteLine($"[DIALOG] Detalhes do card abertos: {card.Title}");
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        var title = TitleTextBox.Text.Trim();

        if (string.IsNullOrEmpty(title))
        {
            MessageBox.Show("Digite um título para o card", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedColumn = ColumnComboBox.SelectedItem as Column;
        
        var request = new UpdateCardRequest
        {
            Title = title,
            Description = DescriptionTextBox.Text.Trim(),
            ColumnId = selectedColumn?.Id != _card.ColumnId ? selectedColumn?.Id : null
        };

        Console.WriteLine($"[DIALOG] Salvando card: {title}");

        var result = await _apiService.UpdateCardAsync(_card.Id, request);

        if (result != null)
        {
            Console.WriteLine("[DIALOG] Card salvo com sucesso");
            DialogResult = true;
            Close();
        }
        else
        {
            MessageBox.Show("Erro ao salvar card", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            $"Deseja realmente excluir o card '{_card.Title}'?",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );

        if (result == MessageBoxResult.Yes)
        {
            Console.WriteLine($"[DIALOG] Excluindo card: {_card.Title}");

            var success = await _apiService.DeleteCardAsync(_card.Id);

            if (success)
            {
                Console.WriteLine("[DIALOG] Card excluído com sucesso");
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Erro ao excluir card", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void RewriteButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            MessageBox.Show("Adicione uma descrição antes de reescrever", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        RewriteButton.IsEnabled = false;
        RewriteButton.Content = "⏳ Reescrevendo...";

        Console.WriteLine("[DIALOG] Reescrevendo descrição com IA...");

        var rewrittenText = await _apiService.RewriteDescriptionAsync(_card.Id);

        if (rewrittenText != null)
        {
            DescriptionTextBox.Text = rewrittenText;
            Console.WriteLine("[DIALOG] Descrição reescrita com sucesso");
            MessageBox.Show("Descrição reescrita com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Erro ao reescrever descrição. Verifique a configuração da IA.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        RewriteButton.IsEnabled = true;
        RewriteButton.Content = "✨ REESCREVER COM IA";
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
