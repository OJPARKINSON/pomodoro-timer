using System.Timers;

namespace dotnet.PomodoroTimer;

public class PomodoroTimer : IPomodoroTimer
{
    private readonly System.Timers.Timer _internalTimer;
    private readonly TimeSpan _duration;
    private TimeSpan _remaining;
    private bool _isRunning;
    private bool _disposed;

    public event EventHandler<ElapsedEventArgs>? TimerElapsed;
    public bool IsRunning => _isRunning;

    public PomodoroTimer(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(duration), "Duration must be positive");
            
        _duration = duration;
        _remaining = duration;
        _internalTimer = new System.Timers.Timer(1000);
        _internalTimer.Elapsed += OnInternalTick;
    }

    private void OnInternalTick(object sender, ElapsedEventArgs e)
    {
        if (_isRunning)
        {
            _remaining -= TimeSpan.FromSeconds(1);
            TimerElapsed?.Invoke(this, e);

            if (_remaining <= TimeSpan.Zero)
            {
                Stop();
            }
        }
    }

    public void Start()
    {
        if (!_isRunning)
        {
            _isRunning = true;
            _internalTimer.Start();
        }
    }
    public async Task StartAsync()
    {
        Start();

        while (_isRunning && _remaining > TimeSpan.Zero)
        {
            await Task.Delay(100);
        }
    }
    public void Stop()
    {
        _isRunning = false;
        _internalTimer.Stop();
    }
    public void Reset()
    {
        Stop();
        _remaining = _duration;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _internalTimer?.Stop();
            _internalTimer?.Dispose();
            _disposed = true;
        }
    }
}

public interface IPomodoroTimer : IDisposable
{
    event EventHandler<ElapsedEventArgs>? TimerElapsed;
    void Start();
    Task StartAsync();
    void Stop();
    void Reset();
    bool IsRunning { get; }
}