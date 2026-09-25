using System.Globalization;
using System.Text;
using System.Text.Json;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Estimates;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Services.Estimates;

/// <summary>
/// Service for interacting with FreeAgent estimates.
/// </summary>
public sealed class EstimateService
{
    private static readonly StringContent EmptyJsonContent =
        new("{}", Encoding.UTF8, "application/json");

    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the estimate service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal EstimateService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists one page of estimates.
    /// </summary>
    /// <param name="page">1-based page number</param>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Optional view filter (for example: <see cref="EstimateViews.Draft"/>)</param>
    /// <param name="fromDate">Return estimates dated on or after this date</param>
    /// <param name="toDate">Return estimates dated on or before this date</param>
    /// <param name="updatedSince">Return estimates updated since this UTC timestamp</param>
    /// <param name="contact">Filter by contact resource reference</param>
    /// <param name="contactId">Filter by contact identifier (equivalent to <c>client.Urls.Contact(contactId)</c>)</param>
    /// <param name="project">Filter by project resource reference</param>
    /// <param name="projectId">Filter by project identifier (equivalent to <c>client.Urls.Project(projectId)</c>)</param>
    /// <param name="invoice">Filter by invoice resource reference</param>
    /// <param name="invoiceId">Filter by invoice identifier (equivalent to <c>client.Urls.Invoice(invoiceId)</c>)</param>
    /// <param name="nestedEstimateItems">When <see langword="true"/>, include estimate items nested in each estimate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated estimates response</returns>
    public async Task<PaginatedResponse<Estimate>> ListAsync(
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
        InvoiceReference? invoice = null,
        long? invoiceId = null,
        bool? nestedEstimateItems = null,
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

        var invoiceFilter = ResolveInvoiceFilter(invoice, invoiceId);
        if (invoiceFilter is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("invoice", invoiceFilter));
        }

        if (nestedEstimateItems is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "nested_estimate_items",
                nestedEstimateItems.Value ? "true" : "false"));
        }

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("estimates", queryParameters);
        var response = await _requestClient.GetWithMetadataAsync<EstimatesResponse>(endpoint, cancellationToken);

        if (response.Data.Estimates is null)
        {
            throw new FreeAgentApiException("Estimates data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(
            response,
            page,
            perPage,
            response.Data.Estimates.Count);

        return new PaginatedResponse<Estimate>(
            page,
            perPage,
            total,
            response.Data.Estimates);
    }

    /// <summary>
    /// Lists all estimates across all pages.
    /// </summary>
    public async IAsyncEnumerable<Estimate> ListAutoPagingAsync(
        int perPage = 25,
        string? view = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        ContactReference? contact = null,
        long? contactId = null,
        ProjectReference? project = null,
        long? projectId = null,
        InvoiceReference? invoice = null,
        long? invoiceId = null,
        bool? nestedEstimateItems = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var estimatesPage = await ListAsync(
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
                invoice,
                invoiceId,
                nestedEstimateItems,
                cancellationToken);

            foreach (var estimate in estimatesPage.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return estimate;
            }

            if (!estimatesPage.HasNextPage)
            {
                yield break;
            }

            page = estimatesPage.NextPage!.Value;
        }
    }

    /// <summary>
    /// Gets a single estimate by identifier.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Estimate details</returns>
    public Task<Estimate> GetEstimateAsync(long estimateId, CancellationToken cancellationToken = default) =>
        GetEstimateAsync(estimateId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single estimate by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Estimate details</returns>
    public async Task<Estimate> GetEstimateAsync(
        long estimateId,
        EstimateGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimateId);

        var response = await _requestClient.GetAsync<EstimateResponse>($"estimates/{estimateId}", cancellationToken);

        if (response.Estimate is null)
        {
            throw new FreeAgentApiException("Estimate data missing from API response");
        }

        await LinkedResourceHydration.HydrateBillingContactAsync(
            response.Estimate,
            _requestClient,
            options?.IncludeContact == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateProjectAsync(
            response.Estimate,
            _requestClient,
            options?.IncludeProject == true,
            cancellationToken);

        return response.Estimate;
    }

    /// <summary>
    /// Downloads an estimate PDF.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Decoded PDF bytes</returns>
    public async Task<byte[]> GetEstimatePdfAsync(long estimateId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimateId);

        var response = await _requestClient.GetAsync<EstimatePdfResponse>($"estimates/{estimateId}/pdf", cancellationToken);

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
    /// Creates an estimate.
    /// </summary>
    /// <param name="estimate">Estimate attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created estimate</returns>
    /// <remarks>
    /// FreeAgent requires <c>status</c> on create. The SDK always sends <see cref="EstimateStatus.Draft"/>
    /// regardless of <see cref="Estimate.Status"/> on the input model. Use transition methods to change
    /// status after create.
    /// </remarks>
    public async Task<Estimate> CreateEstimateAsync(Estimate estimate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(estimate);

        var content = FreeAgentJsonSerializer.CreateContent(new EstimateRequest
        {
            Estimate = EstimateWritePayload.FromEstimate(
                estimate,
                _requestClient.Environment,
                includeStatus: true)
        });

        var response = await _requestClient.PostAsync<EstimateResponse>("estimates", content, cancellationToken);

        if (response.Estimate is null)
        {
            throw new FreeAgentApiException("Estimate data missing from API response");
        }

        return response.Estimate;
    }

    /// <summary>
    /// Updates an estimate.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="estimate">Estimate attributes to update</param>
    /// <param name="options">Optional update behaviour</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated estimate</returns>
    public async Task<Estimate> UpdateEstimateAsync(
        long estimateId,
        Estimate estimate,
        EstimateUpdateOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimateId);
        ArgumentNullException.ThrowIfNull(estimate);

        var content = FreeAgentJsonSerializer.CreateContent(new EstimateRequest
        {
            Estimate = EstimateWritePayload.FromEstimate(
                estimate,
                _requestClient.Environment,
                options?.OmitLineItems == true,
                LinkedResourceWriteOptions.FromEstimateUpdate(options))
        });

        var response = await _requestClient.PutAsync<EstimateResponse>($"estimates/{estimateId}", content, cancellationToken);

        if (response.Estimate is null)
        {
            throw new FreeAgentApiException("Estimate data missing from API response");
        }

        return response.Estimate;
    }

    /// <summary>
    /// Duplicates an estimate.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Duplicated estimate</returns>
    public async Task<Estimate> DuplicateEstimateAsync(long estimateId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimateId);

        var response = await _requestClient.PostAsync<EstimateResponse>(
            $"estimates/{estimateId}/duplicate",
            EmptyJsonContent,
            cancellationToken);

        if (response.Estimate is null)
        {
            throw new FreeAgentApiException("Estimate data missing from API response");
        }

        return response.Estimate;
    }

    /// <summary>
    /// Deletes an estimate.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteEstimateAsync(long estimateId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimateId);

        return _requestClient.DeleteAsync($"estimates/{estimateId}", cancellationToken);
    }

    /// <summary>
    /// Creates an estimate item.
    /// </summary>
    /// <param name="estimateId">Estimate identifier to add the item to</param>
    /// <param name="estimateItem">Estimate item attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created estimate item</returns>
    public async Task<EstimateItem> CreateEstimateItemAsync(
        long estimateId,
        EstimateItem estimateItem,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimateId);
        ArgumentNullException.ThrowIfNull(estimateItem);

        var content = FreeAgentJsonSerializer.CreateContent(new CreateEstimateItemRequest
        {
            Estimate = EstimateReference.ForEnvironment(_requestClient.Environment, estimateId),
            EstimateItem = EstimateItemWritePayload.FromEstimateItem(estimateItem, _requestClient.Environment)
        });

        var response = await _requestClient.PostAsync<EstimateItemResponse>("estimate_items", content, cancellationToken);

        if (response.EstimateItem is null)
        {
            throw new FreeAgentApiException("Estimate item data missing from API response");
        }

        return response.EstimateItem;
    }

    /// <summary>
    /// Updates an estimate item.
    /// </summary>
    /// <param name="estimateItemId">Estimate item identifier from the resource URL</param>
    /// <param name="estimateItem">Estimate item attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated estimate item</returns>
    public async Task<EstimateItem> UpdateEstimateItemAsync(
        long estimateItemId,
        EstimateItem estimateItem,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimateItemId);
        ArgumentNullException.ThrowIfNull(estimateItem);

        var content = FreeAgentJsonSerializer.CreateContent(new EstimateItemRequest
        {
            EstimateItem = EstimateItemWritePayload.FromEstimateItem(estimateItem, _requestClient.Environment)
        });

        var response = await _requestClient.PutAsync<EstimateItemResponse>(
            $"estimate_items/{estimateItemId}",
            content,
            cancellationToken);

        if (response.EstimateItem is null)
        {
            throw new FreeAgentApiException("Estimate item data missing from API response");
        }

        return response.EstimateItem;
    }

    /// <summary>
    /// Deletes an estimate item.
    /// </summary>
    /// <param name="estimateItemId">Estimate item identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteEstimateItemAsync(long estimateItemId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimateItemId);

        return _requestClient.DeleteAsync($"estimate_items/{estimateItemId}", cancellationToken);
    }

    /// <summary>
    /// Sends an estimate by email.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="request">Email attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task SendEstimateEmailAsync(
        long estimateId,
        SendEstimateEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimateId);
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new SendEstimateEmailRequestEnvelope
        {
            Estimate = request
        });

        return _requestClient.SendPostAsync($"estimates/{estimateId}/send_email", content, cancellationToken);
    }

    /// <summary>
    /// Marks an estimate as sent.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated estimate</returns>
    public Task<Estimate> MarkEstimateAsSentAsync(long estimateId, CancellationToken cancellationToken = default) =>
        TransitionEstimateAsync(estimateId, "mark_as_sent", cancellationToken);

    /// <summary>
    /// Marks an estimate as draft.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated estimate</returns>
    public Task<Estimate> MarkEstimateAsDraftAsync(long estimateId, CancellationToken cancellationToken = default) =>
        TransitionEstimateAsync(estimateId, "mark_as_draft", cancellationToken);

    /// <summary>
    /// Marks an estimate as approved.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated estimate</returns>
    public Task<Estimate> MarkEstimateAsApprovedAsync(long estimateId, CancellationToken cancellationToken = default) =>
        TransitionEstimateAsync(estimateId, "mark_as_approved", cancellationToken);

    /// <summary>
    /// Marks an estimate as rejected.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated estimate</returns>
    public Task<Estimate> MarkEstimateAsRejectedAsync(long estimateId, CancellationToken cancellationToken = default) =>
        TransitionEstimateAsync(estimateId, "mark_as_rejected", cancellationToken);

    /// <summary>
    /// Converts an estimate to an invoice.
    /// </summary>
    /// <param name="estimateId">Estimate identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated estimate with invoice link</returns>
    public async Task<Estimate> ConvertToInvoiceAsync(long estimateId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimateId);

        try
        {
            var response = await _requestClient.PutAsync<EstimateResponse>(
                $"estimates/{estimateId}/transitions/convert_to_invoice",
                EmptyJsonContent,
                cancellationToken);

            if (response.Estimate is not null)
            {
                return response.Estimate;
            }
        }
        catch (JsonException)
        {
            // Some environments return an empty transition body; fetch the updated estimate below.
        }

        return await GetEstimateAsync(estimateId, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Gets the company default additional text shown on estimates.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Default additional text, or <see langword="null"/> when unset</returns>
    public async Task<string?> GetDefaultAdditionalTextAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<DefaultAdditionalTextResponse>(
            "estimates/default_additional_text",
            cancellationToken);

        return response.DefaultAdditionalText;
    }

    /// <summary>
    /// Updates the company default additional text shown on estimates.
    /// </summary>
    /// <param name="defaultAdditionalText">Additional text to show on all estimates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated default additional text</returns>
    public async Task<string?> UpdateDefaultAdditionalTextAsync(
        string defaultAdditionalText,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(defaultAdditionalText);

        var content = FreeAgentJsonSerializer.CreateContent(new DefaultAdditionalTextRequest
        {
            DefaultAdditionalText = defaultAdditionalText
        });

        var response = await _requestClient.PutAsync<DefaultAdditionalTextResponse>(
            "estimates/default_additional_text",
            content,
            cancellationToken);

        return response.DefaultAdditionalText;
    }

    /// <summary>
    /// Deletes the company default additional text shown on estimates.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteDefaultAdditionalTextAsync(CancellationToken cancellationToken = default) =>
        _requestClient.DeleteAsync("estimates/default_additional_text", cancellationToken);

    private async Task<Estimate> TransitionEstimateAsync(
        long estimateId,
        string transition,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimateId);
        ArgumentException.ThrowIfNullOrWhiteSpace(transition);

        await _requestClient.SendPutAsync(
            $"estimates/{estimateId}/transitions/{transition}",
            EmptyJsonContent,
            cancellationToken);

        return await GetEstimateAsync(estimateId, cancellationToken: cancellationToken);
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

    private string? ResolveInvoiceFilter(InvoiceReference? invoice, long? invoiceId)
    {
        if (invoice is not null && invoiceId is not null)
        {
            throw new ArgumentException("Specify either invoice or invoiceId, not both.", nameof(invoice));
        }

        if (invoice is not null)
        {
            return invoice.Value.Uri;
        }

        if (invoiceId is null)
        {
            return null;
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(invoiceId.Value);
        return InvoiceReference.ForEnvironment(_requestClient.Environment, invoiceId.Value).Uri;
    }
}
