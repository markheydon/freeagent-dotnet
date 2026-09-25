using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Tasks;

/// <summary>
/// Wrapper for task create and update request payloads.
/// </summary>
internal sealed class ProjectTaskRequest
{
    /// <summary>
    /// Task attributes to create or update.
    /// </summary>
    [JsonPropertyName("task")]
    public TaskWritePayload? ProjectTask { get; set; }
}
