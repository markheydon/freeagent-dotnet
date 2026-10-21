using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankTransactionExplanations;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Services.BankTransactionExplanations;

/// <summary>
/// Service for interacting with FreeAgent bank transaction explanations.
/// </summary>
/// <remarks>
/// Minimum FreeAgent access level: Banking.
/// List requires a bank account. See also <see cref="BankAccounts.BankAccountsService"/> and
/// <see cref="BankTransactions.BankTransactionsService"/>.
/// </remarks>
public sealed class BankTransactionExplanationsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the bank transaction explanations service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal BankTransactionExplanationsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all bank transaction explanations for a bank account.
    /// </summary>
    /// <param name="bankAccount">Bank account reference. Required when <paramref name="bankAccountId"/> is omitted.</param>
    /// <param name="bankAccountId">Bank account identifier. Required when <paramref name="bankAccount"/> is omitted.</param>
    /// <param name="fromDate">Optional inclusive start date.</param>
    /// <param name="toDate">Optional inclusive end date.</param>
    /// <param name="updatedSince">Optional timestamp filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All explanations returned by FreeAgent for the bank account.</returns>
    public async Task<IReadOnlyList<BankTransactionExplanation>> ListAsync(
        BankAccountReference? bankAccount = null,
        long? bankAccountId = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        CancellationToken cancellationToken = default)
    {
        var queryParameters = new List<KeyValuePair<string, string>>
        {
            new("bank_account", ResolveRequiredBankAccount(bankAccount, bankAccountId))
        };

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

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("bank_transaction_explanations", queryParameters);
        var response = await _requestClient.GetAsync<BankTransactionExplanationsListResponse>(endpoint, cancellationToken);

        if (response.BankTransactionExplanations is null)
        {
            throw new FreeAgentApiException("Bank transaction explanations data missing from API response");
        }

        return response.BankTransactionExplanations;
    }

    /// <summary>
    /// Gets a single bank transaction explanation by identifier.
    /// </summary>
    /// <param name="bankTransactionExplanationId">Explanation identifier from the resource URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bank transaction explanation details.</returns>
    public Task<BankTransactionExplanation> GetBankTransactionExplanationAsync(
        long bankTransactionExplanationId,
        CancellationToken cancellationToken = default) =>
        GetBankTransactionExplanationAsync(bankTransactionExplanationId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single bank transaction explanation by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="bankTransactionExplanationId">Explanation identifier from the resource URL.</param>
    /// <param name="options">Optional hydration settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bank transaction explanation details.</returns>
    public async Task<BankTransactionExplanation> GetBankTransactionExplanationAsync(
        long bankTransactionExplanationId,
        BankTransactionExplanationGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionExplanationId);

        var response = await _requestClient.GetAsync<BankTransactionExplanationResponse>(
            $"bank_transaction_explanations/{bankTransactionExplanationId}",
            cancellationToken);

        if (response.ResolvePayload() is not { } explanation)
        {
            throw new FreeAgentApiException("Bank transaction explanation data missing from API response");
        }

        await LinkedResourceHydration.HydrateBankTransactionExplanationAsync(
            explanation,
            _requestClient,
            options,
            cancellationToken);

        return explanation;
    }

    /// <summary>
    /// Creates a category-based bank transaction explanation.
    /// </summary>
    public Task<BankTransactionExplanation> CreateCategoryBankTransactionExplanationAsync(
        CreateCategoryBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromCategoryCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Creates a payment or refund bank transaction explanation with project rebilling.
    /// </summary>
    public Task<BankTransactionExplanation> CreatePaymentOrRefundBankTransactionExplanationAsync(
        CreatePaymentOrRefundBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromPaymentOrRefundCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Creates an invoice receipt or credit note refund bank transaction explanation.
    /// </summary>
    public Task<BankTransactionExplanation> CreateInvoiceReceiptBankTransactionExplanationAsync(
        CreateInvoiceReceiptBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromInvoiceReceiptCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Creates a bill payment or bill refund bank transaction explanation.
    /// </summary>
    public Task<BankTransactionExplanation> CreateBillPaymentBankTransactionExplanationAsync(
        CreateBillPaymentBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromBillPaymentCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Creates a smart user payment bank transaction explanation (paid user without category).
    /// </summary>
    public Task<BankTransactionExplanation> CreateSmartUserPaymentBankTransactionExplanationAsync(
        CreateSmartUserPaymentBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromSmartUserPaymentCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Creates a categorised user payment bank transaction explanation.
    /// </summary>
    public Task<BankTransactionExplanation> CreateCategorisedUserPaymentBankTransactionExplanationAsync(
        CreateCategorisedUserPaymentBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromCategorisedUserPaymentCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Creates a transfer bank transaction explanation between bank accounts.
    /// </summary>
    public Task<BankTransactionExplanation> CreateTransferBankTransactionExplanationAsync(
        CreateTransferBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromTransferCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Creates a stock purchase or sale bank transaction explanation.
    /// </summary>
    public Task<BankTransactionExplanation> CreateStockBankTransactionExplanationAsync(
        CreateStockBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromStockCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Creates a capital asset purchase bank transaction explanation.
    /// </summary>
    public Task<BankTransactionExplanation> CreateCapitalAssetPurchaseBankTransactionExplanationAsync(
        CreateCapitalAssetPurchaseBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromCapitalAssetPurchaseCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Creates a capital asset disposal bank transaction explanation.
    /// </summary>
    public Task<BankTransactionExplanation> CreateCapitalAssetDisposalBankTransactionExplanationAsync(
        CreateCapitalAssetDisposalBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromCapitalAssetDisposalCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Creates a landlord property-linked bank transaction explanation.
    /// </summary>
    public Task<BankTransactionExplanation> CreateLandlordPropertyBankTransactionExplanationAsync(
        CreateLandlordPropertyBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromLandlordPropertyCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Creates an opening balance debtor or creditor bank transaction explanation.
    /// </summary>
    public Task<BankTransactionExplanation> CreateOpeningBalanceBankTransactionExplanationAsync(
        CreateOpeningBalanceBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default) =>
        CreateAsync(
            BankTransactionExplanationWritePayloadMapper.FromOpeningBalanceCreate(request, _requestClient.Environment),
            cancellationToken);

    /// <summary>
    /// Updates a bank transaction explanation.
    /// </summary>
    /// <param name="bankTransactionExplanationId">Explanation identifier from the resource URL.</param>
    /// <param name="request">Attributes to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated bank transaction explanation.</returns>
    public async Task<BankTransactionExplanation> UpdateBankTransactionExplanationAsync(
        long bankTransactionExplanationId,
        UpdateBankTransactionExplanationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionExplanationId);
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new BankTransactionExplanationRequest
        {
            BankTransactionExplanation = BankTransactionExplanationWritePayload.FromUpdate(request, _requestClient.Environment)
        });

        var response = await _requestClient.PutAsync<BankTransactionExplanationResponse>(
            $"bank_transaction_explanations/{bankTransactionExplanationId}",
            content,
            cancellationToken);

        if (response.ResolvePayload() is not { } explanation)
        {
            throw new FreeAgentApiException("Bank transaction explanation data missing from API response");
        }

        return explanation;
    }

    /// <summary>
    /// Deletes a bank transaction explanation.
    /// </summary>
    /// <param name="bankTransactionExplanationId">Explanation identifier from the resource URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task DeleteBankTransactionExplanationAsync(
        long bankTransactionExplanationId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionExplanationId);

        return _requestClient.DeleteAsync(
            $"bank_transaction_explanations/{bankTransactionExplanationId}",
            cancellationToken);
    }

    private async Task<BankTransactionExplanation> CreateAsync(
        BankTransactionExplanationWritePayload payload,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(payload);

        var content = FreeAgentJsonSerializer.CreateContent(new BankTransactionExplanationRequest
        {
            BankTransactionExplanation = payload
        });

        var response = await _requestClient.PostAsync<BankTransactionExplanationResponse>(
            "bank_transaction_explanations",
            content,
            cancellationToken);

        if (response.ResolvePayload() is not { } explanation)
        {
            throw new FreeAgentApiException("Bank transaction explanation data missing from API response");
        }

        return explanation;
    }

    private string ResolveRequiredBankAccount(BankAccountReference? bankAccount, long? bankAccountId)
    {
        if (bankAccount is not null && bankAccountId is not null)
        {
            throw new ArgumentException("Specify either bankAccount or bankAccountId, not both.", nameof(bankAccount));
        }

        if (bankAccount is not null)
        {
            return bankAccount.Value.Uri;
        }

        if (bankAccountId is null)
        {
            throw new ArgumentException(
                "A bank account is required. Specify bankAccount or bankAccountId.",
                nameof(bankAccountId));
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankAccountId.Value);
        return BankAccountReference.ForEnvironment(_requestClient.Environment, bankAccountId.Value).Uri;
    }
}
