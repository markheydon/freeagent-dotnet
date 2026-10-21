using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for single task API responses.
/// </summary>
public class ProjectTaskResponse
{
    /// <summary>
    /// Task payload.
    /// </summary>
    [JsonPropertyName("task")]
    public ProjectTask? ProjectTask { get; set; }
}
