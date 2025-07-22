using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public LettersGrid Grid { get; } = null!;

    public LetterSequence? LetterSequence { get; private set; }
    private List<string> UsedWords = [];

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


    public GameWindowViewModel()
    {
        Grid = new();
        var gridSize = Settings.Instance.GridSize;
        var word = OfflineDictionary.GetRandom;
        for (var i = 0; i < 5; i++)
        {
            Grid.Get(gridSize / 2, (gridSize - 5) / 2 + i).Letter = word[i];
        }
        UsedWords.Add(word);
    }

    public void ResetChosenButton() => Grid.ResetSelectedButton();

    private void EndEnteringLetter()
    {
        DisableInput();
        //ResetChosenButton();
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
        if (ChosenButton != null && char.IsLetter(input))
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

    public async Task OnApplyButtonClick(object? sender, RoutedEventArgs e)
    {
        ApplyVisible = false;

        var word = LetterSequence!.GetWord();
        var reversedWord = new string([.. word.Reverse()]);

        var res =
            await OnlineWordChecker.ExistsInWiktionary(word)
            || await OnlineWordChecker.ExistsInWiktionary(reversedWord);
        if (res)
        {
            // Successful, next round
            // Console.WriteLine(word);
            var wordCost = word.Length - UsedWords.Count(s => s == word) - UsedWords.Count(s => s == reversedWord);

            if (FirstPlayer.IsMakingMove)
                FirstPlayer.Score += wordCost;
            else
                SecondPlayer.Score += wordCost;

            UsedWords.Add(word);

            (FirstPlayer.IsMakingMove, SecondPlayer.IsMakingMove)
                = (SecondPlayer.IsMakingMove, FirstPlayer.IsMakingMove);
            Round++;

            ClearButtonsSelections();
            ClearOldButtons();
            ResetChosenButton();
        }
        else
        {
            UndoChanges();
        }

        LetterSequence = null;

        Mode = GameMode.LetterChoosing;
    }

    public void UndoChanges()
    {
        if (ChosenButton != null)
            ChosenButton.Letter = ' ';
        Mode = GameMode.LetterChoosing;
        LetterSequence = null;
        //ClearOldButtons();
        ClearButtonsSelections();
        ApplyVisible = false;
        EndEnteringLetter();
        ResetChosenButton();
    }

    public void Skip()
    {
        UndoChanges();
        ClearOldButtons();
        Round++;
        (FirstPlayer.IsMakingMove, SecondPlayer.IsMakingMove)
            = (SecondPlayer.IsMakingMove, FirstPlayer.IsMakingMove);
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