using AngioPlayer.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace AngioPlayer.Views.Controls;

public sealed partial class PlayerControl : UserControl
{    
    public PlayerControlViewModel ViewModel { get; } = new PlayerControlViewModel();

    public PlayerControl()
    {
        this.InitializeComponent();
        this.DataContext = ViewModel;
    }
}