using TaskManager.CLI.Models;
using TaskStatus = TaskManager.CLI.Models.TaskStatus;

namespace TaskManager.CLI.Helpers;

public static class ConsoleHelper
{
    public static void PrintTask(TaskItem task)
    {
        var statusColor = task.Status switch
        {
            TaskStatus.Done => ConsoleColor.Green,
            TaskStatus.InProgress => ConsoleColor.Yellow,
            TaskStatus.Cancelled => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };

        var priorityColor = task.Priority switch
        {
            Priority.Critical => ConsoleColor.Red,
            Priority.High => ConsoleColor.DarkYellow,
            Priority.Low => ConsoleColor.DarkGray,
            _ => ConsoleColor.Gray
        };

        Console.Write($"[{task.Id}] ");
        Console.ForegroundColor = statusColor;
        Console.Write($"{task.Status,-12}");
        Console.ResetColor();
        Console.Write(" | ");
        Console.ForegroundColor = priorityColor;
        Console.Write($"{task.Priority,-8}");
        Console.ResetColor();
        Console.WriteLine($" | {task.Title}");

        if (!string.IsNullOrWhiteSpace(task.Description))
            Console.WriteLine($"     {task.Description}");
    }

    public static void PrintTable(IEnumerable<TaskItem> tasks)
    {
        Console.WriteLine($"{"ID",-4} {"Status",-12} {"Priority",-8} {"Title"}");
        Console.WriteLine(new string('-', 60));
        foreach (var task in tasks)
            PrintTask(task);
    }

    public static void Success(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✓ {message}");
        Console.ResetColor();
    }

    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"✗ {message}");
        Console.ResetColor();
    }

    // TODO: add Warn() method
}
