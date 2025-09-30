using PomodoroTimer.Models;

namespace PomodoroTimer.UI;

public interface IUserInterface
{
    void DisplayTimer(TimeSpan remaining);
    void DisplayPhase(SessionPhase phase);
    void ShowCompletionMessage();
    void DisplaySessionStats(int completedCycles, TimeSpan totalFocusTime);
}