using System;
using AntiBaldaGame.Models;
using AntiBaldaGame.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AntiBaldaGame.Views;

public partial class GameWindow : Window
{
    public GameWindow()
    {
        InitializeComponent();
        DataContext = new GameWindowViewModel();
        InitializeButtonGrid();
        ViewModel.DisableInput();
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
                };
                
                button.Bind(ContentProperty, new Binding
                {
                    Source = grid.Get(row,col),
                    Path = nameof(LetterButton.Letter),
                    Mode = BindingMode.TwoWay,
                });
                
                button.Click += ViewModel.Grid.SelectButton(row, col);
                //button.Click += ViewModel.OnButtonClick;
                button.Click += Button_Click;

                Grid.SetRow(button, row);
                Grid.SetColumn(button, col);
                ButtonGrid.Children.Add(button);
            }
        }
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel.OnButtonClick(sender, e);
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
}