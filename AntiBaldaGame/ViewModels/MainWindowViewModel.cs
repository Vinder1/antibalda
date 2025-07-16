using AntiBaldaGame.Models;

namespace AntiBaldaGame.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public Settings Settings { get; } = new Settings();
}