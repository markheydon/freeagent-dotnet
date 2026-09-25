using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Tasks;

/// <summary>
/// Wrapper for task list API responses.
/// </summary>
public class ProjectTasksResponse
{
    /// <summary>
    /// Task collection payload.
    /// </summary>
    [JsonPropertyName("tasks")]
    public List<ProjectTask>? ProjectTasks { get; set; }
}
