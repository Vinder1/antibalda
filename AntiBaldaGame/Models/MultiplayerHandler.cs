namespace AntiBaldaGame.Models;

public class MultiplayerHandler
{
    public LocalNetworkChat Chat { get; } = new(Settings.Instance.ListeningPort);
    public bool IsFirstPlayer;
}
