using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using AngioPlayer.Services;
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
    private const string SearchPattern = "*.png";

    [ObservableProperty]
    private string selectedScan;

    [ObservableProperty]
    private string selectedSpeed = "Normal";

    [ObservableProperty] private bool isPrevEnabled;
    [ObservableProperty] private bool isNextEnabled;
    [ObservableProperty] private bool isPlayEnabled;
    [ObservableProperty] private bool isPauseEnabled;
    [ObservableProperty] private bool isSpeedEnabled;
    [ObservableProperty] private bool isKeyEnabled;

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

    private readonly IScanService _scanService;
    private readonly INotificationService _notificationsService;

    public ObservableCollection<string> Scans => _scanService.Scans;

    public ObservableCollection<string> Speeds { get; } = new() { "Slow", "Normal", "Fast" };

    private List<string> _planeAFilenames = new();
    private List<string> _planeBFilenames = new();
    private List<ImageSource> _planeABitmaps = new();
    private List<ImageSource> _planeBBitmaps = new();
    private int _imageCount;

    public PlayerControlViewModel(IScanService scanService, INotificationService notificationsService)
    {
        _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(200);
        _timer.Tick += TimerTick;

        _scanService = scanService;
        _notificationsService = notificationsService;

        var logoUri = new Uri("ms-appx:///Assets/Logo.png");
        var logoImage = new BitmapImage(logoUri);
        PlaneAImage = logoImage;
        PlaneBImage = logoImage;
    }
    
    private void TimerTick(object sender, object e)
    {
        CurrentFrame = (CurrentFrame + 1) > SliderMax ? 0 : CurrentFrame + 1;
    }

    partial void OnSelectedSpeedChanged(string value)
    {
        bool wasRunning = _timer.IsRunning;

        if (wasRunning)
            _timer.Stop();

        _timer.Interval = value switch
        {
            "Slow" => TimeSpan.FromMilliseconds(400),
            "Normal" => TimeSpan.FromMilliseconds(200),
            _ => TimeSpan.FromMilliseconds(100)
        };

        if (wasRunning)
            _timer.Start();
    }

    [RelayCommand]
    private void Load()
    {
        Pause();

        if (string.IsNullOrEmpty(SelectedScan))
            return;

        string basePath = Path.Combine(ScansPath, SelectedScan);
        string planeAPath = Path.Combine(basePath, "Plane-A");
        string planeBPath = Path.Combine(basePath, "Plane-B");

        if (!Directory.Exists(planeAPath) || !Directory.Exists(planeBPath))
        {

            _notificationsService.ShowError("Scan folder must contain Plane-A and Plane-B directories");
            return;
        }
        _planeAFilenames = Directory.GetFiles(planeAPath, SearchPattern)
                           .OrderBy(f =>
                           {
                               var name = Path.GetFileNameWithoutExtension(f);
                               var parts = name.Split('_');
                               if (int.TryParse(parts[^1], out int index))
                                   return index;
                               return int.MaxValue;
                           })
                           .ToList();

        _planeBFilenames = Directory.GetFiles(planeBPath, SearchPattern)
                           .OrderBy(f =>
                           {
                               var name = Path.GetFileNameWithoutExtension(f);
                               var parts = name.Split('_');
                               if (int.TryParse(parts[^1], out int index))
                                   return index;
                               return int.MaxValue;
                           })
                           .ToList();

        _imageCount = Math.Min(_planeAFilenames.Count, _planeBFilenames.Count);
        SliderMax = _imageCount - 1;

        _planeABitmaps = _planeAFilenames.Select(path =>
        {
            var bitmap = new WriteableBitmap(1, 1);
            using var stream = File.OpenRead(path);
            bitmap.SetSource(stream.AsRandomAccessStream());
            return (ImageSource)bitmap;
        }).ToList();

        _planeBBitmaps = _planeBFilenames.Select(path =>
        {
            var bitmap = new WriteableBitmap(1, 1);
            using var stream = File.OpenRead(path);
            bitmap.SetSource(stream.AsRandomAccessStream());
            return (ImageSource)bitmap;
        }).ToList();

        CurrentFrame = _imageCount / 2;

        EnablePlay();
    }

    [RelayCommand]
    private void Play()
    {
        _timer.Start();
        DisablePlay();
    }

    private void DisablePlay()
    {
        IsPlaying = true;

        IsPlayEnabled = false;
        IsPauseEnabled = true;

        IsSpeedEnabled = true;
        IsKeyEnabled = true;

        IsPrevEnabled = false;
        IsNextEnabled = false;
    }

    [RelayCommand]
    private void Pause()
    {
        _timer.Stop();
        EnablePlay();
    }

    private void EnablePlay()
    {
        IsPlaying = false;

        IsPlayEnabled = true;
        IsPauseEnabled = false;

        IsSpeedEnabled = false;
        IsKeyEnabled = true;

        IsPrevEnabled = true;
        IsNextEnabled = true;
    }

    [RelayCommand]
    private void Next()
    {
        CurrentFrame = (CurrentFrame + 1) > SliderMax ? 0 : CurrentFrame + 1;
    }

    [RelayCommand]
    private void Prev()
    {
        CurrentFrame -= 1;
    }

    [RelayCommand]
    private void KeyImage()
    {
        Pause();
        CurrentFrame = _imageCount / 2;
    }

    partial void OnCurrentFrameChanged(int value)
    {
        SetFrame(value);
    }

    private void SetFrame(int frame)
    {
        if (_imageCount == 0)
            return;
                
        frame = Math.Clamp(frame, 0, _imageCount - 1);

        PlaneAImage = _planeABitmaps[frame];
        PlaneBImage = _planeBBitmaps[frame];
    }
}