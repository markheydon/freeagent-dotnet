using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Projects;

/// <summary>
/// Wrapper for projects list API responses.
/// </summary>
public class ProjectsResponse
{
    /// <summary>
    /// Project list payload.
    /// </summary>
    [JsonPropertyName("projects")]
    public List<Project>? Projects { get; set; }
}
