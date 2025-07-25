using AntiBaldaGame.Models;
using AntiBaldaGame.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace AntiBaldaGame.Views;

public partial class GameWindow : Window
{
    public GameWindow()
    {
        InitializeComponent();
        DataContext = new GameWindowViewModel();
        InitializeButtonGrid();
        ViewModel.DisableInput();
        Closing += CloseGameWindow;
        var m = MultiplayerHandler.Instance;
        if (m.IsNetworkGame)
        {
            m.Chat!.OnGameExit += Close;
            m.Chat!.OnSkipRequested += ViewModel.SkipChecked;
            m.Chat!.OnNextRoundRequested += ViewModel.NextRound;
            // (a, b, c, d) => {
            //     Dispatcher.UIThread.Invoke(() => ViewModel.NextRound(a,b,c,d));
            // };
        }
        ViewModel.OnClose += Close;
        m.IsFirstPlayerMakingMove = true;
    }

    private GameWindowViewModel ViewModel => (GameWindowViewModel)DataContext!;

    private void InitializeButtonGrid()
    {
        var grid = ViewModel.Grid;
        var gridSize = Settings.Instance.GridSize;
        for (var i = 0; i < gridSize; i++)
        {
            ButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ButtonGrid.RowDefinitions.Add(new RowDefinition());
        }

        for (var row = 0; row < gridSize; row++)
        {
            for (var col = 0; col < gridSize; col++)
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
                    //Background = CustomColors.DarkGreen,
                    BorderBrush = CustomColors.White,
                    BorderThickness = new Thickness(2),
                };

                button.Bind(ContentProperty, new Binding
                {
                    Source = grid.Get(row, col),
                    Path = nameof(LetterButton.Letter),
                    Mode = BindingMode.TwoWay,
                });

                button.Bind(BackgroundProperty, new Binding
                {
                    Source = grid.Get(row, col),
                    Path = nameof(LetterButton.Color),
                    Mode = BindingMode.TwoWay,
                });

                button.Bind(ForegroundProperty, new Binding
                {
                    Source = grid.Get(row, col),
                    Path = nameof(LetterButton.TextColor),
                    Mode = BindingMode.TwoWay,
                });

                //button.Click += ViewModel.OnButtonClick; 
                button.Click += Button_Click;
                button.Click += ViewModel.Grid.SelectButton(row, col, ViewModel);

                Grid.SetRow(button, row);
                Grid.SetColumn(button, col);
                ButtonGrid.Children.Add(button);
            }
        }
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.OnGridButtonClick(sender, e);
        InputField.Focus();
    }

    private void InputField_OnTextChange(object? sender, TextChangedEventArgs e)
    {
        ViewModel.OnTextChange(sender, e);
    }

    private void InputField_OnKeyDown(object? sender, KeyEventArgs e)
    {
        ViewModel.OnKeyDown(sender, e);
    }

    private void ApplyButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() => ViewModel.OnApplyButtonClick(sender, e));
    }

    private void UndoButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.UndoChanges();
    }

    private void SkipButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Skip();
    }

    private void CloseGameWindow(object? sender, WindowClosingEventArgs e)
    {
        OpenMainMenu();
    }

    private void OpenMainMenu()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            if (MultiplayerHandler.Instance.IsFirstPlayer)
                MultiplayerHandler.Instance.Chat?.SendExitGameCom();
            new MainWindow().Show();
        });
    }
}