using System.Timers;
using PomodoroTimer.Core;
using PomodoroTimer.Models;
using PomodoroTimer.UI;

namespace PomodoroTimer.Session;

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
        // Sync state with timer's actual remaining time
        _state.RemainingTime = _timer.RemainingTime;
        
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
        
        // Reset timer with new duration
        var newDuration = TimeSpan.FromMinutes(durationMinutes);
        _timer.Reset(newDuration);

        _state.CurrentPhase = newPhase;
        _state.RemainingTime = newDuration;
        _state.PhaseStartTime = DateTime.UtcNow;

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