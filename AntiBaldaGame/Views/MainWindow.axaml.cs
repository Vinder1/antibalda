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
}