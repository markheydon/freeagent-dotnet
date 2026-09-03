using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.Services.Contacts;

/// <summary>
/// Service for interacting with FreeAgent contacts.
/// </summary>
public sealed class ContactService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the contact service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal ContactService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Gets one page of contacts.
    /// </summary>
    /// <param name="page">1-based page number</param>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Contacts view filter (for example: <see cref="ContactViews.Active"/>)</param>
    /// <param name="sort">Sort field (name, created_at, updated_at); prefix with <c>-</c> for descending</param>
    /// <param name="updatedSince">Return contacts updated since this timestamp (ISO 8601)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated contacts response</returns>
    public async Task<PaginatedResponse<Contact>> GetContactsPageAsync(
        int page = 1,
        int perPage = 25,
        string view = ContactViews.Active,
        string? sort = null,
        DateTimeOffset? updatedSince = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(perPage, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(perPage, 100);
        ArgumentException.ThrowIfNullOrWhiteSpace(view);

        var queryParameters = new List<KeyValuePair<string, string>>
        {
            new("page", page.ToString(CultureInfo.InvariantCulture)),
            new("per_page", perPage.ToString(CultureInfo.InvariantCulture)),
            new("view", view)
        };

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

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("contacts", queryParameters);

        var response = await _requestClient.GetWithMetadataAsync<ContactsResponse>(endpoint, cancellationToken);

        if (response.Data.Contacts is null)
        {
            throw new FreeAgentApiException("Contacts data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(response, page, perPage, response.Data.Contacts.Count);

        return new PaginatedResponse<Contact>(
            page,
            perPage,
            total,
            response.Data.Contacts);
    }

    /// <summary>
    /// Iterates all contacts across all pages.
    /// </summary>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Contacts view filter (for example: <see cref="ContactViews.Active"/>)</param>
    /// <param name="sort">Sort field (name, created_at, updated_at); prefix with <c>-</c> for descending</param>
    /// <param name="updatedSince">Return contacts updated since this timestamp (ISO 8601)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async stream of contacts</returns>
    public async IAsyncEnumerable<Contact> GetAllContactsAsync(
        int perPage = 25,
        string view = ContactViews.Active,
        string? sort = null,
        DateTimeOffset? updatedSince = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var contactsPage = await GetContactsPageAsync(page, perPage, view, sort, updatedSince, cancellationToken);

            foreach (var contact in contactsPage.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return contact;
            }

            if (!contactsPage.HasNextPage)
            {
                yield break;
            }

            page = contactsPage.NextPage!.Value;
        }
    }

    /// <summary>
    /// Gets a single contact by identifier.
    /// </summary>
    /// <param name="contactId">Contact identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Contact details</returns>
    public async Task<Contact> GetContactAsync(long contactId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(contactId);

        var response = await _requestClient.GetAsync<ContactResponse>($"contacts/{contactId}", cancellationToken);

        if (response.Contact is null)
        {
            throw new FreeAgentApiException("Contact data missing from API response");
        }

        return response.Contact;
    }

    /// <summary>
    /// Creates a contact.
    /// </summary>
    /// <param name="contact">Contact attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created contact</returns>
    public async Task<Contact> CreateContactAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(contact);

        var content = FreeAgentJsonSerializer.CreateContent(new ContactRequest { Contact = ContactWritePayload.FromContact(contact) });
        var response = await _requestClient.PostAsync<ContactResponse>("contacts", content, cancellationToken);

        if (response.Contact is null)
        {
            throw new FreeAgentApiException("Contact data missing from API response");
        }

        return response.Contact;
    }

    /// <summary>
    /// Updates a contact.
    /// </summary>
    /// <param name="contactId">Contact identifier from the resource URL</param>
    /// <param name="contact">Contact attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated contact</returns>
    public async Task<Contact> UpdateContactAsync(long contactId, Contact contact, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(contactId);
        ArgumentNullException.ThrowIfNull(contact);

        var content = FreeAgentJsonSerializer.CreateContent(new ContactRequest { Contact = ContactWritePayload.FromContact(contact) });
        var response = await _requestClient.PutAsync<ContactResponse>($"contacts/{contactId}", content, cancellationToken);

        if (response.Contact is null)
        {
            throw new FreeAgentApiException("Contact data missing from API response");
        }

        return response.Contact;
    }

    /// <summary>
    /// Deletes a contact.
    /// </summary>
    /// <param name="contactId">Contact identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteContactAsync(long contactId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(contactId);

        return _requestClient.DeleteAsync($"contacts/{contactId}", cancellationToken);
    }
}
