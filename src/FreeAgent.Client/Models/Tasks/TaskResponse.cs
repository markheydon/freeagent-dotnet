using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Tasks;

/// <summary>
/// Wrapper for single task API responses.
/// </summary>
public class TaskResponse
{
    /// <summary>
    /// Task payload.
    /// </summary>
    [JsonPropertyName("task")]
    public Task? Task { get; set; }
}
