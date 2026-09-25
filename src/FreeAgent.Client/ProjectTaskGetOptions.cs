namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Tasks.ProjectTaskService.GetProjectTaskAsync(long, ProjectTaskGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class ProjectTaskGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the parent project when the task response contains only a project URI.
    /// </summary>
    public bool IncludeProject { get; init; }
}
