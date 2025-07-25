using System;
using AntiBaldaGame.Models;
using AntiBaldaGame.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

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
        Settings.Instance.GameRunning = true;
        Settings.Instance.StartWord = OfflineDictionary.GetRandom;
        var mh = MultiplayerHandler.Instance;
        if (mh.IsNetworkGame)
        {
            mh.IsFirstPlayer = true;
            mh.Chat!.SendStartGameCom();
        }
        else
        {
            StartGame();
        }
        //StartGame();
    }

    private void StartGame()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            Settings.Instance.GameRunning = true;
            new GameWindow().Show();
            Close();
        });
    }

    private void NetworkSwitch(object? sender, RoutedEventArgs e)
    {
        MultiplayerHandler.Instance.IsNetworkGame = !MultiplayerHandler.Instance.IsNetworkGame;
    }

    private void Connect(object? sender, RoutedEventArgs e)
    {
        try
        {
            var mp = MultiplayerHandler.Instance;
            mp.Chat = new(mp.ListeningPort);
            mp.Chat.OnGameStart += StartGame;
            mp.Chat.StartServer();
            mp.Chat.SendMessageFromSocket(mp.SendingIp, mp.SendingPort);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);   
        }
    }
}