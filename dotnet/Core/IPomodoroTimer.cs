using System.Timers;

namespace PomodoroTimer.Core;

public interface IPomodoroTimer : IDisposable
{
    event EventHandler<ElapsedEventArgs>? TimerElapsed;
    void Start();
    Task StartAsync();
    void Stop();
    void Reset();
    void Reset(TimeSpan newDuration);
    bool IsRunning { get; }
    TimeSpan RemainingTime { get; }
}