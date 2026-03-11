using Microsoft.UI.Xaml;
using AngioPlayer.Services;
using AngioPlayer.ViewModels;
using AngioPlayer.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AngioPlayer;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private Window? _window;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();

        // Загрузка конфигурации
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Настройки сканов
        var scanSettings = configuration.GetSection("ScanSettings").Get<ScanSettings>();

        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton(scanSettings)
                .AddSingleton<IScanService, ScanService>(provider =>
                {
                    var dispatcher = DispatcherQueue.GetForCurrentThread();
                    return new ScanService(scanSettings, dispatcher);
                })
                .AddSingleton<INotificationService, NotificationService>()                
                .AddTransient<PlayerControlViewModel>()
                .BuildServiceProvider()
            );
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
