namespace PomodoroTimer.Models;

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