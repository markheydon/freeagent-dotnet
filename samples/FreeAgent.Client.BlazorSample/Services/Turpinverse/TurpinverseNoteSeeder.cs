using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Notes;
using FreeAgent.Client.Models.Projects;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Seeds FreeAgent notes on Turpinverse contacts and projects for probe coverage.
/// </summary>
public sealed class TurpinverseNoteSeeder
{
    public const string NoteContentPrefix = "turpinverse:";

    private readonly TurpinverseContactCatalog _contactCatalog;
    private readonly TurpinverseProjectCatalog _projectCatalog;

    public TurpinverseNoteSeeder(
        TurpinverseContactCatalog contactCatalog,
        TurpinverseProjectCatalog projectCatalog)
    {
        _contactCatalog = contactCatalog ?? throw new ArgumentNullException(nameof(contactCatalog));
        _projectCatalog = projectCatalog ?? throw new ArgumentNullException(nameof(projectCatalog));
    }

    public async Task<TurpinverseNoteSeedResult> UpsertTurpinEnterprisesContactNoteAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await _contactCatalog.EnsureLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var contact = await ResolveTurpinEnterprisesContactAsync(client, cancellationToken);
        var content = BuildContactNoteContent();

        return await UpsertContactNoteAsync(client, contact, content, cancellationToken);
    }

    public async Task<TurpinverseNoteSeedResult> UpsertBlackBessProjectNoteAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await _projectCatalog.EnsureLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var project = await ResolveProjectByContractReferenceAsync(
            client,
            TurpinverseProjectMapper.BuildContractReference(TurpinverseProjectCatalog.BlackBessRouteOptimiserProjectId),
            cancellationToken);
        var content = BuildProjectNoteContent(TurpinverseProjectCatalog.BlackBessRouteOptimiserProjectId);

        return await UpsertProjectNoteAsync(client, project, content, cancellationToken);
    }

    private static string BuildContactNoteContent() =>
        $"{NoteContentPrefix} Turpin Enterprises follow-up — canon contact note for SDK probe coverage.";

    private static string BuildProjectNoteContent(string projectId) =>
        $"{NoteContentPrefix} {projectId} — project status note for SDK probe coverage.";

    private async Task<Contact> ResolveTurpinEnterprisesContactAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken)
    {
        var page = await client.Contacts.ListAsync(perPage: 100, cancellationToken: cancellationToken);
        var organisation = _contactCatalog.TurpinEnterprises;
        var desiredEmail = TurpinverseContactMapper.ResolveUpsertEmail(organisation, _contactCatalog.PersonasById);

        var match = page.Items.FirstOrDefault(contact =>
            string.Equals(contact.Email, desiredEmail, StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidOperationException(
                "Turpin Enterprises contact was not found in FreeAgent. Seed contacts first.");
        return match;
    }

    private async Task<Project> ResolveProjectByContractReferenceAsync(
        FreeAgentClient client,
        string contractReference,
        CancellationToken cancellationToken)
    {
        var page = await client.Projects.ListAsync(perPage: 100, cancellationToken: cancellationToken);
        var match = page.Items.FirstOrDefault(project =>
            string.Equals(project.ContractReference, contractReference, StringComparison.Ordinal)) ?? throw new InvalidOperationException(
                $"Project with contract reference '{contractReference}' was not found. Seed projects first.");
        return match;
    }

    private static async Task<TurpinverseNoteSeedResult> UpsertContactNoteAsync(
        FreeAgentClient client,
        Contact contact,
        string content,
        CancellationToken cancellationToken)
    {
        var existing = await client.Notes.ListContactNotesAsync(contactId: contact.ResourceId, cancellationToken: cancellationToken);
        var match = existing.FirstOrDefault(note =>
            note.Content?.StartsWith(NoteContentPrefix, StringComparison.Ordinal) == true);

        if (match is not null)
        {
            var updated = await client.Notes.UpdateNoteAsync(
                match.ResourceId,
                UpdateNoteRequest.Create(content),
                cancellationToken);
            return new TurpinverseNoteSeedResult(updated, NoteSeedAction.Updated);
        }

        var created = await client.Notes.CreateContactNoteAsync(
            contact.ResourceId,
            CreateContactNoteRequest.Create(content),
            cancellationToken);
        return new TurpinverseNoteSeedResult(created, NoteSeedAction.Created);
    }

    private static async Task<TurpinverseNoteSeedResult> UpsertProjectNoteAsync(
        FreeAgentClient client,
        Project project,
        string content,
        CancellationToken cancellationToken)
    {
        var existing = await client.Notes.ListProjectNotesAsync(projectId: project.ResourceId, cancellationToken: cancellationToken);
        var match = existing.FirstOrDefault(note =>
            note.Content?.StartsWith(NoteContentPrefix, StringComparison.Ordinal) == true);

        if (match is not null)
        {
            var updated = await client.Notes.UpdateNoteAsync(
                match.ResourceId,
                UpdateNoteRequest.Create(content),
                cancellationToken);
            return new TurpinverseNoteSeedResult(updated, NoteSeedAction.Updated);
        }

        var created = await client.Notes.CreateProjectNoteAsync(
            project.ResourceId,
            CreateProjectNoteRequest.Create(content),
            cancellationToken);
        return new TurpinverseNoteSeedResult(created, NoteSeedAction.Created);
    }
}

public enum NoteSeedAction
{
    Created,
    Updated
}

public sealed record TurpinverseNoteSeedResult(Note Note, NoteSeedAction Action);
