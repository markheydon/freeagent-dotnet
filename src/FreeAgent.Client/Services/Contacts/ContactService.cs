using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
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
    /// <param name="view">Contacts view filter (for example: all, active, clients, suppliers)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated contacts response</returns>
    public async Task<PaginatedResponse<ContactSummary>> GetContactsPageAsync(
        int page = 1,
        int perPage = 25,
        string view = "all",
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(perPage, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(perPage, 100);
        ArgumentException.ThrowIfNullOrWhiteSpace(view);

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint(
            endpoint: "contacts",
            queryParameters:
            [
                new KeyValuePair<string, string>("page", page.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("per_page", perPage.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("view", view)
            ]);

        var response = await _requestClient.GetWithMetadataAsync<ContactsResponse>(endpoint, cancellationToken);

        if (response.Data.Contacts is null)
        {
            throw new FreeAgentApiException("Contacts data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(response, page, perPage, response.Data.Contacts.Count);

        return new PaginatedResponse<ContactSummary>(
            page,
            perPage,
            total,
            response.Data.Contacts);
    }

    /// <summary>
    /// Iterates all contacts across all pages.
    /// </summary>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Contacts view filter (for example: all, active, clients, suppliers)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async stream of contacts</returns>
    public async IAsyncEnumerable<ContactSummary> GetAllContactsAsync(
        int perPage = 25,
        string view = "all",
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var contactsPage = await GetContactsPageAsync(page, perPage, view, cancellationToken);

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
}
