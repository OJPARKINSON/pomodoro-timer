using PomodoroTimer.Models;

namespace PomodoroTimer.UI;

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