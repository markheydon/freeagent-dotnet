using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for single project API responses.
/// </summary>
public class ProjectResponse
{
    /// <summary>
    /// Project payload.
    /// </summary>
    [JsonPropertyName("project")]
    public Project? Project { get; set; }
}
