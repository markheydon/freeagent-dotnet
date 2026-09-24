using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Projects;

namespace FreeAgent.Client.Models.Notes;

/// <summary>
/// Represents a FreeAgent note attached to a contact or project.
/// </summary>
public class Note : IFreeAgentResource
{
    private Contact? _contact;
    private Project? _project;

    /// <summary>
    /// Note resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Note content.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Content { get; set; }

    /// <summary>
    /// Wire representation of the parent contact or project link.
    /// </summary>
    [JsonPropertyName("parent_url")]
    [JsonInclude]
    internal string? ParentUrl { get; set; }

    /// <summary>
    /// Parent contact when <see cref="ParentUrl"/> refers to a contact, or when hydrated via <see cref="NoteGetOptions.IncludeParentContact"/>.
    /// </summary>
    [JsonIgnore]
    public Contact? Contact => _contact;

    /// <summary>
    /// Parent contact identifier parsed from <see cref="ParentUrl"/>.
    /// </summary>
    [JsonIgnore]
    public long? ContactId => TryParseParentContactId(out var id) ? id : null;

    /// <summary>
    /// Parent project when <see cref="ParentUrl"/> refers to a project, or when hydrated via <see cref="NoteGetOptions.IncludeParentProject"/>.
    /// </summary>
    [JsonIgnore]
    public Project? Project => _project;

    /// <summary>
    /// Parent project identifier parsed from <see cref="ParentUrl"/>.
    /// </summary>
    [JsonIgnore]
    public long? ProjectId => TryParseParentProjectId(out var id) ? id : null;

    /// <summary>
    /// Name of the user that created the note. Read-only on the wire.
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

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

    /// <summary>
    /// Attaches a hydrated parent contact to this note.
    /// </summary>
    /// <param name="contact">Parent contact details.</param>
    internal void AttachContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        _contact = contact;
    }

    /// <summary>
    /// Attaches a hydrated parent project to this note.
    /// </summary>
    /// <param name="project">Parent project details.</param>
    internal void AttachProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        _project = project;
    }

    internal bool TryParseParentContactId(out long id)
    {
        id = 0;
        return !string.IsNullOrWhiteSpace(ParentUrl)
            && ParentUrl.Contains("/contacts/", StringComparison.OrdinalIgnoreCase)
            && FreeAgentResourceId.TryParse(ParentUrl, out id);
    }

    internal bool TryParseParentProjectId(out long id)
    {
        id = 0;
        if (string.IsNullOrWhiteSpace(ParentUrl))
        {
            return false;
        }

        if (!ParentUrl.Contains("/projects/", StringComparison.OrdinalIgnoreCase)
            && !ParentUrl.Contains("/project/", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return FreeAgentResourceId.TryParse(ParentUrl, out id);
    }
}
