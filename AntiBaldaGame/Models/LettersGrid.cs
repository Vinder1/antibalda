using System;
using Avalonia.Controls;
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
                _grid[i,j] = new LetterButton();
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
        //Console.WriteLine("Reset!");
    }
        
    public EventHandler<RoutedEventArgs> SelectButton(int row, int column)
        => (from, e) =>
        {
            SelectedRow = row;
            SelectedColumn = column;
            //Console.WriteLine($"SelectedRow: {SelectedRow}, SelectedColumn: {SelectedColumn}");
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