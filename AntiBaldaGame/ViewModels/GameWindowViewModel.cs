using System;
using System.Linq;
using AntiBaldaGame.Models;
using AntiBaldaGame.Views;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.ViewModels;

public partial class GameWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string? _inputFieldText = string.Empty;
    [ObservableProperty] private bool _inputVisible = false;
    [ObservableProperty] private int _round = 1;

    public LettersGrid Grid { get; } = new();
    
    public Player FirstPlayer { get; } = new()
    {
        Name = Settings.Instance.Name
    };
    public Player SecondPlayer { get; } = new()
    {
        Name = "Бибурат"
    };
    
    

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
        if (InputFieldText?.Trim().Length != 0)
            InputFieldText = InputFieldText?.Trim();
        if (InputFieldText?.Length >= 1)
        {
            var lastChar = InputFieldText[^1];
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
        FirstPlayer.Score++;
        SecondPlayer.Score++;
        Round = FirstPlayer.Score / 2;
    }
}