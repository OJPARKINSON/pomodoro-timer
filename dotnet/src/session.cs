using System.Timers;
using dotnet.PomodoroTimer;

namespace dotnet.session;

public enum SessionPhase
{
    Focus,
    ShortBreak,
    LongBreak,
    Completed
}

public class SessionState
{
    public SessionPhase CurrentPhase { get; set; }
    public int CompletedCycles { get; set; }
    public TimeSpan RemainingTime { get; set; }
    public DateTime PhaseStartTime { get; set; }
}

public class PomodoroSession
{
    private readonly IPomodoroTimer _timer;
    private readonly IUserInterface _ui;
    private readonly SessionConfiguration _config;
    private SessionState _state;

    public SessionState CurrentState => _state;

    public PomodoroSession(IPomodoroTimer timer, IUserInterface ui, SessionConfiguration config)
    {
        _timer = timer ?? throw new ArgumentNullException(nameof(timer));
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        _config = config ?? throw new ArgumentNullException(nameof(config));


        InitialiseSession();
        _timer.TimerElapsed += OnTimerTick;
    }

    private void InitialiseSession()
    {
        _state = new SessionState
        {
            CurrentPhase = SessionPhase.Focus,
            CompletedCycles = 0,
            RemainingTime = TimeSpan.FromMinutes(_config.FocusTime),
            PhaseStartTime = DateTime.UtcNow
        };
    }

    private void OnTimerTick(object sender, ElapsedEventArgs e)
    {
        _state.RemainingTime -= TimeSpan.FromSeconds(1);
        _ui.DisplayTimer(_state.RemainingTime);
        _ui.DisplayPhase(_state.CurrentPhase);

        if (_state.RemainingTime <= TimeSpan.Zero)
        {
            CompleteCurrentPhase();
        }
    }

    private void CompleteCurrentPhase()
    {
        _ui.ShowCompletionMessage();

        switch (_state.CurrentPhase)
        {
            case SessionPhase.Focus:
                _state.CompletedCycles++;
                if (_state.CompletedCycles % 4 == 0)
                {
                    StartLongBreak();
                }
                else
                {
                    StartShortBreak();
                }
                break;
            case SessionPhase.ShortBreak:
            case SessionPhase.LongBreak:
                StartFocusSession();
                break;
        }
    }

    private void StartFocusSession()
    {
        TransitionToPhase(SessionPhase.Focus, _config.FocusTime);
    }

    private void StartShortBreak()
    {
        TransitionToPhase(SessionPhase.ShortBreak, _config.ShortBreakTime);
    }
    
    private void StartLongBreak()
    {
        TransitionToPhase(SessionPhase.LongBreak, _config.LongBreakTime);
    }

    private void TransitionToPhase(SessionPhase newPhase, int durationMinutes)
    {
        _timer.Stop();
        _timer.Reset();

        _state.CurrentPhase = newPhase;
        _state.RemainingTime = TimeSpan.FromMinutes(durationMinutes);
        _state.PhaseStartTime = DateTime.UtcNow;

        // Note: Creating a new timer here is problematic - need to reuse existing timer

        _ui.DisplayPhase(newPhase);
        _timer.Start();
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();
    public void Reset()
    {
        _timer.Reset();
        InitialiseSession();
    }
}

public class SessionConfiguration
{
    public int FocusTime { get; set; } = 25;
    public int ShortBreakTime { get; set; } = 5;
    public int LongBreakTime { get; set; } = 15;
    public int CyclesBeforeLongBreak { get; set; } = 4;

    public static SessionConfiguration Default => new();

    public static SessionConfiguration LoadFromFile(string path)
    {
        throw new NotImplementedException();
    }
}

public interface IUserInterface
{
    void DisplayTimer(TimeSpan remaining);
    void DisplayPhase(SessionPhase phase);
    void ShowCompletionMessage();
    void DisplaySessionStats(int completedCycles, TimeSpan totalFocusTime);
}

public class ConsoleUserInterface : IUserInterface
{
    private const int TIMER_ROW = 2;
    private const int PHASE_ROW = 1;
    private const int STATS_ROW = 4;

    public void DisplayTimer(TimeSpan remaining)
    {
        SetCursorAndClear(0, TIMER_ROW);

        var minutes = (int)remaining.TotalMinutes;
        var seconds = remaining.Seconds;

        // Color coding based on time remaining
        var timeColor = remaining.TotalMinutes switch
        {
            > 5 => ConsoleColor.Green,
            > 2 => ConsoleColor.Yellow,
            _ => ConsoleColor.Red
        };

        Console.ForegroundColor = timeColor;
        Console.Write($"⏱️  {minutes:00}:{seconds:00}");
        Console.ResetColor();

        // Progress bar
        var totalSeconds = 25 * 60; // Assume 25 min for now - should be configurable
        var elapsedSeconds = totalSeconds - (int)remaining.TotalSeconds;
        var progressPercent = (double)elapsedSeconds / totalSeconds;
        DrawProgressBar(progressPercent, 30);
    }

    public void DisplayPhase(SessionPhase phase)
    {
        SetCursorAndClear(0, PHASE_ROW);

        var (emoji, text, color) = phase switch
        {
            SessionPhase.Focus => ("🍅", "FOCUS TIME", ConsoleColor.Red),
            SessionPhase.ShortBreak => ("☕", "SHORT BREAK", ConsoleColor.Blue),
            SessionPhase.LongBreak => ("🌟", "LONG BREAK", ConsoleColor.Magenta),
            _ => ("❓", "UNKNOWN", ConsoleColor.Gray)
        };

        Console.ForegroundColor = color;
        Console.Write($"{emoji} {text}");
        Console.ResetColor();
    }

    public void ShowCompletionMessage()
    {
        Console.SetCursorPosition(0, 6);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("⏰ Phase Complete!");
        Console.ResetColor();

        // Multiple beeps for attention
        for (int i = 0; i < 3; i++)
        {
            Console.Beep(800, 200);
            Thread.Sleep(100);
        }

        Console.WriteLine("Press SPACE to continue, Q to quit...");

        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
        } while (key.Key != ConsoleKey.Spacebar && key.Key != ConsoleKey.Q);

        if (key.Key == ConsoleKey.Q)
        {
            throw new OperationCanceledException("User requested exit");
        }

        ClearCompletionMessage();
    }

    public void DisplaySessionStats(int completedCycles, TimeSpan totalFocusTime)
    {
        SetCursorAndClear(0, STATS_ROW);
        Console.WriteLine($"Completed Cycles: {completedCycles} | Total Focus: {totalFocusTime:hh\\:mm\\:ss}");
    }

    private void SetCursorAndClear(int left, int top)
    {
        if (top >= 0 && top < Console.WindowHeight && left >= 0)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(new string(' ', Math.Max(0, Console.WindowWidth - 1)));
            Console.SetCursorPosition(left, top);
        }
    }

    private void DrawProgressBar(double percent, int width)
    {
        Console.SetCursorPosition(0, 3);
        Console.Write("[");

        var filled = (int)(percent * width);
        for (int i = 0; i < width; i++)
        {
            if (i < filled)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("█");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("░");
            }
        }

        Console.ResetColor();
        Console.Write($"] {percent:P0}");
    }

    private void ClearCompletionMessage()
    {
        for (int i = 6; i <= 8; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write(new string(' ', Console.WindowWidth - 1));
        }
    }
}