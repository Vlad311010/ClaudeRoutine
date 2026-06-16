using System.Text.Json;
using TaskManager.CLI.Models;

namespace TaskManager.CLI.Services;

public class StorageService
{
    private readonly string _filePath;
    // HACK: using a flat JSON file instead of a real DB — fine for demo, won't scale
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public StorageService(string filePath = "tasks.json")
    {
        _filePath = filePath;
    }

    public List<TaskItem> Load()
    {
        if (!File.Exists(_filePath))
            return new List<TaskItem>();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<TaskItem>>(json, _jsonOptions) ?? new List<TaskItem>();
    }

    public void Save(List<TaskItem> tasks)
    {
        // TODO: add backup before overwrite
        var json = JsonSerializer.Serialize(tasks, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }
}
