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
                    Content = " ",
                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    Margin = new Thickness(2),
                    Width = 40,
                    Height = 40,
                    FontSize = 20,
                };
                
                button.Click += Button_Click;

                Grid.SetRow(button, row);
                Grid.SetColumn(button, col);
                ButtonGrid.Children.Add(button);
            }
        }
    }

    private Button? _chosenButton;

    private void InputFieldOn()
    {
        InputField.IsVisible = true;
        InputSign.IsVisible = true;
        InputField.Focus();
    }
    
    private void InputFieldOff()
    {
        InputField.IsVisible = false;
        InputSign.IsVisible = false;
        InputField.Text = "";
        _chosenButton = null;
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            _chosenButton = button;
            InputFieldOn();
        }
    }

    private const string alphabet = "йцукенгшщзхъфывапролджэячсмитьбю";
    private void InputField_OnTextChange(object? sender, TextChangedEventArgs e)
    {
        if (InputField.Text?.Length >= 1)
        {
            var lastChar = InputField.Text[^1];
            if (!alphabet.Contains(lastChar))
                lastChar = ' ';
            InputField.Text = lastChar.ToString();
        }
            
        var input = InputField.Text;
        if (_chosenButton != null)
            _chosenButton.Content = string.IsNullOrEmpty(input) ? "" : input;
    }

    private void InputField_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        InputFieldOff();

        //TODO тут сделать поиск слов
    }
}