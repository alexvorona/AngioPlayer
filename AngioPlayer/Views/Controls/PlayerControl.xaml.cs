using System;
using AngioPlayer.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace AngioPlayer.Views.Controls;

public sealed partial class PlayerControl : UserControl
{    
    public PlayerControlViewModel ViewModel { get; }

    public PlayerControl()
    {
        this.InitializeComponent();

        ViewModel = Ioc.Default.GetRequiredService<PlayerControlViewModel>();
        ViewModel.PlaneAImage = new BitmapImage(new Uri("ms-appx:///Assets/Logo.png"));
        ViewModel.PlaneBImage = ViewModel.PlaneAImage;
        this.DataContext = ViewModel;
    }

}