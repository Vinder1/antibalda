using System.Collections.ObjectModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.Models;

public partial class Player : ObservableObject
{
    public required string Name { get; init; }
    [ObservableProperty] private int _score;

    [ObservableProperty] private Brush _color = CustomColors.LightGreen;

    public ObservableCollection<string> UsedWords { get; } = [];

    private bool isMakingMove;
    public bool IsMakingMove
    {
        get => isMakingMove;
        set
        {
            isMakingMove = value;
            Color = isMakingMove ? CustomColors.Yellow : CustomColors.LightGreen;
        }
    }
}