using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace AngioPlayer.ViewModels;

public partial class PlayerControlViewModel : ObservableObject
{
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

    public ObservableCollection<string> Scans { get; } = new() { "Scan 1", "Scan 2" };
    public ObservableCollection<string> Speeds { get; } = new() { "Slow", "Normal", "Fast" };

    private List<string> _planeA = new();
    private List<string> _planeB = new();
    private int _imageCount;

    public PlayerControlViewModel()
    {
        _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(200);
        _timer.Tick += TimerTick;

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
        // Пример загрузки SVG (PlaneA и PlaneB)
        _planeA = Enumerable.Range(1, 30)
            .Select(i => $"ms-appx:///Assets/PlaneA/{i}.svg")
            .ToList();

        _planeB = Enumerable.Range(1, 30)
            .Select(i => $"ms-appx:///Assets/PlaneB/{i}.svg")
            .ToList();

        _imageCount = _planeA.Count;
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
        SetFrame(CurrentFrame + 1);
    }

    [RelayCommand]
    private void Prev()
    {
        SetFrame(CurrentFrame - 1);
    }

    [RelayCommand]
    private void KeyImage()
    {
        SetFrame(_imageCount / 2);
    }

    partial void OnCurrentFrameChanged(int value)
    {
        SetFrame(value);
    }

    private void SetFrame(int frame)
    {
        if (_imageCount == 0)
            return;

        frame = (frame + _imageCount) % _imageCount;
        CurrentFrame = frame;

        // Загружаем SVG через SvgImageSource
        PlaneAImage = new SvgImageSource(new Uri(_planeA[frame]));
        PlaneBImage = new SvgImageSource(new Uri(_planeB[frame]));
    }
}