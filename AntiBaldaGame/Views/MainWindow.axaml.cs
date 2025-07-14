using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AntiBaldaGame.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeButtonGrid();
        InputField.IsVisible = false;
        InputSign.IsVisible = false;
    }
    
    private const int GridSize = 5;
    
    private void InitializeButtonGrid()
    {
        for (var i = 0; i < GridSize; i++)
        {
            ButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ButtonGrid.RowDefinitions.Add(new RowDefinition());
        }
        
        for (var row = 0; row < GridSize; row++)
        {
            for (var col = 0; col < GridSize; col++)
            {
                var button = new Button
                {
                    Content = "    ",
                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    Margin = new Thickness(2),
                    Width = 30,
                    Height = 30,
                };
                
                button.Click += Button_Click;

                Grid.SetRow(button, row);
                Grid.SetColumn(button, col);
                ButtonGrid.Children.Add(button);
            }
        }
    }

    private Button? _chosenButton;

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            _chosenButton = button;
            InputField.IsVisible = true;
            InputSign.IsVisible = true;
        }
    }

    private void InputField_OnTextChange(object? sender, TextChangedEventArgs e)
    {
        if (InputField.Text?.Length > 1)
            InputField.Text = InputField.Text[^1].ToString();
        var input = InputField.Text;
        if (_chosenButton != null)
            _chosenButton.Content = string.IsNullOrEmpty(input) ? "" : input;
    }

    private void InputField_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;
        
        InputField.IsVisible = false;
        InputSign.IsVisible = false;
        InputField.Text = "";
        _chosenButton = null;
            
        //TODO тут сделать поиск слов
    }
}