using TaskManager.CLI.Helpers;
using TaskManager.CLI.Services;
using TaskManager.CLI.Models;
using TaskStatus = TaskManager.CLI.Models.TaskStatus;

var storage = new StorageService();
var service = new TaskService(storage);

if (args.Length == 0)
{
    PrintHelp();
    return;
}

switch (args[0].ToLower())
{
    case "list":
        var filter = args.Length > 1 ? args[1] : null;
        var tasks = filter != null && Enum.TryParse<TaskStatus>(filter, true, out var status)
            ? service.GetByStatus(status)
            : service.GetAll();
        ConsoleHelper.PrintTable(tasks);
        break;

    case "add":
        if (args.Length < 2) { ConsoleHelper.Error("Usage: add <title> [description] [priority]"); break; }
        var title = args[1];
        var desc = args.Length > 2 ? args[2] : string.Empty;
        var priority = args.Length > 3 && Enum.TryParse<Priority>(args[3], true, out var p) ? p : Priority.Medium;
        var newTask = service.Add(title, desc, priority);
        ConsoleHelper.Success($"Created task #{newTask.Id}: {newTask.Title}");
        break;

    case "done":
        if (args.Length < 2 || !int.TryParse(args[1], out var doneId))
        { ConsoleHelper.Error("Usage: done <id>"); break; }
        ConsoleHelper.Success(service.UpdateStatus(doneId, TaskStatus.Done)
            ? $"Task #{doneId} marked as done."
            : $"Task #{doneId} not found.");
        break;

    case "start":
        if (args.Length < 2 || !int.TryParse(args[1], out var startId))
        { ConsoleHelper.Error("Usage: start <id>"); break; }
        ConsoleHelper.Success(service.UpdateStatus(startId, TaskStatus.InProgress)
            ? $"Task #{startId} started."
            : $"Task #{startId} not found.");
        break;

    case "delete":
        if (args.Length < 2 || !int.TryParse(args[1], out var delId))
        { ConsoleHelper.Error("Usage: delete <id>"); break; }
        ConsoleHelper.Success(service.Delete(delId)
            ? $"Task #{delId} deleted."
            : $"Task #{delId} not found.");
        break;

    case "search":
        if (args.Length < 2) { ConsoleHelper.Error("Usage: search <query>"); break; }
        ConsoleHelper.PrintTable(service.Search(args[1]));
        break;

    case "stats":
        var statusStats = service.GetStatusStats();
        var priorityStats = service.GetPriorityStats();
        Console.WriteLine("== Status ==");
        foreach (var kv in statusStats)
            Console.WriteLine($"  {kv.Key,-12}: {kv.Value}");
        Console.WriteLine("== Priority ==");
        foreach (var kv in priorityStats)
            Console.WriteLine($"  {kv.Key,-12}: {kv.Value}");
        break;

    default:
        ConsoleHelper.Error($"Unknown command: {args[0]}");
        PrintHelp();
        break;
}

static void PrintHelp()
{
    Console.WriteLine("TaskManager CLI");
    Console.WriteLine("  list [status]           List tasks (optionally filter by status)");
    Console.WriteLine("  add <title> [desc] [p]  Add a new task");
    Console.WriteLine("  start <id>              Mark task as in-progress");
    Console.WriteLine("  done <id>               Mark task as done");
    Console.WriteLine("  delete <id>             Delete a task");
    Console.WriteLine("  search <query>          Search tasks by title");
    Console.WriteLine("  stats                   Show task statistics");
    // TODO: add export command (CSV/markdown)
    // TODO: add import command
}
