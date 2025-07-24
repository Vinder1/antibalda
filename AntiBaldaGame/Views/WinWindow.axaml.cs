using AntiBaldaGame.ViewModels;
using Avalonia.Controls;

namespace AntiBaldaGame.Views;

public partial class WinWindow : Window
{
    public WinWindow(string message)
    {
        InitializeComponent();
        DataContext = new WinWindowViewModel(message);
    }
}