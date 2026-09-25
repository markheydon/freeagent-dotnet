using System.Globalization;
using System.Text;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.CreditNotes;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Services.Invoices;

/// <summary>
/// Service for interacting with FreeAgent invoices.
/// </summary>
public sealed class InvoiceService
{
    private static readonly StringContent EmptyJsonContent =
        new("{}", Encoding.UTF8, "application/json");

    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the invoice service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal InvoiceService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists one page of invoices.
    /// </summary>
    /// <param name="page">1-based page number</param>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Optional view filter (for example: <see cref="InvoiceViews.Open"/>)</param>
    /// <param name="sort">Sort field (<see cref="InvoiceSortOptions.CreatedAt"/> or <see cref="InvoiceSortOptions.UpdatedAt"/>); prefix with <c>-</c> for descending</param>
    /// <param name="updatedSince">Return invoices updated since this UTC timestamp</param>
    /// <param name="contact">Filter by contact resource reference</param>
    /// <param name="contactId">Filter by contact identifier (equivalent to <c>client.Urls.Contact(contactId)</c>)</param>
    /// <param name="project">Filter by project resource reference</param>
    /// <param name="projectId">Filter by project identifier (equivalent to <c>client.Urls.Project(projectId)</c>)</param>
    /// <param name="nestedInvoiceItems">When <see langword="true"/>, include invoice items nested in each invoice</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated invoices response</returns>
    public async Task<PaginatedResponse<Invoice>> ListAsync(
        int page = 1,
        int perPage = 25,
        string? view = null,
        string? sort = null,
        DateTimeOffset? updatedSince = null,
        ContactReference? contact = null,
        long? contactId = null,
        ProjectReference? project = null,
        long? projectId = null,
        bool? nestedInvoiceItems = null,
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

        if (nestedInvoiceItems is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "nested_invoice_items",
                nestedInvoiceItems.Value ? "true" : "false"));
        }

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("invoices", queryParameters);
        var response = await _requestClient.GetWithMetadataAsync<InvoicesResponse>(endpoint, cancellationToken);

        if (response.Data.Invoices is null)
        {
            throw new FreeAgentApiException("Invoices data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(
            response,
            page,
            perPage,
            response.Data.Invoices.Count);

        return new PaginatedResponse<Invoice>(
            page,
            perPage,
            total,
            response.Data.Invoices);
    }

    /// <summary>
    /// Lists all invoices across all pages.
    /// </summary>
    public async IAsyncEnumerable<Invoice> ListAutoPagingAsync(
        int perPage = 25,
        string? view = null,
        string? sort = null,
        DateTimeOffset? updatedSince = null,
        ContactReference? contact = null,
        long? contactId = null,
        ProjectReference? project = null,
        long? projectId = null,
        bool? nestedInvoiceItems = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var invoicesPage = await ListAsync(
                page,
                perPage,
                view,
                sort,
                updatedSince,
                contact,
                contactId,
                project,
                projectId,
                nestedInvoiceItems,
                cancellationToken);

            foreach (var invoice in invoicesPage.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return invoice;
            }

            if (!invoicesPage.HasNextPage)
            {
                yield break;
            }

            page = invoicesPage.NextPage!.Value;
        }
    }

    /// <summary>
    /// Gets a single invoice by identifier.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Invoice details</returns>
    public Task<Invoice> GetInvoiceAsync(long invoiceId, CancellationToken cancellationToken = default) =>
        GetInvoiceAsync(invoiceId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single invoice by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Invoice details</returns>
    public async Task<Invoice> GetInvoiceAsync(
        long invoiceId,
        InvoiceGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(invoiceId);

        var response = await _requestClient.GetAsync<InvoiceResponse>($"invoices/{invoiceId}", cancellationToken);

        if (response.Invoice is null)
        {
            throw new FreeAgentApiException("Invoice data missing from API response");
        }

        await LinkedResourceHydration.HydrateBillingContactAsync(
            response.Invoice,
            _requestClient,
            options?.IncludeContact == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateProjectAsync(
            response.Invoice,
            _requestClient,
            options?.IncludeProject == true,
            cancellationToken);

        return response.Invoice;
    }

    /// <summary>
    /// Downloads an invoice PDF.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Decoded PDF bytes</returns>
    public async Task<byte[]> GetInvoicePdfAsync(long invoiceId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(invoiceId);

        var response = await _requestClient.GetAsync<InvoicePdfResponse>($"invoices/{invoiceId}/pdf", cancellationToken);

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
    /// Creates an invoice.
    /// </summary>
    /// <param name="invoice">Invoice attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created invoice</returns>
    /// <remarks>
    /// <see cref="Invoice.ShowProjectName"/> is included in the create payload when set.
    /// </remarks>
    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        var content = FreeAgentJsonSerializer.CreateContent(new InvoiceRequest
        {
            Invoice = InvoiceWritePayload.FromInvoice(
                invoice,
                _requestClient.Environment,
                includeShowProjectName: true)
        });

        var response = await _requestClient.PostAsync<InvoiceResponse>("invoices", content, cancellationToken);

        if (response.Invoice is null)
        {
            throw new FreeAgentApiException("Invoice data missing from API response");
        }

        return response.Invoice;
    }

    /// <summary>
    /// Duplicates an invoice.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Duplicated invoice</returns>
    public async Task<Invoice> DuplicateInvoiceAsync(long invoiceId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(invoiceId);

        var response = await _requestClient.PostAsync<InvoiceResponse>(
            $"invoices/{invoiceId}/duplicate",
            EmptyJsonContent,
            cancellationToken);

        if (response.Invoice is null)
        {
            throw new FreeAgentApiException("Invoice data missing from API response");
        }

        return response.Invoice;
    }

    /// <summary>
    /// Updates an invoice.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="invoice">Invoice attributes to update</param>
    /// <param name="options">Optional update behaviour</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated invoice</returns>
    /// <remarks>
    /// <see cref="Invoice.ShowProjectName"/> is sent on update only when
    /// <see cref="Invoice.Status"/> is <see cref="InvoiceStatus.Draft"/>. FreeAgent locks
    /// <c>show_project_name</c> once an invoice leaves draft.
    /// </remarks>
    public async Task<Invoice> UpdateInvoiceAsync(
        long invoiceId,
        Invoice invoice,
        InvoiceUpdateOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(invoiceId);
        ArgumentNullException.ThrowIfNull(invoice);

        var content = FreeAgentJsonSerializer.CreateContent(new InvoiceRequest
        {
            Invoice = InvoiceWritePayload.FromInvoice(
                invoice,
                _requestClient.Environment,
                options?.OmitLineItems == true,
                LinkedResourceWriteOptions.FromInvoiceUpdate(options),
                includeShowProjectName: invoice.Status == InvoiceStatus.Draft)
        });

        var response = await _requestClient.PutAsync<InvoiceResponse>($"invoices/{invoiceId}", content, cancellationToken);

        if (response.Invoice is null)
        {
            throw new FreeAgentApiException("Invoice data missing from API response");
        }

        return response.Invoice;
    }

    /// <summary>
    /// Deletes an invoice.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteInvoiceAsync(long invoiceId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(invoiceId);

        return _requestClient.DeleteAsync($"invoices/{invoiceId}", cancellationToken);
    }

    /// <summary>
    /// Sends an invoice by email.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="request">Email attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task SendInvoiceEmailAsync(
        long invoiceId,
        SendInvoiceEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(invoiceId);
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new SendInvoiceEmailRequestEnvelope
        {
            Invoice = request
        });

        return _requestClient.SendPostAsync($"invoices/{invoiceId}/send_email", content, cancellationToken);
    }

    /// <summary>
    /// Marks an invoice as sent.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated invoice</returns>
    public Task<Invoice> MarkInvoiceAsSentAsync(long invoiceId, CancellationToken cancellationToken = default) =>
        TransitionInvoiceAsync(invoiceId, "mark_as_sent", cancellationToken);

    /// <summary>
    /// Marks an invoice as scheduled.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated invoice</returns>
    public Task<Invoice> MarkInvoiceAsScheduledAsync(long invoiceId, CancellationToken cancellationToken = default) =>
        TransitionInvoiceAsync(invoiceId, "mark_as_scheduled", cancellationToken);

    /// <summary>
    /// Marks an invoice as draft.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated invoice</returns>
    public Task<Invoice> MarkInvoiceAsDraftAsync(long invoiceId, CancellationToken cancellationToken = default) =>
        TransitionInvoiceAsync(invoiceId, "mark_as_draft", cancellationToken);

    /// <summary>
    /// Marks an invoice as cancelled.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated invoice</returns>
    public Task<Invoice> MarkInvoiceAsCancelledAsync(long invoiceId, CancellationToken cancellationToken = default) =>
        TransitionInvoiceAsync(invoiceId, "mark_as_cancelled", cancellationToken);

    /// <summary>
    /// Takes payment using a GoCardless Direct Debit Mandate.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task TakeDirectDebitPaymentAsync(long invoiceId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(invoiceId);

        return _requestClient.SendPostAsync(
            $"invoices/{invoiceId}/direct_debit",
            EmptyJsonContent,
            cancellationToken);
    }

    /// <summary>
    /// Lists company invoice timeline items.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Invoice timeline items</returns>
    public async Task<IReadOnlyList<InvoiceTimelineItem>> ListTimelineAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<InvoiceTimelineResponse>("invoices/timeline", cancellationToken);

        if (response.InvoiceTimelineItems is null)
        {
            throw new FreeAgentApiException("Invoice timeline data missing from API response");
        }

        return response.InvoiceTimelineItems;
    }

    /// <summary>
    /// Converts a draft negative invoice to a credit note.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Converted credit note</returns>
    public async Task<CreditNote> ConvertToCreditNoteAsync(long invoiceId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(invoiceId);

        var response = await _requestClient.PutAsync<Models.CreditNotes.CreditNoteResponse>(
            $"invoices/{invoiceId}/transitions/convert_to_credit_note",
            EmptyJsonContent,
            cancellationToken);

        if (response.CreditNote is null)
        {
            throw new FreeAgentApiException("Credit note data missing from API response");
        }

        return response.CreditNote;
    }

    /// <summary>
    /// Gets the company default additional text shown on invoices.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Default additional text, or <see langword="null"/> when unset</returns>
    public async Task<string?> GetDefaultAdditionalTextAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<DefaultAdditionalTextResponse>(
            "invoices/default_additional_text",
            cancellationToken);

        return response.DefaultAdditionalText;
    }

    /// <summary>
    /// Updates the company default additional text shown on invoices.
    /// </summary>
    /// <param name="defaultAdditionalText">Additional text to show on all invoices</param>
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
            "invoices/default_additional_text",
            content,
            cancellationToken);

        return response.DefaultAdditionalText;
    }

    /// <summary>
    /// Deletes the company default additional text shown on invoices.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteDefaultAdditionalTextAsync(CancellationToken cancellationToken = default) =>
        _requestClient.DeleteAsync("invoices/default_additional_text", cancellationToken);

    private async Task<Invoice> TransitionInvoiceAsync(
        long invoiceId,
        string transition,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(invoiceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(transition);

        await _requestClient.SendPutAsync(
            $"invoices/{invoiceId}/transitions/{transition}",
            EmptyJsonContent,
            cancellationToken);

        return await GetInvoiceAsync(invoiceId, cancellationToken: cancellationToken);
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
