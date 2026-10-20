using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankTransactionExplanations;

namespace FreeAgent.Client.Services.BankTransactionExplanations;

/// <summary>
/// Maps public create requests to internal write payloads.
/// </summary>
internal static class BankTransactionExplanationWritePayloadMapper
{
    public static BankTransactionExplanationWritePayload FromCategoryCreate(
        CreateCategoryBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.Category = LinkedResourceWriteMapper.ToCategoryReference(environment, request.CategoryNominalCode);
        return payload;
    }

    public static BankTransactionExplanationWritePayload FromPaymentOrRefundCreate(
        CreatePaymentOrRefundBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.Project = LinkedResourceWriteMapper.ToProjectReference(environment, request.ProjectId);
        payload.RebillType = request.RebillType;
        payload.RebillFactor = request.RebillFactor;
        payload.ReceiptReference = request.ReceiptReference;
        payload.Category = LinkedResourceWriteMapper.ToCategoryReference(environment, request.CategoryNominalCode);
        return payload;
    }

    public static BankTransactionExplanationWritePayload FromInvoiceReceiptCreate(
        CreateInvoiceReceiptBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.PaidInvoice = LinkedResourceWriteMapper.ToInvoiceReference(environment, request.PaidInvoiceId);
        payload.ForeignCurrencyValue = request.ForeignCurrencyValue;
        return payload;
    }

    public static BankTransactionExplanationWritePayload FromBillPaymentCreate(
        CreateBillPaymentBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.PaidBill = LinkedResourceWriteMapper.ToBillReference(environment, request.PaidBillId);
        payload.ForeignCurrencyValue = request.ForeignCurrencyValue;
        return payload;
    }

    public static BankTransactionExplanationWritePayload FromSmartUserPaymentCreate(
        CreateSmartUserPaymentBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.PaidUser = LinkedResourceWriteMapper.ToUserReference(environment, request.PaidUserId);
        return payload;
    }

    public static BankTransactionExplanationWritePayload FromCategorisedUserPaymentCreate(
        CreateCategorisedUserPaymentBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.PaidUser = LinkedResourceWriteMapper.ToUserReference(environment, request.PaidUserId);
        payload.Category = LinkedResourceWriteMapper.ToCategoryReference(environment, request.CategoryNominalCode);
        return payload;
    }

    public static BankTransactionExplanationWritePayload FromTransferCreate(
        CreateTransferBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.TransferBankAccount = LinkedResourceWriteMapper.ToBankAccountReference(environment, request.TransferBankAccountId);
        return payload;
    }

    public static BankTransactionExplanationWritePayload FromStockCreate(
        CreateStockBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.StockItem = LinkedResourceWriteMapper.ToStockItemReference(environment, request.StockItemId);
        payload.StockAlteringQuantity = request.StockAlteringQuantity;
        payload.Category = LinkedResourceWriteMapper.ToCategoryReference(environment, request.CategoryNominalCode);
        return payload;
    }

    public static BankTransactionExplanationWritePayload FromCapitalAssetPurchaseCreate(
        CreateCapitalAssetPurchaseBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.Category = LinkedResourceWriteMapper.ToCategoryReference(environment, request.CategoryNominalCode);
        payload.AssetLifeYears = request.AssetLifeYears;
        return payload;
    }

    public static BankTransactionExplanationWritePayload FromCapitalAssetDisposalCreate(
        CreateCapitalAssetDisposalBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.DisposedAsset = LinkedResourceWriteMapper.ToCapitalAssetReference(environment, request.DisposedAssetId);
        return payload;
    }

    public static BankTransactionExplanationWritePayload FromLandlordPropertyCreate(
        CreateLandlordPropertyBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.Category = LinkedResourceWriteMapper.ToCategoryReference(environment, request.CategoryNominalCode);
        payload.PropertyUri = request.PropertyUri;
        return payload;
    }

    public static BankTransactionExplanationWritePayload FromOpeningBalanceCreate(
        CreateOpeningBalanceBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        var payload = CreateCommonPayload(request, environment);
        payload.DirectContact = LinkedResourceWriteMapper.ToContactReference(environment, request.DirectContactId);
        payload.Category = LinkedResourceWriteMapper.ToCategoryReference(environment, request.CategoryNominalCode);
        return payload;
    }

    private static BankTransactionExplanationWritePayload CreateCommonPayload(
        BankTransactionExplanationCreateRequestBase request,
        FreeAgentEnvironment environment)
    {
        return new BankTransactionExplanationWritePayload
        {
            BankAccount = LinkedResourceWriteMapper.ToBankAccountReference(environment, request.BankAccountId),
            BankTransaction = LinkedResourceWriteMapper.ToBankTransactionReference(environment, request.BankTransactionId),
            DatedOn = request.DatedOn,
            GrossValue = request.GrossValue,
            Description = request.Description,
            EcStatus = request.EcStatus,
            PlaceOfSupply = request.PlaceOfSupply,
            SalesTaxRate = request.SalesTaxRate,
            SecondSalesTaxRate = request.SecondSalesTaxRate,
            SalesTaxValue = request.SalesTaxValue,
            SecondSalesTaxValue = request.SecondSalesTaxValue,
            SalesTaxStatus = request.SalesTaxStatus,
            SecondSalesTaxStatus = request.SecondSalesTaxStatus,
            ChequeNumber = request.ChequeNumber,
            Attachment = request.Attachment
        };
    }
}
