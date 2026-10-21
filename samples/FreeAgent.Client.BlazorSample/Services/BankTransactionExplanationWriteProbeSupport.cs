using FreeAgent.Client.Models.BankTransactionExplanations;
using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Models.DepreciationProfiles;

namespace FreeAgent.Client.BlazorSample.Services;

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
        const decimal grossValue = -1m;

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
