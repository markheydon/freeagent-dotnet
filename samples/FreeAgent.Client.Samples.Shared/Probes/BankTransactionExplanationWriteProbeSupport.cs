using FreeAgent.Client;

namespace FreeAgent.Client.Samples.Shared.Probes;

/// <summary>
/// Documented bank transaction explanation create variants exposed by the SDK for sample probing.
/// </summary>
public enum BankTransactionExplanationWriteProbeVariant
{
    CreateCategory,
    CreatePaymentOrRefund,
    CreateInvoiceReceipt,
    CreateBillPayment,
    CreateSmartUserPayment,
    CreateCategorisedUserPayment,
    CreateTransfer,
    CreateStock,
    CreateCapitalAssetPurchase,
    CreateCapitalAssetPurchaseWithDepreciationProfile,
    CreateCapitalAssetDisposal,
    CreateLandlordProperty,
    CreateOpeningBalance
}

/// <summary>
/// Input values for explanation write probes.
/// </summary>
public sealed class BankTransactionExplanationWriteProbeContext
{
    public long BankAccountId { get; init; }

    public long BankTransactionId { get; init; }

    public long ProjectId { get; init; }

    public long InvoiceId { get; init; }

    public long BillId { get; init; }

    public long UserId { get; init; }

    public long TransferBankAccountId { get; init; }

    public long StockItemId { get; init; }

    public long ContactId { get; init; }

    public long DisposedAssetId { get; init; }

    public string CategoryNominalCode { get; init; } = "285";

    public string PropertyUri { get; init; } = string.Empty;
}

/// <summary>
/// Executes bank transaction explanation write probes using typed SDK requests.
/// </summary>
public static class BankTransactionExplanationWriteProbeSupport
{
    /// <summary>
    /// Gross value for money-out explanation probes; matches uploaded statement line amounts.
    /// </summary>
    public const decimal ProbeMoneyOutGrossValue = -0.01m;

    /// <summary>
    /// Statement upload amount for money-out bank transaction probes.
    /// </summary>
    public const decimal ProbeMoneyOutStatementAmount = ProbeMoneyOutGrossValue;

    /// <summary>
    /// Gross value for capital asset disposal probes (money in).
    /// </summary>
    public const decimal ProbeDisposalGrossValue = 0.01m;

    /// <summary>
    /// Statement upload amount for disposal bank transaction probes.
    /// </summary>
    public const decimal ProbeDisposalStatementAmount = ProbeDisposalGrossValue;

    /// <summary>
    /// Resolves a bank transaction ID from a <c>last_uploaded</c> list using FIT id or description.
    /// </summary>
    public static long? TryResolveUploadedBankTransactionId(
        IEnumerable<BankTransaction> items,
        string fitId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fitId);

        var created = items.FirstOrDefault(transaction => transaction.TransactionId == fitId)
            ?? items.FirstOrDefault(transaction => transaction.Description == fitId);

        return created?.ResourceId;
    }

    /// <summary>
    /// Returns upload rows that match a probe FIT id (for orphan cleanup when resolution fails).
    /// </summary>
    public static IReadOnlyList<BankTransaction> FindUploadOrphans(
        IEnumerable<BankTransaction> items,
        string fitId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fitId);

        return items
            .Where(
                transaction =>
                    string.Equals(transaction.TransactionId, fitId, StringComparison.Ordinal)
                    || string.Equals(transaction.Description, fitId, StringComparison.Ordinal))
            .ToList();
    }

    /// <summary>
    /// Returns <see langword="false"/> when required probe context is missing for the variant.
    /// </summary>
    public static bool TryValidateCreatePrerequisites(
        BankTransactionExplanationWriteProbeVariant variant,
        BankTransactionExplanationWriteProbeContext context,
        out string? skipReason)
    {
        ArgumentNullException.ThrowIfNull(context);

        skipReason = variant switch
        {
            BankTransactionExplanationWriteProbeVariant.CreateTransfer
                when context.TransferBankAccountId <= 0 =>
                "Skipped: need at least two bank accounts for a transfer probe.",
            BankTransactionExplanationWriteProbeVariant.CreateLandlordProperty
                when string.IsNullOrWhiteSpace(context.PropertyUri) =>
                "Skipped: no property URI available (landlord company type required).",
            BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetDisposal
                when context.DisposedAssetId <= 0 =>
                "Skipped: no disposed capital asset ID (console sample creates a probe asset first).",
            BankTransactionExplanationWriteProbeVariant.CreateInvoiceReceipt
                when context.InvoiceId <= 0 =>
                "Skipped: no invoice available for invoice receipt probe.",
            BankTransactionExplanationWriteProbeVariant.CreateBillPayment
                when context.BillId <= 0 =>
                "Skipped: no bill available for bill payment probe.",
            BankTransactionExplanationWriteProbeVariant.CreateStock
                when context.StockItemId <= 0 =>
                "Skipped: no stock item available for stock probe.",
            BankTransactionExplanationWriteProbeVariant.CreateSmartUserPayment
                or BankTransactionExplanationWriteProbeVariant.CreateCategorisedUserPayment
                when context.UserId <= 0 =>
                "Skipped: no user available for user payment probe.",
            BankTransactionExplanationWriteProbeVariant.CreateOpeningBalance
                when context.ContactId <= 0 =>
                "Skipped: no contact available for opening balance probe.",
            BankTransactionExplanationWriteProbeVariant.CreatePaymentOrRefund
                when context.ProjectId <= 0 =>
                "Skipped: no project available for payment or refund probe.",
            _ => null
        };

        return skipReason is null;
    }

    private static decimal GetProbeGrossValue(BankTransactionExplanationWriteProbeVariant variant) =>
        variant == BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetDisposal
            ? ProbeDisposalGrossValue
            : ProbeMoneyOutGrossValue;

    public static string GetCallUnderTest(BankTransactionExplanationWriteProbeVariant variant) => variant switch
    {
        BankTransactionExplanationWriteProbeVariant.CreateCategory =>
            "BankTransactionExplanationsService.CreateCategoryBankTransactionExplanationAsync",
        BankTransactionExplanationWriteProbeVariant.CreatePaymentOrRefund =>
            "BankTransactionExplanationsService.CreatePaymentOrRefundBankTransactionExplanationAsync",
        BankTransactionExplanationWriteProbeVariant.CreateInvoiceReceipt =>
            "BankTransactionExplanationsService.CreateInvoiceReceiptBankTransactionExplanationAsync",
        BankTransactionExplanationWriteProbeVariant.CreateBillPayment =>
            "BankTransactionExplanationsService.CreateBillPaymentBankTransactionExplanationAsync",
        BankTransactionExplanationWriteProbeVariant.CreateSmartUserPayment =>
            "BankTransactionExplanationsService.CreateSmartUserPaymentBankTransactionExplanationAsync",
        BankTransactionExplanationWriteProbeVariant.CreateCategorisedUserPayment =>
            "BankTransactionExplanationsService.CreateCategorisedUserPaymentBankTransactionExplanationAsync",
        BankTransactionExplanationWriteProbeVariant.CreateTransfer =>
            "BankTransactionExplanationsService.CreateTransferBankTransactionExplanationAsync",
        BankTransactionExplanationWriteProbeVariant.CreateStock =>
            "BankTransactionExplanationsService.CreateStockBankTransactionExplanationAsync",
        BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchase =>
            "BankTransactionExplanationsService.CreateCapitalAssetPurchaseBankTransactionExplanationAsync",
        BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchaseWithDepreciationProfile =>
            "CreateCapitalAssetPurchaseBankTransactionExplanationRequest.CreateWithDepreciationProfile",
        BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetDisposal =>
            "BankTransactionExplanationsService.CreateCapitalAssetDisposalBankTransactionExplanationAsync",
        BankTransactionExplanationWriteProbeVariant.CreateLandlordProperty =>
            "BankTransactionExplanationsService.CreateLandlordPropertyBankTransactionExplanationAsync",
        BankTransactionExplanationWriteProbeVariant.CreateOpeningBalance =>
            "BankTransactionExplanationsService.CreateOpeningBalanceBankTransactionExplanationAsync",
        _ => "BankTransactionExplanationsService"
    };

    public static async Task<BankTransactionExplanation> ExecuteCreateAsync(
        FreeAgentClient client,
        BankTransactionExplanationWriteProbeVariant variant,
        BankTransactionExplanationWriteProbeContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(context);

        var datedOn = DateOnly.FromDateTime(DateTime.UtcNow);
        var grossValue = GetProbeGrossValue(variant);

        return variant switch
        {
            BankTransactionExplanationWriteProbeVariant.CreateCategory =>
                await client.BankTransactionExplanations.CreateCategoryBankTransactionExplanationAsync(
                    CreateCategoryBankTransactionExplanationRequest.ForExistingTransaction(
                        context.BankTransactionId,
                        context.CategoryNominalCode,
                        datedOn,
                        grossValue,
                        description: "SDK probe category explanation"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreatePaymentOrRefund =>
                await client.BankTransactionExplanations.CreatePaymentOrRefundBankTransactionExplanationAsync(
                    CreatePaymentOrRefundBankTransactionExplanationRequest.Create(
                        context.BankTransactionId,
                        context.ProjectId,
                        BillRebillType.Markup,
                        datedOn,
                        grossValue,
                        rebillFactor: 0.25m,
                        categoryNominalCode: context.CategoryNominalCode,
                        description: "SDK probe payment explanation"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreateInvoiceReceipt =>
                await client.BankTransactionExplanations.CreateInvoiceReceiptBankTransactionExplanationAsync(
                    CreateInvoiceReceiptBankTransactionExplanationRequest.Create(
                        context.BankTransactionId,
                        context.InvoiceId,
                        datedOn,
                        grossValue,
                        description: "SDK probe invoice receipt"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreateBillPayment =>
                await client.BankTransactionExplanations.CreateBillPaymentBankTransactionExplanationAsync(
                    CreateBillPaymentBankTransactionExplanationRequest.Create(
                        context.BankTransactionId,
                        context.BillId,
                        datedOn,
                        grossValue,
                        description: "SDK probe bill payment"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreateSmartUserPayment =>
                await client.BankTransactionExplanations.CreateSmartUserPaymentBankTransactionExplanationAsync(
                    CreateSmartUserPaymentBankTransactionExplanationRequest.Create(
                        context.BankTransactionId,
                        context.UserId,
                        datedOn,
                        grossValue,
                        description: "SDK probe smart user payment"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreateCategorisedUserPayment =>
                await client.BankTransactionExplanations.CreateCategorisedUserPaymentBankTransactionExplanationAsync(
                    CreateCategorisedUserPaymentBankTransactionExplanationRequest.Create(
                        context.BankTransactionId,
                        context.UserId,
                        context.CategoryNominalCode,
                        datedOn,
                        grossValue,
                        description: "SDK probe categorised user payment"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreateTransfer =>
                await client.BankTransactionExplanations.CreateTransferBankTransactionExplanationAsync(
                    CreateTransferBankTransactionExplanationRequest.Create(
                        context.BankTransactionId,
                        context.TransferBankAccountId,
                        datedOn,
                        grossValue,
                        description: "SDK probe transfer"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreateStock =>
                await client.BankTransactionExplanations.CreateStockBankTransactionExplanationAsync(
                    CreateStockBankTransactionExplanationRequest.Create(
                        context.BankTransactionId,
                        context.StockItemId,
                        stockAlteringQuantity: 1,
                        datedOn,
                        grossValue,
                        categoryNominalCode: context.CategoryNominalCode,
                        description: "SDK probe stock explanation"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchase =>
                await client.BankTransactionExplanations.CreateCapitalAssetPurchaseBankTransactionExplanationAsync(
                    CreateCapitalAssetPurchaseBankTransactionExplanationRequest.Create(
                        context.BankTransactionId,
                        context.CategoryNominalCode,
                        datedOn,
                        grossValue,
                        description: "SDK probe capital asset purchase"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchaseWithDepreciationProfile =>
                await client.BankTransactionExplanations.CreateCapitalAssetPurchaseBankTransactionExplanationAsync(
                    CreateCapitalAssetPurchaseBankTransactionExplanationRequest.CreateWithDepreciationProfile(
                        context.BankTransactionId,
                        context.CategoryNominalCode,
                        datedOn,
                        grossValue,
                        CapitalAssetPurchaseWrite.StraightLine(10, DepreciationFrequency.Annually),
                        description: "SDK probe capital asset purchase with depreciation profile"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetDisposal =>
                await client.BankTransactionExplanations.CreateCapitalAssetDisposalBankTransactionExplanationAsync(
                    CreateCapitalAssetDisposalBankTransactionExplanationRequest.Create(
                        context.BankTransactionId,
                        context.DisposedAssetId,
                        datedOn,
                        grossValue,
                        description: "SDK probe capital asset disposal"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreateLandlordProperty =>
                await client.BankTransactionExplanations.CreateLandlordPropertyBankTransactionExplanationAsync(
                    CreateLandlordPropertyBankTransactionExplanationRequest.Create(
                        context.BankTransactionId,
                        context.CategoryNominalCode,
                        context.PropertyUri,
                        datedOn,
                        grossValue,
                        description: "SDK probe landlord property explanation"),
                    cancellationToken),
            BankTransactionExplanationWriteProbeVariant.CreateOpeningBalance =>
                await client.BankTransactionExplanations.CreateOpeningBalanceBankTransactionExplanationAsync(
                    CreateOpeningBalanceBankTransactionExplanationRequest.Create(
                        context.BankTransactionId,
                        context.ContactId,
                        context.CategoryNominalCode,
                        datedOn,
                        grossValue,
                        description: "SDK probe opening balance explanation"),
                    cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, "Unsupported write probe variant.")
        };
    }
}
