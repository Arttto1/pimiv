using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KanbanDesktop.Services;
using KanbanDesktop.Models;

namespace KanbanDesktop.Views;

public partial class MainWindow : Window
{
    private readonly ApiService _apiService;
    private List<Column> _columns = new();

    public MainWindow()
    {
        InitializeComponent();
        _apiService = new ApiService();
        UsernameText.Text = $"Olá, {App.CurrentUsername}";
        Console.WriteLine("[MAIN] Janela principal inicializada");
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadColumnsAsync();
    }

    private async Task LoadColumnsAsync()
    {
        try
        {
            Console.WriteLine("[MAIN] Carregando colunas...");
            _columns = await _apiService.GetColumnsAsync(App.CurrentUserId!.Value);

            ColumnsPanel.Items.Clear();

            foreach (var column in _columns)
            {
                var columnControl = CreateColumnControl(column);
                ColumnsPanel.Items.Add(columnControl);
            }

            Console.WriteLine($"[MAIN] {_columns.Count} colunas carregadas");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MAIN ERRO] Falha ao carregar colunas: {ex.Message}");
            MessageBox.Show("Erro ao carregar colunas", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private Border CreateColumnControl(Column column)
    {
        var columnBorder = new Border
        {
            Width = 300,
            Margin = new Thickness(0, 0, 15, 0),
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A")),
            BorderBrush = new SolidColorBrush(GetColorFromName(column.Color)),
            BorderThickness = new Thickness(2),
            CornerRadius = new CornerRadius(8),
            Tag = column
        };

        var mainGrid = new Grid();
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Header
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Cards
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Button

        var header = new Grid
        {
            Background = new SolidColorBrush(GetColorFromName(column.Color)),
            Height = 50,
            Margin = new Thickness(0)
        };

        var columnName = new TextBlock
        {
            Text = column.Name,
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.White,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(15, 0, 0, 0)
        };

        header.Children.Add(columnName);

        // Só adiciona botão de deletar se a coluna for deletável
        if (column.IsDeletable)
        {
            var deleteButton = new Button
            {
                Content = "X",
                Width = 30,
                Height = 30,
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = column,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 15, 0),
                FontWeight = FontWeights.Bold,
                FontSize = 14
            };
            deleteButton.Click += DeleteColumn_Click;
            header.Children.Add(deleteButton);
        }

        Grid.SetRow(header, 0);
        mainGrid.Children.Add(header);

        var cardsScroll = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Margin = new Thickness(10)
        };

        var cardsPanel = new StackPanel
        {
            Tag = column.Id,
            AllowDrop = true,
            Background = Brushes.Transparent // Necessário para receber eventos de drop
        };

        cardsPanel.Drop += CardsPanel_Drop;
        cardsPanel.DragOver += CardsPanel_DragOver;

        cardsScroll.Content = cardsPanel;
        Grid.SetRow(cardsScroll, 1);
        mainGrid.Children.Add(cardsScroll);

        var addCardButton = new Button
        {
            Content = "+ Novo Card",
            Height = 35,
            Margin = new Thickness(10, 0, 10, 10),
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00AA00")),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            Cursor = System.Windows.Input.Cursors.Hand,
            Tag = column
        };
        addCardButton.Click += AddCard_Click;
        Grid.SetRow(addCardButton, 2);
        mainGrid.Children.Add(addCardButton);

        columnBorder.Child = mainGrid;

        LoadCardsForColumn(column.Id, cardsPanel);

        return columnBorder;
    }

    private async void LoadCardsForColumn(Guid columnId, StackPanel cardsPanel)
    {
        try
        {
            var cards = await _apiService.GetCardsAsync(columnId);
            cardsPanel.Children.Clear();

            foreach (var card in cards)
            {
                var cardControl = CreateCardControl(card);
                cardsPanel.Children.Add(cardControl);
            }

            Console.WriteLine($"[MAIN] {cards.Count} cards carregados para coluna {columnId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MAIN ERRO] Falha ao carregar cards: {ex.Message}");
        }
    }

    private Border CreateCardControl(Card card)
    {
        var cardBorder = new Border
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2A2A")),
            BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(5),
            Margin = new Thickness(0, 0, 0, 10),
            Padding = new Thickness(10),
            Cursor = System.Windows.Input.Cursors.Hand,
            Tag = card,
            AllowDrop = true
        };

        cardBorder.MouseLeftButtonUp += Card_Click;
        cardBorder.PreviewMouseLeftButtonDown += Card_MouseDown;
        cardBorder.PreviewMouseMove += Card_MouseMove;
        cardBorder.Drop += Card_Drop;
        cardBorder.DragEnter += Card_DragEnter;

        var stack = new StackPanel();

        var title = new TextBlock
        {
            Text = card.Title,
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.White,
            TextWrapping = TextWrapping.Wrap
        };

        var description = new TextBlock
        {
            Text = card.Description,
            FontSize = 12,
            Foreground = Brushes.LightGray,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 5, 0, 0),
            MaxHeight = 60
        };

        stack.Children.Add(title);
        if (!string.IsNullOrEmpty(card.Description))
        {
            stack.Children.Add(description);
        }

        cardBorder.Child = stack;
        return cardBorder;
    }

    private Color GetColorFromName(string colorName)
    {
        return colorName.ToLower() switch
        {
            "red" => Colors.Red,
            "blue" => Colors.Blue,
            "green" => Colors.Green,
            "yellow" => Colors.Yellow,
            "orange" => Colors.Orange,
            "pink" => Colors.Pink,
            "brown" => Colors.Brown,
            "black" => Colors.Black,
            "white" => Colors.White,
            "gray" => Colors.Gray,
            _ => Colors.Green
        };
    }

    private async void AddColumnButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AddColumnDialog();
        if (dialog.ShowDialog() == true)
        {
            var column = await _apiService.CreateColumnAsync(
                App.CurrentUserId!.Value,
                dialog.ColumnName,
                dialog.ColumnColor
            );

            if (column != null)
            {
                await LoadColumnsAsync();
                Console.WriteLine("[MAIN] Coluna adicionada com sucesso");
            }
            else
            {
                MessageBox.Show("Erro ao criar coluna", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void DeleteColumn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Column column)
        {
            var result = MessageBox.Show(
                $"Deseja realmente excluir a coluna '{column.Name}' e todos os seus cards?",
                "Confirmar exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                var success = await _apiService.DeleteColumnAsync(column.Id);
                if (success)
                {
                    await LoadColumnsAsync();
                    Console.WriteLine("[MAIN] Coluna excluída com sucesso");
                }
                else
                {
                    MessageBox.Show("Erro ao excluir coluna", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private async void AddCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Column column)
        {
            var dialog = new AddCardDialog();
            if (dialog.ShowDialog() == true)
            {
                var card = await _apiService.CreateCardAsync(
                    column.Id,
                    dialog.CardTitle,
                    dialog.CardDescription
                );

                if (card != null)
                {
                    await LoadColumnsAsync();
                    Console.WriteLine("[MAIN] Card adicionado com sucesso");
                }
                else
                {
                    MessageBox.Show("Erro ao criar card", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void Card_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Border border && border.Tag is Card card && !_isDragging)
        {
            var dialog = new CardDetailsDialog(card, _apiService, _columns);
            if (dialog.ShowDialog() == true)
            {
                _ = LoadColumnsAsync();
            }
        }
    }

    private Point _startPoint;
    private bool _isDragging;
    private Card? _draggedCard;

    private void Card_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _startPoint = e.GetPosition(null);
        _isDragging = false;
        _draggedCard = null;
    }

    private void Card_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && sender is Border border)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = _startPoint - mousePos;

            if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                _isDragging = true;
                _draggedCard = border.Tag as Card;

                if (_draggedCard != null)
                {
                    Console.WriteLine($"[DRAG] ✅ Iniciando drag do card: {_draggedCard.Title} (ID: {_draggedCard.Id})");
                    Console.WriteLine($"[DRAG] Coluna atual: {_draggedCard.ColumnId}");
                    
                    // Criar um DataObject com o card
                    var data = new DataObject(typeof(Card), _draggedCard);
                    
                    // Adicionar efeito visual ao card durante o drag
                    border.Opacity = 0.5;
                    
                    // Iniciar o drag and drop
                    var result = DragDrop.DoDragDrop(border, data, DragDropEffects.Move);
                    
                    // Restaurar opacidade após o drop
                    border.Opacity = 1.0;
                    
                    // Reset dragging flag após o drop
                    _isDragging = false;
                }
            }
        }
    }

    private void Card_DragEnter(object sender, DragEventArgs e)
    {
        Console.WriteLine($"[DRAG] Mouse sobre área de drop");
        if (!e.Data.GetDataPresent(typeof(Card)))
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private async void Card_Drop(object sender, DragEventArgs e)
    {
        Console.WriteLine($"\n[DROP] ⬇️ DROP DETECTADO!");
        
        if (e.Data.GetDataPresent(typeof(Card)) && sender is Border targetBorder)
        {
            var droppedCard = e.Data.GetData(typeof(Card)) as Card;
            
            if (droppedCard != null && targetBorder.Parent is StackPanel stackPanel && stackPanel.Tag is Guid newColumnId)
            {
                Console.WriteLine($"[DROP] Card: {droppedCard.Title}");
                Console.WriteLine($"[DROP] Coluna alvo: {newColumnId}");
                Console.WriteLine($"[DROP] Coluna origem: {droppedCard.ColumnId}");

                if (droppedCard.ColumnId != newColumnId)
                {
                    Console.WriteLine($"[DROP] 🔄 Movendo card entre colunas...");
                    var request = new UpdateCardRequest { ColumnId = newColumnId };
                    var result = await _apiService.UpdateCardAsync(droppedCard.Id, request);

                    if (result != null)
                    {
                        Console.WriteLine("[DROP] ✅ Card movido com sucesso!");
                        await LoadColumnsAsync();
                    }
                    else
                    {
                        Console.WriteLine("[DROP] ❌ Falha ao mover card!");
                        MessageBox.Show("Erro ao mover card", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    Console.WriteLine($"[DROP] ℹ️ Card já está nesta coluna");
                }
            }
        }

        _isDragging = false;
        _draggedCard = null;
    }

    private void CardsPanel_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(Card)))
        {
            e.Effects = DragDropEffects.Move;
            Console.WriteLine($"[DRAG] ✅ Cursor sobre área válida");
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        e.Handled = true;
    }

    private async void CardsPanel_Drop(object sender, DragEventArgs e)
    {
        Console.WriteLine($"\n[DROP] ⬇️ DROP NO PAINEL DETECTADO!");
        
        if (e.Data.GetDataPresent(typeof(Card)) && sender is StackPanel stackPanel && stackPanel.Tag is Guid newColumnId)
        {
            var droppedCard = e.Data.GetData(typeof(Card)) as Card;
            
            if (droppedCard != null)
            {
                Console.WriteLine($"[DROP] Card: {droppedCard.Title}");
                Console.WriteLine($"[DROP] Coluna alvo: {newColumnId}");
                Console.WriteLine($"[DROP] Coluna origem: {droppedCard.ColumnId}");

                if (droppedCard.ColumnId != newColumnId)
                {
                    Console.WriteLine($"[DROP] 🔄 Movendo card entre colunas...");
                    var request = new UpdateCardRequest { ColumnId = newColumnId };
                    var result = await _apiService.UpdateCardAsync(droppedCard.Id, request);

                    if (result != null)
                    {
                        Console.WriteLine("[DROP] ✅ Card movido com sucesso!");
                        await LoadColumnsAsync();
                    }
                    else
                    {
                        Console.WriteLine("[DROP] ❌ Falha ao mover card!");
                        MessageBox.Show("Erro ao mover card", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    Console.WriteLine($"[DROP] ℹ️ Card já está nesta coluna");
                }
            }
        }

        _isDragging = false;
        _draggedCard = null;
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
            Console.WriteLine($"[MAIN] 🚪 Logout do usuário: {App.CurrentUsername}");
            
            // Limpar sessão salva
            SessionManager.ClearSession();
            
            // Limpar dados atuais
            App.CurrentUserId = null;
            App.CurrentUsername = null;
            App.IsAdmin = false;

            // Mostrar tela de login novamente (não como dialog)
            var loginWindow = new LoginWindow(isDialog: false);
            loginWindow.Closed += (s, args) =>
            {
                // Quando o login fechar, se não houver CurrentUserId, encerra a aplicação
                if (!App.CurrentUserId.HasValue)
                {
                    Application.Current.Shutdown();
                }
            };
            loginWindow.Show();
            
            // Fechar a janela atual
            this.Close();
        }
    }
}