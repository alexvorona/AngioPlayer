using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace AngioPlayer.ViewModels;

public partial class PlayerControlViewModel : ObservableObject
{
    private const string ScansPath = @"C:\Temp\Scans"; // ToDo:should be in config
    [ObservableProperty]
    private string selectedScan;

    [ObservableProperty]
    private string selectedSpeed = "Normal";

    [ObservableProperty]
    private bool isPlaying;

    [ObservableProperty]
    private int currentFrame;

    [ObservableProperty]
    private ImageSource planeAImage;

    [ObservableProperty]
    private ImageSource planeBImage;

    [ObservableProperty]
    private int sliderMax;

    private readonly DispatcherQueueTimer _timer;

    private readonly ScanService _scanService;

    public ObservableCollection<string> Scans => _scanService.Scans;

    public ObservableCollection<string> Speeds { get; } = new() { "Slow", "Normal", "Fast" };

    private List<string> _planeA = new();
    private List<string> _planeB = new();
    private int _imageCount;

    public PlayerControlViewModel()
    {
        _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(200);
        _timer.Tick += TimerTick;

        // Создаём сервис сканов, указываем путь к папке и UI-диспетчер
        _scanService = new ScanService(@"C:\Temp\Scans", DispatcherQueue.GetForCurrentThread());

        // Сразу показываем Logo.png при старте
        var logoUri = new Uri("ms-appx:///Assets/Logo.png");
        var logoImage = new BitmapImage(logoUri);
        PlaneAImage = logoImage;
        PlaneBImage = logoImage;
    }

    private void TimerTick(object sender, object e)
    {
        SetFrame(CurrentFrame + 1);
    }

    partial void OnSelectedSpeedChanged(string value)
    {
        _timer.Interval = value switch
        {
            "Slow" => TimeSpan.FromMilliseconds(400),
            "Fast" => TimeSpan.FromMilliseconds(100),
            _ => TimeSpan.FromMilliseconds(200)
        };
    }

    [RelayCommand]
    private void Load()
    {
        if (string.IsNullOrEmpty(SelectedScan))
            return;

        string basePath = Path.Combine(ScansPath, SelectedScan);

        _planeA = Directory.GetFiles(Path.Combine(basePath, "Plane-A"), "*.png")
                           .OrderBy(f => f)
                           .ToList();

        _planeB = Directory.GetFiles(Path.Combine(basePath, "Plane-B"), "*.png")
                           .OrderBy(f => f)
                           .ToList();

        _imageCount = Math.Min(_planeA.Count, _planeB.Count);
        SliderMax = _imageCount - 1;

        // Сразу показать середину
        SetFrame(_imageCount / 2);
    }

    [RelayCommand]
    private void Play()
    {
        _timer.Start();
        IsPlaying = true;
    }

    [RelayCommand]
    private void Pause()
    {
        _timer.Stop();
        IsPlaying = false;
    }

    [RelayCommand]
    private void Next()
    {
        CurrentFrame += 1;        
    }

    [RelayCommand]
    private void Prev()
    {
        CurrentFrame -= 1;
    }

    [RelayCommand]
    private void KeyImage()
    {
        CurrentFrame = (_imageCount / 2);
    }

    partial void OnCurrentFrameChanged(int value)
    {
        SetFrame(value);
    }

    private void SetFrame(int frame)
    {
        if (_imageCount == 0)
            return;

        PlaneAImage = new BitmapImage(new Uri(_planeA[frame]));
        PlaneBImage = new BitmapImage(new Uri(_planeB[frame]));
    }
}