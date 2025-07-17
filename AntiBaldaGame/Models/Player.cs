using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.Models;

public partial class Player : ObservableObject
{
    public required string Name { get; init; }
    [ObservableProperty] private int _score;

    [ObservableProperty] private Brush _color = CustomColors.LightGreen;

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