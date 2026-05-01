using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Services;

namespace FreeAgent.Client.Services.Contacts;

/// <summary>
/// Service for interacting with FreeAgent contacts.
/// </summary>
public class ContactService : ServiceBase
{
    /// <summary>
    /// Initializes a new instance of the contact service.
    /// </summary>
    /// <param name="httpClient">FreeAgent HTTP client</param>
    public ContactService(FreeAgentHttpClient httpClient)
        : base(httpClient)
    {
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

        var endpoint = $"contacts?page={page}&per_page={perPage}&view={Uri.EscapeDataString(view)}";
        var response = await HttpClient.GetWithMetadataAsync<ContactsResponse>(endpoint, cancellationToken);

        if (response.Data.Contacts is null)
        {
            throw new FreeAgentApiException("Contacts data missing from API response");
        }

        var total = ParseTotalCount(response) ?? EstimateTotalWithoutHeader(page, perPage, response.Data.Contacts.Count);

        return new PaginatedResponse<ContactSummary>
        {
            Page = page,
            PerPage = perPage,
            Total = total,
            Items = response.Data.Contacts
        };
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

    private static int? ParseTotalCount(FreeAgentHttpResponse<ContactsResponse> response)
    {
        var headerValues = response.GetHeaderValues("X-Total-Count");
        if (headerValues is null || headerValues.Count == 0)
        {
            return null;
        }

        return int.TryParse(headerValues[0], out var total) ? total : null;
    }

    private static int EstimateTotalWithoutHeader(int page, int perPage, int itemsOnPage)
    {
        // If the page is full and total headers are absent, force HasNextPage=true and continue until a short page is observed.
        if (itemsOnPage == perPage)
        {
            return (page * perPage) + 1;
        }

        return ((page - 1) * perPage) + itemsOnPage;
    }
}
