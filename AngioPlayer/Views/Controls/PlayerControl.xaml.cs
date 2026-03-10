using AngioPlayer.ViewModels;
using Microsoft.UI.Xaml.Controls;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AngioPlayer.Views.Controls;

public sealed partial class PlayerControl : UserControl
{
    public PlayerControlViewModel ViewModel { get; }
    public PlayerControl()
    {
        InitializeComponent();
        ViewModel = new PlayerControlViewModel();
        this.DataContext = ViewModel;
    }
}
