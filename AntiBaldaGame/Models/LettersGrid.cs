using System;
using AntiBaldaGame.ViewModels;
using Avalonia.Interactivity;

namespace AntiBaldaGame.Models;

public class LettersGrid
{
    private readonly LetterButton[,] _grid;

    public LettersGrid()
    {
        var gridSize = Settings.Instance.GridSize;
        //Console.WriteLine($"Grid size: {gridSize}");
        _grid = new LetterButton[gridSize, gridSize];
        for (var i = 0; i < gridSize; i++)
        {
            for (var j = 0; j < gridSize; j++)
            {
                _grid[i, j] = new LetterButton();
            }
        }
    }

    public LetterButton Get(int row, int column) => _grid[row, column];

    public int SelectedRow { get; private set; }
    public int SelectedColumn { get; private set; }

    public void ResetSelectedButton()
    {
        SelectedRow = -1;
        SelectedColumn = -1;
    }

    public EventHandler<RoutedEventArgs> SelectButton(int row, int column, GameWindowViewModel vm)
        => (from, e) =>
        {
            if (vm.Mode == GameWindowViewModel.GameMode.LetterChoosing)
            {
                if (SelectedRow != -1 && SelectedColumn != -1)
                {
                    Get(SelectedRow, SelectedColumn).IsSelected = false;
                    SelectedRow = SelectedColumn = -1;
                }

                if (Get(row, column).Letter is not ' ' or '\0')
                    return;
                Get(row, column).IsSelected = true;
                SelectedRow = row;
                SelectedColumn = column;
            }
            else
            {
                var button = Get(row, column);

                if (button.Letter is ' ' or '\0')
                    return;

                var cb = new CoordinatedLetterButton(button, row, column);

                if (button.IsSelected)
                {
                    if (button.Color == CustomColors.White)
                        return;
                    if (vm.LetterSequence!.TryRemoveFirstOrLast(cb))
                    {
                        button.IsSelected = false;
                    }
                    return;
                }        

                if (vm.LetterSequence!.TryAdd(cb))
                {
                    button.IsSelected = true;
                }
            }
        };

    // public int Sum()
    // {
    //     var sum = 0;
    //     for (var i = 0; i < _grid.GetLength(0); i++)
    //     {
    //         for (var j = 0; j < _grid.GetLength(1); j++)
    //         {
    //             sum += _grid[i,j].Letter == ' ' ? 0 : 1;
    //         }
    //     }
    //     return sum;
    // }
}