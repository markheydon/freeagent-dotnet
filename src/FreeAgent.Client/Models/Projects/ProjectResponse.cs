using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Projects;

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
