using System;
using AntiBaldaGame.Models;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.ViewModels;

public partial class GameWindowViewModel : ViewModelBase
{
    public enum GameMode
    {
        LetterChoosing,
        LetterCombining
    }

    public GameMode Mode { get; private set; }

    [ObservableProperty] private string? _inputFieldText = string.Empty;
    [ObservableProperty] private bool _inputVisible = false;
    [ObservableProperty] private bool _applyVisible = false;
    [ObservableProperty] private int _round = 1;

    public LettersGrid Grid { get; } = new();

    public LetterSequence? LetterSequence { get; private set; }

    public Player FirstPlayer { get; } = new()
    {
        Name = Settings.Instance.Name,
        IsMakingMove = true,
    };
    public Player SecondPlayer { get; } = new()
    {
        Name = "Бибурат",
        IsMakingMove = false,
    };

    public LetterButton? ChosenButton =>
        Grid is { SelectedColumn: > -1, SelectedRow: > -1 }
            ? Grid.Get(Grid.SelectedRow, Grid.SelectedColumn)
            : null;

    public void ResetChosenButton() => Grid.ResetSelectedButton();


    private void EndEnteringLetter()
    {
        DisableInput();
        ResetChosenButton();
        previousLetterOnChosenButton = '-';
    }

    public void DisableInput()
    {
        InputFieldText = string.Empty;
        InputVisible = false;
    }

    public void OnGridButtonClick(object? sender, RoutedEventArgs e)
    {
        if (Mode == GameMode.LetterChoosing)
        {
            if (ChosenButton != null && previousLetterOnChosenButton != '-')
            {
                ChosenButton.Letter = previousLetterOnChosenButton;
                previousLetterOnChosenButton = '-';
            }

            if (sender is Button button && button.Content is char s && s is not ' ' or '\0')
            {
                DisableInput();
                return;
            }

            InputFieldText = " ";
            InputVisible = true;
        }
    }

    private char previousLetterOnChosenButton = '-';
    public void OnTextChange(object? sender, TextChangedEventArgs e)
    {
        var input = StringFormatter.LeaveOneCharacter(InputFieldText);
        InputFieldText = input.ToString();
        if (ChosenButton != null)
        {
            if (previousLetterOnChosenButton == '-')
                previousLetterOnChosenButton = ChosenButton.Letter;
            ChosenButton.Letter = input;
        }

    }

    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        if (Mode == GameMode.LetterCombining)
            return;

        ChosenButton!.SpawnTime = Round;
        ChosenButton!.Color = CustomColors.White;
        LetterSequence = new LetterSequence(new(ChosenButton, Grid.SelectedRow, Grid.SelectedColumn));
        EndEnteringLetter();
        ApplyVisible = true;


        Mode = GameMode.LetterCombining;
    }

    public void OnApplyButtonClick(object? sender, RoutedEventArgs e)
    {
        ApplyVisible = false;
        ClearButtonsSelections();
        ClearOldButtons();

        var word = LetterSequence!.GetWord();
        //Console.WriteLine(word);
        // TODO Проверка корректности слова
        LetterSequence = null;

        if (FirstPlayer.IsMakingMove)
            FirstPlayer.Score += word.Length;
        else
            SecondPlayer.Score += word.Length;
        (FirstPlayer.IsMakingMove, SecondPlayer.IsMakingMove)
            = (SecondPlayer.IsMakingMove, FirstPlayer.IsMakingMove);
        Round++;

        Mode = GameMode.LetterChoosing;
    }

    private void ClearButtonsSelections()
    {
        for (var i = 0; i < Settings.Instance.GridSize; i++)
        {
            for (var j = 0; j < Settings.Instance.GridSize; j++)
            {
                Grid.Get(i, j).IsSelected = false;
            }
        }
    }

    private void ClearOldButtons()
    {
        for (var i = 0; i < Settings.Instance.GridSize; i++)
        {
            for (var j = 0; j < Settings.Instance.GridSize; j++)
            {
                var button = Grid.Get(i, j);
                if (button.SpawnTime == -1)
                    continue;
                if (button.SpawnTime + Settings.Instance.TimeOut == Round)
                {
                    button.Letter = ' ';
                    button.SpawnTime = -1;
                }
            }
        }
    }
}