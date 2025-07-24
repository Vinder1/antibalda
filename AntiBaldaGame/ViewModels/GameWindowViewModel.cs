using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiBaldaGame.Models;
using AntiBaldaGame.Views;
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

    public event Action OnClose = () => { };

    public Player FirstPlayer { get; } = new()
    {
        Name = MultiplayerHandler.Instance.IsFirstPlayer ? Settings.Instance.Name : Settings.Instance.OtherPlayerName,
        IsMakingMove = true,
    };
    public Player SecondPlayer { get; } = new()
    {
        Name = MultiplayerHandler.Instance.IsFirstPlayer ? Settings.Instance.OtherPlayerName : Settings.Instance.Name,
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
        var word = Settings.Instance.StartWord;
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
        if (!MultiplayerHandler.Instance.IsCurrentPlayerMakingMove)
            return;

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
        if (!MultiplayerHandler.Instance.IsCurrentPlayerMakingMove)
            return;

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
        if (!MultiplayerHandler.Instance.IsCurrentPlayerMakingMove)
            return;

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
        if (!MultiplayerHandler.Instance.IsCurrentPlayerMakingMove)
            return;

        ApplyVisible = false;

        var word = LetterSequence!.GetWord();
        var reversedWord = new string([.. word.Reverse()]);

        var res =
            await OnlineWordChecker.ExistsInWiktionary(word)
            || await OnlineWordChecker.ExistsInWiktionary(reversedWord);

        if (res)
        {
            var scoreAdded = word.Length - UsedWords.Count(s => s == word) - UsedWords.Count(s => s == reversedWord);
            var cell = Grid.SelectedRow * Settings.Instance.GridSize + Grid.SelectedColumn;

            if (MultiplayerHandler.Instance.IsNetworkGame)
                MultiplayerHandler.Instance.Chat!.SendNextMoveCom(cell, ChosenButton!.Letter, scoreAdded, word);

            NextRound(
                cell: cell,
                letter: ChosenButton!.Letter,
                scoreAdded: scoreAdded,
                word: word);
        }
        else
            UndoChanges();

        LetterSequence = null;
        Mode = GameMode.LetterChoosing;
    }

    public void NextRound(int cell, char letter, int scoreAdded, string word)
    {
        UsedWords.Add(word);
        if (FirstPlayer.IsMakingMove)
            FirstPlayer.Score += scoreAdded;
        else
            SecondPlayer.Score += scoreAdded;

        var size = Settings.Instance.GridSize;
        Grid.Get(cell / size, cell % size).Letter = letter;
        Grid.Get(cell / size, cell % size).SpawnTime = Round;

        if (Math.Max(FirstPlayer.Score, SecondPlayer.Score) >= Settings.Instance.GoalScore)
        {
            var winnerName =
                FirstPlayer.Score >= Settings.Instance.GoalScore
                ? FirstPlayer.Name : SecondPlayer.Name;
            new WinWindow($"{winnerName} победил!").Show();
            OnClose.Invoke();
            return;
        }

        (FirstPlayer.IsMakingMove, SecondPlayer.IsMakingMove)
            = (SecondPlayer.IsMakingMove, FirstPlayer.IsMakingMove);
        Round++;
        _skipCount = 0;
        MultiplayerHandler.Instance.IsFirstPlayerMakingMove = !MultiplayerHandler.Instance.IsFirstPlayerMakingMove;

        ClearButtonsSelections();
        ClearOldButtons();
        ResetChosenButton();

        LetterSequence = null;
        Mode = GameMode.LetterChoosing;
    }

    public void UndoChanges()
    {
        if (!MultiplayerHandler.Instance.IsCurrentPlayerMakingMove)
            return;

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

    private int _skipCount = 0;
    public void Skip()
    {
        if (!MultiplayerHandler.Instance.IsCurrentPlayerMakingMove)
            return;

        if (MultiplayerHandler.Instance.IsNetworkGame)
            MultiplayerHandler.Instance.Chat!.SendSkipCom();

        SkipChecked();
    }

    public void SkipChecked()
    {
        _skipCount++;
        if (_skipCount == 6)
        {
            new WinWindow("Ничья!").Show();
            OnClose.Invoke();
            return;
        }

        UndoChanges();
        ClearOldButtons();
        Round++;
        (FirstPlayer.IsMakingMove, SecondPlayer.IsMakingMove)
            = (SecondPlayer.IsMakingMove, FirstPlayer.IsMakingMove);
        MultiplayerHandler.Instance.IsFirstPlayerMakingMove = !MultiplayerHandler.Instance.IsFirstPlayerMakingMove;
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