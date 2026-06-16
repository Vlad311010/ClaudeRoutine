using TaskManager.CLI.Models;
using TaskStatus = TaskManager.CLI.Models.TaskStatus;

namespace TaskManager.CLI.Services;

public class TaskService
{
    private readonly StorageService _storage;
    private List<TaskItem> _tasks;

    public TaskService(StorageService storage)
    {
        _storage = storage;
        _tasks = _storage.Load();
    }

    public IReadOnlyList<TaskItem> GetAll() => _tasks.AsReadOnly();

    public IReadOnlyList<TaskItem> GetByStatus(TaskStatus status) =>
        _tasks.Where(t => t.Status == status).ToList().AsReadOnly();

    public TaskItem? GetById(int id) => _tasks.FirstOrDefault(t => t.Id == id);

    public TaskItem Add(string title, string description, Priority priority = Priority.Medium)
    {
        // HACK: ID generation breaks if tasks are deleted — use a real sequence later
        var id = _tasks.Count == 0 ? 1 : _tasks.Max(t => t.Id) + 1;
        var task = new TaskItem
        {
            Id = id,
            Title = title,
            Description = description,
            Priority = priority
        };
        _tasks.Add(task);
        _storage.Save(_tasks);
        return task;
    }

    public bool UpdateStatus(int id, TaskStatus status)
    {
        var task = GetById(id);
        if (task == null) return false;

        task.Status = status;
        if (status == TaskStatus.Done)
            task.CompletedAt = DateTime.UtcNow;

        _storage.Save(_tasks);
        return true;
    }

    public bool Delete(int id)
    {
        var task = GetById(id);
        if (task == null) return false;
        // TODO: soft delete instead of hard delete
        _tasks.Remove(task);
        _storage.Save(_tasks);
        return true;
    }

    public List<TaskItem> Search(string query)
    {
        // TODO: make search case-insensitive and support description search
        return _tasks.Where(t => t.Title.Contains(query)).ToList();
    }

    public Dictionary<Priority, int> GetPriorityStats()
    {
        return _tasks
            .GroupBy(t => t.Priority)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public Dictionary<TaskStatus, int> GetStatusStats()
    {
        return _tasks
            .GroupBy(t => t.Status)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
