using System.Globalization;
using System.Text;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.CreditNotes;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Services.CreditNotes;

/// <summary>
/// Service for interacting with FreeAgent credit notes.
/// </summary>
public sealed class CreditNotesService
{
    private static readonly StringContent EmptyJsonContent =
        new("{}", Encoding.UTF8, "application/json");

    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the credit notes service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal CreditNotesService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists one page of credit notes.
    /// </summary>
    /// <param name="page">1-based page number</param>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Optional view filter (for example: <see cref="CreditNoteViews.Open"/>)</param>
    /// <param name="sort">Sort field (<see cref="CreditNoteSortOptions.CreatedAt"/> or <see cref="CreditNoteSortOptions.UpdatedAt"/>); prefix with <c>-</c> for descending</param>
    /// <param name="updatedSince">Return credit notes updated since this UTC timestamp</param>
    /// <param name="contact">Filter by contact resource reference</param>
    /// <param name="contactId">Filter by contact identifier (equivalent to <c>client.Urls.Contact(contactId)</c>)</param>
    /// <param name="project">Filter by project resource reference</param>
    /// <param name="projectId">Filter by project identifier (equivalent to <c>client.Urls.Project(projectId)</c>)</param>
    /// <param name="nestedCreditNoteItems">When <see langword="true"/>, include credit note items nested in each credit note</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated credit notes response</returns>
    public async Task<PaginatedResponse<CreditNote>> ListAsync(
        int page = 1,
        int perPage = 25,
        string? view = null,
        string? sort = null,
        DateTimeOffset? updatedSince = null,
        ContactReference? contact = null,
        long? contactId = null,
        ProjectReference? project = null,
        long? projectId = null,
        bool? nestedCreditNoteItems = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(perPage, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(perPage, 100);

        var queryParameters = new List<KeyValuePair<string, string>>
        {
            new("page", page.ToString(CultureInfo.InvariantCulture)),
            new("per_page", perPage.ToString(CultureInfo.InvariantCulture))
        };

        if (!string.IsNullOrWhiteSpace(view))
        {
            queryParameters.Add(new KeyValuePair<string, string>("view", view));
        }

        if (!string.IsNullOrWhiteSpace(sort))
        {
            queryParameters.Add(new KeyValuePair<string, string>("sort", sort));
        }

        if (updatedSince is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "updated_since",
                updatedSince.Value.ToString("o", CultureInfo.InvariantCulture)));
        }

        var contactFilter = ResolveContactFilter(contact, contactId);
        if (contactFilter is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("contact", contactFilter));
        }

        var projectFilter = ResolveProjectFilter(project, projectId);
        if (projectFilter is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("project", projectFilter));
        }

        if (nestedCreditNoteItems is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "nested_credit_note_items",
                nestedCreditNoteItems.Value ? "true" : "false"));
        }

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("credit_notes", queryParameters);
        var response = await _requestClient.GetWithMetadataAsync<CreditNotesResponse>(endpoint, cancellationToken);

        if (response.Data.CreditNotes is null)
        {
            throw new FreeAgentApiException("Credit notes data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(
            response,
            page,
            perPage,
            response.Data.CreditNotes.Count);

        return new PaginatedResponse<CreditNote>(
            page,
            perPage,
            total,
            response.Data.CreditNotes);
    }

    /// <summary>
    /// Lists all credit notes across all pages.
    /// </summary>
    public async IAsyncEnumerable<CreditNote> ListAutoPagingAsync(
        int perPage = 25,
        string? view = null,
        string? sort = null,
        DateTimeOffset? updatedSince = null,
        ContactReference? contact = null,
        long? contactId = null,
        ProjectReference? project = null,
        long? projectId = null,
        bool? nestedCreditNoteItems = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var creditNotesPage = await ListAsync(
                page,
                perPage,
                view,
                sort,
                updatedSince,
                contact,
                contactId,
                project,
                projectId,
                nestedCreditNoteItems,
                cancellationToken);

            foreach (var creditNote in creditNotesPage.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return creditNote;
            }

            if (!creditNotesPage.HasNextPage)
            {
                yield break;
            }

            page = creditNotesPage.NextPage!.Value;
        }
    }

    /// <summary>
    /// Gets a single credit note by identifier.
    /// </summary>
    /// <param name="creditNoteId">Credit note identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Credit note details</returns>
    public Task<CreditNote> GetCreditNoteAsync(long creditNoteId, CancellationToken cancellationToken = default) =>
        GetCreditNoteAsync(creditNoteId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single credit note by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="creditNoteId">Credit note identifier from the resource URL</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Credit note details</returns>
    public async Task<CreditNote> GetCreditNoteAsync(
        long creditNoteId,
        CreditNoteGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(creditNoteId);

        var response = await _requestClient.GetAsync<CreditNoteResponse>(
            $"credit_notes/{creditNoteId}",
            cancellationToken);

        if (response.CreditNote is null)
        {
            throw new FreeAgentApiException("Credit note data missing from API response");
        }

        await LinkedResourceHydration.HydrateBillingContactAsync(
            response.CreditNote,
            _requestClient,
            options?.IncludeContact == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateProjectAsync(
            response.CreditNote,
            _requestClient,
            options?.IncludeProject == true,
            cancellationToken);

        return response.CreditNote;
    }

    /// <summary>
    /// Downloads a credit note PDF.
    /// </summary>
    /// <param name="creditNoteId">Credit note identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Decoded PDF bytes</returns>
    public async Task<byte[]> GetCreditNotePdfAsync(long creditNoteId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(creditNoteId);

        var response = await _requestClient.GetAsync<CreditNotePdfResponse>(
            $"credit_notes/{creditNoteId}/pdf",
            cancellationToken);

        if (response.Pdf?.Content is not string encodedContent)
        {
            throw new FreeAgentApiException("PDF content missing from API response");
        }

        try
        {
            return Convert.FromBase64String(encodedContent);
        }
        catch (FormatException ex)
        {
            throw new FreeAgentApiException("PDF content is not valid base64.", ex);
        }
    }

    /// <summary>
    /// Creates a credit note.
    /// </summary>
    /// <param name="creditNote">Credit note attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created credit note</returns>
    public async Task<CreditNote> CreateCreditNoteAsync(
        CreditNote creditNote,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(creditNote);

        var content = FreeAgentJsonSerializer.CreateContent(new CreditNoteRequest
        {
            CreditNote = CreditNoteWritePayload.FromCreditNote(creditNote, _requestClient.Environment)
        });

        var response = await _requestClient.PostAsync<CreditNoteResponse>("credit_notes", content, cancellationToken);

        if (response.CreditNote is null)
        {
            throw new FreeAgentApiException("Credit note data missing from API response");
        }

        return response.CreditNote;
    }

    /// <summary>
    /// Updates a credit note.
    /// </summary>
    /// <param name="creditNoteId">Credit note identifier from the resource URL</param>
    /// <param name="creditNote">Credit note attributes to update</param>
    /// <param name="options">Optional update behaviour</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated credit note</returns>
    public async Task<CreditNote> UpdateCreditNoteAsync(
        long creditNoteId,
        CreditNote creditNote,
        CreditNoteUpdateOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(creditNoteId);
        ArgumentNullException.ThrowIfNull(creditNote);

        var content = FreeAgentJsonSerializer.CreateContent(new CreditNoteRequest
        {
            CreditNote = CreditNoteWritePayload.FromCreditNote(
                creditNote,
                _requestClient.Environment,
                options?.OmitLineItems == true,
                LinkedResourceWriteOptions.FromCreditNoteUpdate(options))
        });

        var response = await _requestClient.PutAsync<CreditNoteResponse>(
            $"credit_notes/{creditNoteId}",
            content,
            cancellationToken);

        if (response.CreditNote is null)
        {
            throw new FreeAgentApiException("Credit note data missing from API response");
        }

        return response.CreditNote;
    }

    /// <summary>
    /// Deletes a credit note.
    /// </summary>
    /// <param name="creditNoteId">Credit note identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteCreditNoteAsync(long creditNoteId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(creditNoteId);

        return _requestClient.DeleteAsync($"credit_notes/{creditNoteId}", cancellationToken);
    }

    /// <summary>
    /// Sends a credit note by email.
    /// </summary>
    /// <param name="creditNoteId">Credit note identifier from the resource URL</param>
    /// <param name="request">Email attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task SendCreditNoteEmailAsync(
        long creditNoteId,
        SendCreditNoteEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(creditNoteId);
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new SendCreditNoteEmailRequestEnvelope
        {
            CreditNote = request
        });

        return _requestClient.SendPostAsync(
            $"credit_notes/{creditNoteId}/send_email",
            content,
            cancellationToken);
    }

    /// <summary>
    /// Marks a credit note as sent.
    /// </summary>
    /// <param name="creditNoteId">Credit note identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated credit note</returns>
    public Task<CreditNote> MarkCreditNoteAsSentAsync(long creditNoteId, CancellationToken cancellationToken = default) =>
        TransitionCreditNoteAsync(creditNoteId, "mark_as_sent", cancellationToken);

    /// <summary>
    /// Marks a credit note as draft.
    /// </summary>
    /// <param name="creditNoteId">Credit note identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated credit note</returns>
    public Task<CreditNote> MarkCreditNoteAsDraftAsync(long creditNoteId, CancellationToken cancellationToken = default) =>
        TransitionCreditNoteAsync(creditNoteId, "mark_as_draft", cancellationToken);

    private async Task<CreditNote> TransitionCreditNoteAsync(
        long creditNoteId,
        string transition,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(creditNoteId);
        ArgumentException.ThrowIfNullOrWhiteSpace(transition);

        await _requestClient.SendPutAsync(
            $"credit_notes/{creditNoteId}/transitions/{transition}",
            EmptyJsonContent,
            cancellationToken);

        return await GetCreditNoteAsync(creditNoteId, cancellationToken: cancellationToken);
    }

    private string? ResolveContactFilter(ContactReference? contact, long? contactId)
    {
        if (contact is not null && contactId is not null)
        {
            throw new ArgumentException("Specify either contact or contactId, not both.", nameof(contact));
        }

        if (contact is not null)
        {
            return contact.Value.Uri;
        }

        if (contactId is null)
        {
            return null;
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(contactId.Value);
        return ContactReference.ForEnvironment(_requestClient.Environment, contactId.Value).Uri;
    }

    private string? ResolveProjectFilter(ProjectReference? project, long? projectId)
    {
        if (project is not null && projectId is not null)
        {
            throw new ArgumentException("Specify either project or projectId, not both.", nameof(project));
        }

        if (project is not null)
        {
            return project.Value.Uri;
        }

        if (projectId is null)
        {
            return null;
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(projectId.Value);
        return ProjectReference.ForEnvironment(_requestClient.Environment, projectId.Value).Uri;
    }
}
