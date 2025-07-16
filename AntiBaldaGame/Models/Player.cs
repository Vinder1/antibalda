using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.Models;

public partial class Player : ObservableObject
{
    public string Name { get; init; }
    [ObservableProperty] private int _score;
}