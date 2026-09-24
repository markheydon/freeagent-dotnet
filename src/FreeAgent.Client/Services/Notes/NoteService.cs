using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Notes;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Services.Notes;

/// <summary>
/// Service for interacting with FreeAgent notes.
/// </summary>
/// <remarks>
/// Notes belong to a contact or project. List and create operations scope the parent via
/// <c>contact</c> or <c>project</c> query parameters.
/// </remarks>
public sealed class NoteService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the note service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal NoteService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all notes for a contact.
    /// </summary>
    /// <param name="contact">Parent contact resource reference</param>
    /// <param name="contactId">Parent contact identifier (equivalent to <c>client.Urls.Contact(contactId)</c>)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All notes returned by FreeAgent for the contact</returns>
    /// <exception cref="ArgumentException">Neither or both of <paramref name="contact"/> and <paramref name="contactId"/> are supplied.</exception>
    public Task<IReadOnlyList<Note>> ListContactNotesAsync(
        ContactReference? contact = null,
        long? contactId = null,
        CancellationToken cancellationToken = default) =>
        ListNotesAsync("contact", ResolveContactFilter(contact, contactId), cancellationToken);

    /// <summary>
    /// Lists all notes for a project.
    /// </summary>
    /// <param name="project">Parent project resource reference</param>
    /// <param name="projectId">Parent project identifier (equivalent to <c>client.Urls.Project(projectId)</c>)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All notes returned by FreeAgent for the project</returns>
    /// <exception cref="ArgumentException">Neither or both of <paramref name="project"/> and <paramref name="projectId"/> are supplied.</exception>
    public Task<IReadOnlyList<Note>> ListProjectNotesAsync(
        ProjectReference? project = null,
        long? projectId = null,
        CancellationToken cancellationToken = default) =>
        ListNotesAsync("project", ResolveProjectFilter(project, projectId), cancellationToken);

    /// <summary>
    /// Gets a single note by identifier.
    /// </summary>
    /// <param name="noteId">Note identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Note details</returns>
    public Task<Note> GetNoteAsync(long noteId, CancellationToken cancellationToken = default) =>
        GetNoteAsync(noteId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single note by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="noteId">Note identifier from the resource URL</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Note details</returns>
    public async Task<Note> GetNoteAsync(
        long noteId,
        NoteGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(noteId);

        var response = await _requestClient.GetAsync<NoteResponse>($"notes/{noteId}", cancellationToken);

        if (response.Note is null)
        {
            throw new FreeAgentApiException("Note data missing from API response");
        }

        await LinkedResourceHydration.HydrateNoteParentAsync(
            response.Note,
            _requestClient,
            options?.IncludeParentContact == true,
            options?.IncludeParentProject == true,
            cancellationToken);

        return response.Note;
    }

    /// <summary>
    /// Creates a note on a contact.
    /// </summary>
    /// <param name="contactId">Parent contact identifier</param>
    /// <param name="request">Note attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created note</returns>
    public Task<Note> CreateContactNoteAsync(
        long contactId,
        CreateContactNoteRequest request,
        CancellationToken cancellationToken = default) =>
        CreateContactNoteAsync(
            ContactReference.ForEnvironment(_requestClient.Environment, contactId),
            request,
            cancellationToken);

    /// <summary>
    /// Creates a note on a contact.
    /// </summary>
    /// <param name="contact">Parent contact resource reference</param>
    /// <param name="request">Note attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created note</returns>
    public Task<Note> CreateContactNoteAsync(
        ContactReference contact,
        CreateContactNoteRequest request,
        CancellationToken cancellationToken = default) =>
        CreateNoteAsync("contact", contact.Uri, request, cancellationToken);

    /// <summary>
    /// Creates a note on a project.
    /// </summary>
    /// <param name="projectId">Parent project identifier</param>
    /// <param name="request">Note attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created note</returns>
    public Task<Note> CreateProjectNoteAsync(
        long projectId,
        CreateProjectNoteRequest request,
        CancellationToken cancellationToken = default) =>
        CreateProjectNoteAsync(
            ProjectReference.ForEnvironment(_requestClient.Environment, projectId),
            request,
            cancellationToken);

    /// <summary>
    /// Creates a note on a project.
    /// </summary>
    /// <param name="project">Parent project resource reference</param>
    /// <param name="request">Note attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created note</returns>
    public Task<Note> CreateProjectNoteAsync(
        ProjectReference project,
        CreateProjectNoteRequest request,
        CancellationToken cancellationToken = default) =>
        CreateNoteAsync("project", project.Uri, request, cancellationToken);

    /// <summary>
    /// Updates a note.
    /// </summary>
    /// <param name="noteId">Note identifier from the resource URL</param>
    /// <param name="request">Note attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated note</returns>
    public async Task<Note> UpdateNoteAsync(
        long noteId,
        UpdateNoteRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(noteId);
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new NoteRequest
        {
            Note = NoteWritePayload.FromUpdate(request)
        });

        var response = await _requestClient.PutAsync<NoteResponse>($"notes/{noteId}", content, cancellationToken);

        if (response.Note is null)
        {
            throw new FreeAgentApiException("Note data missing from API response");
        }

        return response.Note;
    }

    /// <summary>
    /// Deletes a note.
    /// </summary>
    /// <param name="noteId">Note identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public System.Threading.Tasks.Task DeleteNoteAsync(long noteId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(noteId);

        return _requestClient.DeleteAsync($"notes/{noteId}", cancellationToken);
    }

    private async Task<IReadOnlyList<Note>> ListNotesAsync(
        string parentQueryKey,
        string parentUri,
        CancellationToken cancellationToken)
    {
        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint(
            "notes",
            [new KeyValuePair<string, string>(parentQueryKey, parentUri)]);

        var response = await _requestClient.GetAsync<NotesResponse>(endpoint, cancellationToken);

        if (response.Notes is null)
        {
            throw new FreeAgentApiException("Notes data missing from API response");
        }

        return response.Notes;
    }

    private async Task<Note> CreateNoteAsync(
        string parentQueryKey,
        string parentUri,
        CreateContactNoteRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint(
            "notes",
            [new KeyValuePair<string, string>(parentQueryKey, parentUri)]);

        var content = FreeAgentJsonSerializer.CreateContent(new NoteRequest
        {
            Note = NoteWritePayload.FromCreate(request)
        });

        var response = await _requestClient.PostAsync<NoteResponse>(endpoint, content, cancellationToken);

        if (response.Note is null)
        {
            throw new FreeAgentApiException("Note data missing from API response");
        }

        return response.Note;
    }

    private async Task<Note> CreateNoteAsync(
        string parentQueryKey,
        string parentUri,
        CreateProjectNoteRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint(
            "notes",
            [new KeyValuePair<string, string>(parentQueryKey, parentUri)]);

        var content = FreeAgentJsonSerializer.CreateContent(new NoteRequest
        {
            Note = NoteWritePayload.FromCreate(request)
        });

        var response = await _requestClient.PostAsync<NoteResponse>(endpoint, content, cancellationToken);

        if (response.Note is null)
        {
            throw new FreeAgentApiException("Note data missing from API response");
        }

        return response.Note;
    }

    private string ResolveContactFilter(ContactReference? contact, long? contactId)
    {
        if (contact is not null && contactId is not null)
        {
            throw new ArgumentException(
                "Specify either contact or contactId, not both.",
                nameof(contact));
        }

        if (contact is not null)
        {
            return contact.Value.Uri;
        }

        if (contactId is null)
        {
            throw new ArgumentException(
                "Specify either contact or contactId.",
                nameof(contact));
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(contactId.Value);
        return ContactReference.ForEnvironment(_requestClient.Environment, contactId.Value).Uri;
    }

    private string ResolveProjectFilter(ProjectReference? project, long? projectId)
    {
        if (project is not null && projectId is not null)
        {
            throw new ArgumentException(
                "Specify either project or projectId, not both.",
                nameof(project));
        }

        if (project is not null)
        {
            return project.Value.Uri;
        }

        if (projectId is null)
        {
            throw new ArgumentException(
                "Specify either project or projectId.",
                nameof(project));
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(projectId.Value);
        return ProjectReference.ForEnvironment(_requestClient.Environment, projectId.Value).Uri;
    }
}
