using AngioPlayer.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace AngioPlayer.Views.Controls;

public sealed partial class PlayerControl : UserControl
{    
    public PlayerControlViewModel ViewModel { get; }

    public PlayerControl()
    {
        this.InitializeComponent();

        ViewModel = Ioc.Default.GetRequiredService<PlayerControlViewModel>();
        this.DataContext = ViewModel;
    }

}