using System;
using AntiBaldaGame.Models;
using AntiBaldaGame.Views;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string? _inputFieldText = string.Empty;
    [ObservableProperty] private bool _inputVisible = false;

    public LettersGrid Grid { get; } = new(MainWindow.GridSize);

    public LetterButton? ChosenButton =>
        Grid is { SelectedColumn: > -1, SelectedRow: > -1 }
            ? Grid.Get(Grid.SelectedRow, Grid.SelectedColumn)
            : null;

    public void ResetChosenButton() => Grid.ResetSelectedButton();

    public void DisableInput()
    {
        InputFieldText = string.Empty;
        InputVisible = false;
        ResetChosenButton();
    }

    public void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        InputFieldText = " ";
        InputVisible = true;
    }
    
    private const string Alphabet = "йцукенгшщзхъфывапролджэячсмитьбю";

    public void OnTextChange(object? sender, TextChangedEventArgs e)
    {
        if (InputFieldText?.Length >= 1)
        {
            var lastChar = InputFieldText[^1];
            if (lastChar.ToString() == InputFieldText)
                return;
            if (!Alphabet.Contains(lastChar))
                lastChar = ' ';
            InputFieldText = lastChar.ToString();
        }
        else
        {
            InputFieldText = " ";
        }
        

        var input = InputFieldText[0];
        if (ChosenButton != null)
            ChosenButton.Letter = input;
    }

    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;
        
        DisableInput();
    }
}