using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Services.Bills;

/// <summary>
/// Service for interacting with FreeAgent bills.
/// </summary>
public sealed class BillService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the bills service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal BillService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists one page of bills.
    /// </summary>
    /// <param name="page">1-based page number</param>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Optional view filter (for example: <see cref="BillViews.Open"/>)</param>
    /// <param name="fromDate">Return bills dated on or after this date</param>
    /// <param name="toDate">Return bills dated on or before this date</param>
    /// <param name="updatedSince">Return bills updated since this UTC timestamp</param>
    /// <param name="contact">Filter by contact resource reference</param>
    /// <param name="contactId">Filter by contact identifier (equivalent to <c>client.Urls.Contact(contactId)</c>)</param>
    /// <param name="project">Filter by project resource reference</param>
    /// <param name="projectId">Filter by project identifier (equivalent to <c>client.Urls.Project(projectId)</c>)</param>
    /// <param name="nestedBillItems">When <see langword="true"/>, include bill items nested in each bill</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated bills response</returns>
    public async Task<PaginatedResponse<Bill>> ListAsync(
        int page = 1,
        int perPage = 25,
        string? view = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        ContactReference? contact = null,
        long? contactId = null,
        ProjectReference? project = null,
        long? projectId = null,
        bool? nestedBillItems = null,
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

        if (fromDate is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "from_date",
                fromDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        if (toDate is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "to_date",
                toDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
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

        if (nestedBillItems is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "nested_bill_items",
                nestedBillItems.Value ? "true" : "false"));
        }

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("bills", queryParameters);
        var response = await _requestClient.GetWithMetadataAsync<BillsResponse>(endpoint, cancellationToken);

        if (response.Data.Bills is null)
        {
            throw new FreeAgentApiException("Bills data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(
            response,
            page,
            perPage,
            response.Data.Bills.Count);

        return new PaginatedResponse<Bill>(
            page,
            perPage,
            total,
            response.Data.Bills);
    }

    /// <summary>
    /// Lists all bills across all pages.
    /// </summary>
    public async IAsyncEnumerable<Bill> ListAutoPagingAsync(
        int perPage = 25,
        string? view = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        ContactReference? contact = null,
        long? contactId = null,
        ProjectReference? project = null,
        long? projectId = null,
        bool? nestedBillItems = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var billsPage = await ListAsync(
                page,
                perPage,
                view,
                fromDate,
                toDate,
                updatedSince,
                contact,
                contactId,
                project,
                projectId,
                nestedBillItems,
                cancellationToken);

            foreach (var bill in billsPage.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return bill;
            }

            if (!billsPage.HasNextPage)
            {
                yield break;
            }

            page = billsPage.NextPage!.Value;
        }
    }

    /// <summary>
    /// Gets a single bill by identifier.
    /// </summary>
    /// <param name="billId">Bill identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bill details</returns>
    public Task<Bill> GetBillAsync(long billId, CancellationToken cancellationToken = default) =>
        GetBillAsync(billId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single bill by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="billId">Bill identifier from the resource URL</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bill details</returns>
    public async Task<Bill> GetBillAsync(
        long billId,
        BillGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(billId);

        var response = await _requestClient.GetAsync<BillResponse>(
            $"bills/{billId}",
            cancellationToken);

        if (response.Bill is null)
        {
            throw new FreeAgentApiException("Bill data missing from API response");
        }

        await LinkedResourceHydration.HydrateBillingContactAsync(
            response.Bill,
            _requestClient,
            options?.IncludeContact == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateProjectAsync(
            response.Bill,
            _requestClient,
            options?.IncludeProject == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateRebillToProjectAsync(
            response.Bill,
            _requestClient,
            options?.IncludeRebillToProject == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateRebilledOnInvoiceAsync(
            response.Bill,
            _requestClient,
            options?.IncludeRebilledOnInvoice == true,
            cancellationToken);

        return response.Bill;
    }

    /// <summary>
    /// Creates a bill.
    /// </summary>
    /// <param name="bill">Bill attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created bill</returns>
    public async Task<Bill> CreateBillAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(bill);

        var content = FreeAgentJsonSerializer.CreateContent(new BillRequest
        {
            Bill = BillWritePayload.FromBill(bill, _requestClient.Environment)
        });

        var response = await _requestClient.PostAsync<BillResponse>("bills", content, cancellationToken);

        if (response.Bill is null)
        {
            throw new FreeAgentApiException("Bill data missing from API response");
        }

        return response.Bill;
    }

    /// <summary>
    /// Updates a bill.
    /// </summary>
    /// <param name="billId">Bill identifier from the resource URL</param>
    /// <param name="bill">Bill attributes to update</param>
    /// <param name="options">Optional update behaviour</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated bill</returns>
    public async Task<Bill> UpdateBillAsync(
        long billId,
        Bill bill,
        BillUpdateOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(billId);
        ArgumentNullException.ThrowIfNull(bill);

        var content = FreeAgentJsonSerializer.CreateContent(new BillRequest
        {
            Bill = BillWritePayload.FromBill(
                bill,
                _requestClient.Environment,
                options?.OmitLineItems == true,
                LinkedResourceWriteOptions.FromBillUpdate(options),
                isUpdate: true)
        });

        var response = await _requestClient.PutAsync<BillResponse>(
            $"bills/{billId}",
            content,
            cancellationToken);

        if (response.Bill is null)
        {
            throw new FreeAgentApiException("Bill data missing from API response");
        }

        return response.Bill;
    }

    /// <summary>
    /// Deletes a bill.
    /// </summary>
    /// <param name="billId">Bill identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteBillAsync(long billId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(billId);

        return _requestClient.DeleteAsync($"bills/{billId}", cancellationToken);
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
