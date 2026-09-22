namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Tasks.TaskService.GetTaskAsync(long, TaskGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class TaskGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the parent project when the task response contains only a project URI.
    /// </summary>
    public bool IncludeProject { get; init; }
}
