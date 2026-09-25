using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.RecurringInvoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Services.RecurringInvoices;

/// <summary>
/// Service for interacting with FreeAgent recurring invoices.
/// </summary>
/// <remarks>
/// The FreeAgent API documents list and get operations only for recurring invoices.
/// </remarks>
public sealed class RecurringInvoiceService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the recurring invoice service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal RecurringInvoiceService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists one page of recurring invoices.
    /// </summary>
    /// <param name="page">1-based page number</param>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Optional view filter (for example: <see cref="RecurringInvoiceViews.Draft"/>)</param>
    /// <param name="contact">Filter by contact resource reference</param>
    /// <param name="contactId">Filter by contact identifier (equivalent to <c>client.Urls.Contact(contactId)</c>)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated recurring invoices response</returns>
    public async Task<PaginatedResponse<RecurringInvoice>> ListAsync(
        int page = 1,
        int perPage = 25,
        string? view = null,
        ContactReference? contact = null,
        long? contactId = null,
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

        var contactFilter = ResolveContactFilter(contact, contactId);
        if (contactFilter is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("contact", contactFilter));
        }

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("recurring_invoices", queryParameters);
        var response = await _requestClient.GetWithMetadataAsync<RecurringInvoicesResponse>(endpoint, cancellationToken);

        if (response.Data.RecurringInvoices is null)
        {
            throw new FreeAgentApiException("Recurring invoices data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(
            response,
            page,
            perPage,
            response.Data.RecurringInvoices.Count);

        return new PaginatedResponse<RecurringInvoice>(
            page,
            perPage,
            total,
            response.Data.RecurringInvoices);
    }

    /// <summary>
    /// Lists all recurring invoices across all pages.
    /// </summary>
    public async IAsyncEnumerable<RecurringInvoice> ListAutoPagingAsync(
        int perPage = 25,
        string? view = null,
        ContactReference? contact = null,
        long? contactId = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var recurringInvoicesPage = await ListAsync(
                page,
                perPage,
                view,
                contact,
                contactId,
                cancellationToken);

            foreach (var recurringInvoice in recurringInvoicesPage.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return recurringInvoice;
            }

            if (!recurringInvoicesPage.HasNextPage)
            {
                yield break;
            }

            page = recurringInvoicesPage.NextPage!.Value;
        }
    }

    /// <summary>
    /// Gets a single recurring invoice by identifier.
    /// </summary>
    /// <param name="recurringInvoiceId">Recurring invoice identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Recurring invoice details</returns>
    public Task<RecurringInvoice> GetRecurringInvoiceAsync(
        long recurringInvoiceId,
        CancellationToken cancellationToken = default) =>
        GetRecurringInvoiceAsync(recurringInvoiceId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single recurring invoice by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="recurringInvoiceId">Recurring invoice identifier from the resource URL</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Recurring invoice details</returns>
    public async Task<RecurringInvoice> GetRecurringInvoiceAsync(
        long recurringInvoiceId,
        RecurringInvoiceGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(recurringInvoiceId);

        var response = await _requestClient.GetAsync<RecurringInvoiceResponse>(
            $"recurring_invoices/{recurringInvoiceId}",
            cancellationToken);

        if (response.RecurringInvoice is null)
        {
            throw new FreeAgentApiException("Recurring invoice data missing from API response");
        }

        await LinkedResourceHydration.HydrateBillingContactAsync(
            response.RecurringInvoice,
            _requestClient,
            options?.IncludeContact == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateProjectAsync(
            response.RecurringInvoice,
            _requestClient,
            options?.IncludeProject == true,
            cancellationToken);

        return response.RecurringInvoice;
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
}
