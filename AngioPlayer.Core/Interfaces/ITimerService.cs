using System;

public interface ITimerService
{
    bool IsRunning { get; }
    TimeSpan Interval { get; set; }

    void Start();
    void Stop();

    event EventHandler Tick;
}