#!/usr/bin/env dotnet-script

var logPath = @"C:\Projects\ClaudeRoutine\logs\app.log";
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

var rng = new Random();
var requestIds = new[] { "req-a1b2", "req-c3d4", "req-e5f6", "req-g7h8" };
var users = new[] { "user:42", "user:17", "user:99" };
var routes = new[] { "/api/tasks", "/api/tasks/search", "/api/health", "/api/stats" };

string Timestamp() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

void Log(string level, string message) {
    var line = $"[{Timestamp()}] [{level,-8}] {message}";
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

Console.WriteLine($"Writing logs to: {logPath}");
Console.WriteLine("Press Ctrl+C to stop.\n");

var tick = 0;
while (true)
{
    tick++;

    // health check — every 5 ticks (ignored by monitor)
    if (tick % 5 == 0)
        Log("INFO", "GET /api/health 200 OK (2ms)");

    // normal request
    var route = routes[rng.Next(routes.Length)];
    var user = users[rng.Next(users.Length)];
    var ms = rng.Next(12, 180);
    Log("INFO", $"GET {route} 200 OK ({ms}ms) [{user}]");

    // occasional warning
    if (rng.Next(8) == 0)
        Log("WARNING", $"Slow query on TaskRepository.Search: {rng.Next(800, 2000)}ms [{requestIds[rng.Next(requestIds.Length)]}]");

    // connection pool noise — every 12 ticks (ignored by monitor)
    if (tick % 12 == 0)
        Log("INFO", "Connection pool recycled (idle timeout)");

    // cascade: DB unavailable triggers multiple failures
    if (tick % 20 == 0)
    {
        var reqId = requestIds[rng.Next(requestIds.Length)];
        Log("ERROR", $"Failed to open DB connection: timeout after 5000ms [{reqId}]");
        Log("ERROR", $"TaskRepository.GetAll failed: underlying connection unavailable [{reqId}]");
        Log("ERROR", $"TaskRepository.Search failed: underlying connection unavailable [{reqId}]");
        Log("CRITICAL", $"StorageService degraded — all read operations failing [{reqId}]");
    }

    // independent error
    if (rng.Next(15) == 0)
        Log("ERROR", $"Unhandled exception in TaskService.Delete: Object reference not set [{requestIds[rng.Next(requestIds.Length)]}]");

    await Task.Delay(TimeSpan.FromSeconds(rng.Next(1, 4)));
}
