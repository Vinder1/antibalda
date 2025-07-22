using System;
using AntiBaldaGame.Models;
using AntiBaldaGame.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AntiBaldaGame.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        new GameWindow().Show();
        Close();
    }

    private void NetworkSwitch(object? sender, RoutedEventArgs e)
    {
        Settings.Instance.IsNetworkGame = !Settings.Instance.IsNetworkGame;
        if (Settings.Instance.IsNetworkGame)
        {
            Settings.Instance.MultiplayerHandler = new();
        }
        else
        {
            Settings.Instance.MultiplayerHandler = null;
        }
    }

    private void Connect(object? sender, RoutedEventArgs e)
    {
        try
        {
            Settings.Instance.MultiplayerHandler!.Chat.StartServer();
            Settings.Instance.MultiplayerHandler!.Chat
                .SendMessageFromSocket(Settings.Instance.SendingIp, Settings.Instance.SendingPort);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);   
        }
    }
}