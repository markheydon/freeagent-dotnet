using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Projects;

/// <summary>
/// Wrapper for project create and update request payloads.
/// </summary>
internal sealed class ProjectRequest
{
    /// <summary>
    /// Project attributes to create or update.
    /// </summary>
    [JsonPropertyName("project")]
    public ProjectWritePayload? Project { get; set; }
}
