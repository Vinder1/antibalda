using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.Models;

public class Settings : ObservableObject
{
    public int GridSize { get; set; } = 5;
    public string Name { get; set; } = "Гость";
    
    
    public static Settings Instance { get; private set; } = null!;

    public Settings()
    {
        Instance = this;
    }
}