using System.Linq;
using System.Net;
using System.Net.Sockets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.Models;

public partial class Settings : ObservableObject
{
    public int GridSize { get; set; } = 5;
    public string Name { get; set; } = "Гость";
    public int TimeOut { get; set; } = 3;
    public bool GameRunning { get; set; } = false;
    public string StartWord { get; set; } = "";
    [ObservableProperty] private string otherPlayerName = "Бибурат";
    
    public static Settings Instance { get; private set; } = null!;
    public static void ResetInstance() => Instance = new();

    public Settings()
    {
        Instance = this;
    }
}