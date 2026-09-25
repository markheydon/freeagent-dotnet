using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.Tasks;
using FreeAgent.Client.Models.Users;

namespace FreeAgent.Client.Models.Timeslips;

/// <summary>
/// Represents a FreeAgent timeslip.
/// </summary>
public class Timeslip : IFreeAgentResource
{
    private SettableLinkId _projectTaskId;
    private SettableLinkId _projectId;
    private SettableLinkId _userId;

    /// <summary>
    /// Timeslip resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Wire representation of the task link.
    /// </summary>
    [JsonPropertyName("task")]
    [JsonInclude]
    internal ExpandableField<ProjectTask>? ProjectTaskLink { get; set; }

    /// <summary>
    /// Task when returned nested on the wire or hydrated via <see cref="TimeslipGetOptions.IncludeProjectTask"/>.
    /// </summary>
    [JsonIgnore]
    public ProjectTask? ProjectTask => ProjectTaskLink?.Value;

    /// <summary>
    /// Task identifier for create and update requests. Populated after GET when the API returns a task link.
    /// </summary>
    [JsonIgnore]
    public long? ProjectTaskId
    {
        get => _projectTaskId.Get(ProjectTaskLink?.Id);
        set => _projectTaskId.Set(value);
    }

    /// <summary>
    /// Wire representation of the project link.
    /// </summary>
    [JsonPropertyName("project")]
    [JsonInclude]
    internal ExpandableField<Project>? ProjectLink { get; set; }

    /// <summary>
    /// Project when returned nested on the wire or hydrated via <see cref="TimeslipGetOptions.IncludeProject"/>.
    /// </summary>
    [JsonIgnore]
    public Project? Project => ProjectLink?.Value;

    /// <summary>
    /// Project identifier for create and update requests. Populated after GET when the API returns a project link.
    /// </summary>
    [JsonIgnore]
    public long? ProjectId
    {
        get => _projectId.Get(ProjectLink?.Id);
        set => _projectId.Set(value);
    }

    /// <summary>
    /// Wire representation of the user link.
    /// </summary>
    [JsonPropertyName("user")]
    [JsonInclude]
    internal ExpandableField<User>? UserLink { get; set; }

    /// <summary>
    /// User when returned nested on the wire or hydrated via <see cref="TimeslipGetOptions.IncludeUser"/>.
    /// </summary>
    [JsonIgnore]
    public User? User => UserLink?.Value;

    /// <summary>
    /// User identifier for create and update requests. Populated after GET when the API returns a user link.
    /// </summary>
    [JsonIgnore]
    public long? UserId
    {
        get => _userId.Get(UserLink?.Id);
        set => _userId.Set(value);
    }

    /// <summary>
    /// Wire representation of the billed invoice link.
    /// </summary>
    [JsonPropertyName("billed_on_invoice")]
    [JsonInclude]
    internal ExpandableField<Invoice>? BilledOnInvoiceLink { get; set; }

    /// <summary>
    /// Invoice identifier when the timeslip has been billed. Read-only on the wire.
    /// </summary>
    [JsonIgnore]
    public long? BilledOnInvoiceId => BilledOnInvoiceLink?.Id;

    /// <summary>
    /// Date of the timeslip.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Number of hours worked.
    /// </summary>
    [JsonPropertyName("hours")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Hours { get; set; }

    /// <summary>
    /// Free-text comment.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>
    /// Running timer state when a timer is active.
    /// </summary>
    [JsonPropertyName("timer")]
    public TimeslipTimer? Timer { get; set; }

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

    internal SettableLinkId ProjectTaskIdBacking => _projectTaskId;

    internal long? ProjectTaskLinkId => ProjectTaskLink?.Id;

    internal SettableLinkId ProjectIdBacking => _projectId;

    internal long? ProjectLinkId => ProjectLink?.Id;

    internal SettableLinkId UserIdBacking => _userId;

    internal long? UserLinkId => UserLink?.Id;

    /// <summary>
    /// Attaches a hydrated task to this timeslip.
    /// </summary>
    /// <param name="task">Task details.</param>
    internal void AttachProjectTask(ProjectTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        ProjectTaskLink = new ExpandableField<ProjectTask>(task.Url, task);
    }

    /// <summary>
    /// Attaches a hydrated project to this timeslip.
    /// </summary>
    /// <param name="project">Project details.</param>
    internal void AttachProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        ProjectLink = new ExpandableField<Project>(project.Url, project);
    }

    /// <summary>
    /// Attaches a hydrated user to this timeslip.
    /// </summary>
    /// <param name="user">User details.</param>
    internal void AttachUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        UserLink = new ExpandableField<User>(user.Url, user);
    }
}
