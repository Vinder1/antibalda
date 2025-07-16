using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.Models;

public partial class LetterButton : ObservableObject
{
    [ObservableProperty] private char _letter = ' ';
}