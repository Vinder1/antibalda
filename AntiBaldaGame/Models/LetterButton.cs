using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.Models;

public partial class LetterButton : ObservableObject
{
    [ObservableProperty] public char letter = ' ';
}