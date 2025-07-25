using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.Models;

public partial class LetterButton : ObservableObject
{
    [ObservableProperty] private char _letter = ' ';
    [ObservableProperty] private Brush _color = CustomColors.DarkGreen;
    [ObservableProperty] private Brush _textColor = CustomColors.White;

    public int SpawnTime { get; set; } = -1;

    private bool isSelected;
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            Color = isSelected ? CustomColors.Yellow : CustomColors.DarkGreen;
            TextColor = isSelected ? CustomColors.Black : CustomColors.White;
        }
    }
}