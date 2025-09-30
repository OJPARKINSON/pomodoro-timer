using PomodoroTimer.Models;
using PomodoroTimer.Session;
using PomodoroTimer.UI;

Console.Clear();
Console.CursorVisible = false;

try
{
    Console.WriteLine("🍅 Pomodoro Timer");
    Console.WriteLine("Press ENTER to start, Q to quit");

    var key = Console.ReadKey(true);
    if (key.Key == ConsoleKey.Q) return;

    var config = SessionConfiguration.Default;
    var ui = new ConsoleUserInterface();
    var timer = new PomodoroTimer.Core.Timer(TimeSpan.FromMinutes(config.FocusTime));
    var session = new PomodoroSession(timer, ui, config);

    Console.Clear();
    session.Start();

    // Keep the application running
    Console.WriteLine("\nPress Q to quit, P to pause/resume...");
    while (true)
    {
        var inputKey = Console.ReadKey(true);
        switch (inputKey.Key)
        {
            case ConsoleKey.Q:
                session.Stop();
                Console.WriteLine("\nGoodbye! 🍅");
                return;
            case ConsoleKey.P:
                if (timer.IsRunning)
                {
                    session.Stop();
                    Console.WriteLine("\nPaused. Press P to resume...");
                }
                else
                {
                    session.Start();
                    Console.WriteLine("\nResumed!");
                }
                break;
            case ConsoleKey.R:
                session.Reset();
                Console.WriteLine("\nTimer reset!");
                break;
        }
    }
}
finally
{
    Console.CursorVisible = true;
    Console.Clear();
}
