namespace TaskManager.CLI.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    // TODO: add Tags support
    // TODO: add AssignedTo field for multi-user support
}

public enum TaskStatus
{
    Todo,
    InProgress,
    Done,
    Cancelled
}

public enum Priority
{
    Low,
    Medium,
    High,
    Critical
}
