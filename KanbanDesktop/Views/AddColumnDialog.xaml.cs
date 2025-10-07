using System.Windows;
using System.Windows.Controls;

namespace KanbanDesktop.Views;

public partial class AddColumnDialog : Window
{
    public string ColumnName { get; private set; } = string.Empty;
    public string ColumnColor { get; private set; } = string.Empty;

    public AddColumnDialog()
    {
        InitializeComponent();
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        var name = NameTextBox.Text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            MessageBox.Show("Digite um nome para a coluna", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ColumnName = name;
        ColumnColor = (ColorComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "green";

        Console.WriteLine($"[DIALOG] Coluna criada: {ColumnName} - {ColumnColor}");

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
