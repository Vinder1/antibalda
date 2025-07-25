namespace AntiBaldaGame.ViewModels;

public partial class WinWindowViewModel(string message) : ViewModelBase
{
    public string Message => message;
}