using CommunityToolkit.Mvvm.ComponentModel;

namespace AntiBaldaGame.Models;

public partial class MultiplayerHandler : ObservableObject
{
    [ObservableProperty] public bool isNetworkGame = false;
    public int ListeningPort { get; set; } = 5000;
    public string SendingIp { get; set; } = LocalNetworkChat.GetLocalIPv4Addresses()[0];
    public int SendingPort { get; set; } = 5001;

    [ObservableProperty] private bool connected = false;
    [ObservableProperty] private string incomeText = " ";
    [ObservableProperty] private string sendingText = " ";

    public LocalNetworkChat? Chat { get; set; }
    public bool IsFirstPlayer = true;

    public bool IsFirstPlayerMakingMove = false;
    public bool IsCurrentPlayerMakingMove => !IsNetworkGame || IsFirstPlayerMakingMove == IsFirstPlayer;

    public static MultiplayerHandler Instance { get; private set; } = null!;
    
    public MultiplayerHandler()
    {
        Instance = this;
    }
}
