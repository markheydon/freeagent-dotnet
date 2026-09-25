using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Tasks;

/// <summary>
/// Represents a FreeAgent task belonging to a project.
/// </summary>
public class ProjectTask : IFreeAgentResource
{
    /// <summary>
    /// Task resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Wire representation of the parent project link.
    /// </summary>
    [JsonPropertyName("project")]
    [JsonInclude]
    internal ExpandableField<Project>? ProjectLink { get; set; }

    /// <summary>
    /// Parent project when returned nested on the wire or hydrated via <see cref="ProjectTaskGetOptions.IncludeProject"/>.
    /// </summary>
    [JsonIgnore]
    public Project? Project => ProjectLink?.Value;

    /// <summary>
    /// Parent project identifier parsed from the task response.
    /// </summary>
    [JsonIgnore]
    public long? ProjectId => ProjectLink?.Id;

    /// <summary>
    /// Attaches a hydrated parent project to this task.
    /// </summary>
    /// <param name="project">Parent project details.</param>
    internal void AttachProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        ProjectLink = new ExpandableField<Project>(project.Url, project);
    }

    /// <summary>
    /// Task name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Currency code inherited from the parent project. Read-only on the wire.
    /// </summary>
    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    /// <summary>
    /// Whether the task is billable to clients.
    /// </summary>
    [JsonPropertyName("is_billable")]
    public bool? IsBillable { get; set; }

    /// <summary>
    /// Billing rate per <see cref="BillingPeriod"/>.
    /// </summary>
    [JsonPropertyName("billing_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? BillingRate { get; set; }

    /// <summary>
    /// Unit for <see cref="BillingRate"/>.
    /// </summary>
    [JsonPropertyName("billing_period")]
    public ProjectTaskBillingPeriod? BillingPeriod { get; set; }

    /// <summary>
    /// Task status.
    /// </summary>
    [JsonPropertyName("status")]
    public ProjectTaskStatus? Status { get; set; }

    /// <summary>
    /// Whether the task can be deleted. Returned on single-task GET only.
    /// </summary>
    [JsonPropertyName("is_deletable")]
    public bool? IsDeletable { get; set; }

    /// <summary>
    /// Creation timestamp in UTC.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp in UTC.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
