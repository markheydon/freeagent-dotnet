using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.CreditNoteReconciliations;

namespace FreeAgent.Client.Services.CreditNoteReconciliations;

/// <summary>
/// Service for interacting with FreeAgent credit note reconciliations.
/// </summary>
public sealed class CreditNoteReconciliationsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the credit note reconciliations service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal CreditNoteReconciliationsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all credit note reconciliations for the current company.
    /// </summary>
    /// <param name="updatedSince">Return reconciliations updated since this UTC timestamp.</param>
    /// <param name="fromDate">Return reconciliations dated on or after this date.</param>
    /// <param name="toDate">Return reconciliations dated on or before this date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All credit note reconciliations returned by FreeAgent.</returns>
    public async Task<IReadOnlyList<CreditNoteReconciliation>> ListAsync(
        DateTimeOffset? updatedSince = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var queryParameters = new List<KeyValuePair<string, string>>();

        if (updatedSince is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "updated_since",
                updatedSince.Value.ToString("o", CultureInfo.InvariantCulture)));
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

        var endpoint = queryParameters.Count == 0
            ? "credit_note_reconciliations"
            : FreeAgentQueryStringBuilder.BuildEndpoint("credit_note_reconciliations", queryParameters);

        var response = await _requestClient.GetAsync<CreditNoteReconciliationsResponse>(endpoint, cancellationToken);

        if (response.CreditNoteReconciliations is null)
        {
            throw new FreeAgentApiException("Credit note reconciliations data missing from API response");
        }

        return response.CreditNoteReconciliations;
    }

    /// <summary>
    /// Gets a single credit note reconciliation by identifier.
    /// </summary>
    /// <param name="creditNoteReconciliationId">Credit note reconciliation identifier from the resource URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Credit note reconciliation details.</returns>
    public Task<CreditNoteReconciliation> GetCreditNoteReconciliationAsync(
        long creditNoteReconciliationId,
        CancellationToken cancellationToken = default) =>
        GetCreditNoteReconciliationAsync(creditNoteReconciliationId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single credit note reconciliation by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="creditNoteReconciliationId">Credit note reconciliation identifier from the resource URL.</param>
    /// <param name="options">Optional hydration settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Credit note reconciliation details.</returns>
    public async Task<CreditNoteReconciliation> GetCreditNoteReconciliationAsync(
        long creditNoteReconciliationId,
        CreditNoteReconciliationGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(creditNoteReconciliationId);

        var response = await _requestClient.GetAsync<CreditNoteReconciliationResponse>(
            $"credit_note_reconciliations/{creditNoteReconciliationId}",
            cancellationToken);

        if (response.ResolvePayload() is not { } reconciliation)
        {
            throw new FreeAgentApiException("Credit note reconciliation data missing from API response");
        }

        await LinkedResourceHydration.HydrateInvoiceAsync(
            reconciliation,
            _requestClient,
            options?.IncludeInvoice == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateCreditNoteAsync(
            reconciliation,
            _requestClient,
            options?.IncludeCreditNote == true,
            cancellationToken);

        return reconciliation;
    }

    /// <summary>
    /// Creates a credit note reconciliation.
    /// </summary>
    /// <param name="request">Credit note reconciliation attributes to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created credit note reconciliation.</returns>
    public async Task<CreditNoteReconciliation> CreateCreditNoteReconciliationAsync(
        CreateCreditNoteReconciliationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new CreditNoteReconciliationRequest
        {
            CreditNoteReconciliation = CreditNoteReconciliationWritePayload.FromCreate(request, _requestClient.Environment)
        });

        var response = await _requestClient.PostAsync<CreditNoteReconciliationResponse>(
            "credit_note_reconciliations",
            content,
            cancellationToken);

        if (response.ResolvePayload() is not { } reconciliation)
        {
            throw new FreeAgentApiException("Credit note reconciliation data missing from API response");
        }

        return reconciliation;
    }

    /// <summary>
    /// Updates a credit note reconciliation.
    /// </summary>
    /// <param name="creditNoteReconciliationId">Credit note reconciliation identifier from the resource URL.</param>
    /// <param name="request">Credit note reconciliation attributes to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated credit note reconciliation.</returns>
    public async Task<CreditNoteReconciliation> UpdateCreditNoteReconciliationAsync(
        long creditNoteReconciliationId,
        UpdateCreditNoteReconciliationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(creditNoteReconciliationId);
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new CreditNoteReconciliationRequest
        {
            CreditNoteReconciliation = CreditNoteReconciliationWritePayload.FromUpdate(request, _requestClient.Environment)
        });

        var response = await _requestClient.PutAsync<CreditNoteReconciliationResponse>(
            $"credit_note_reconciliations/{creditNoteReconciliationId}",
            content,
            cancellationToken);

        if (response.ResolvePayload() is not { } reconciliation)
        {
            throw new FreeAgentApiException("Credit note reconciliation data missing from API response");
        }

        return reconciliation;
    }

    /// <summary>
    /// Deletes a credit note reconciliation.
    /// </summary>
    /// <param name="creditNoteReconciliationId">Credit note reconciliation identifier from the resource URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public System.Threading.Tasks.Task DeleteCreditNoteReconciliationAsync(
        long creditNoteReconciliationId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(creditNoteReconciliationId);

        return _requestClient.DeleteAsync(
            $"credit_note_reconciliations/{creditNoteReconciliationId}",
            cancellationToken);
    }
}
