using System;
using Microsoft.UI.Dispatching;

public class DispatcherTimerService : ITimerService
{
    private readonly DispatcherQueueTimer _timer;

    public DispatcherTimerService()
    {
        _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _timer.Tick += (s, e) => Tick?.Invoke(this, EventArgs.Empty);
    }

    public bool IsRunning => _timer.IsRunning;

    public TimeSpan Interval
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();
    public event EventHandler Tick;
}