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

    [ObservableProperty] public bool isNetworkGame = false;
    public int ListeningPort { get; set; } = 5000;
    public string SendingIp { get; set; } = LocalNetworkChat.GetLocalIPv4Addresses()[0];
    public int SendingPort { get; set; } = 5001;

    public MultiplayerHandler? MultiplayerHandler { get; set; } = null;
    public bool Connected { get; set; } = false;
    
    public static Settings Instance { get; private set; } = null!;

    public Settings()
    {
        Instance = this;
    }
}