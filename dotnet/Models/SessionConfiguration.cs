namespace PomodoroTimer.Models;

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