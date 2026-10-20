using FreeAgent.Client.Models.Bills;

namespace FreeAgent.Client.Models.BankTransactionExplanations;

/// <summary>
/// Request attributes for creating a category-based bank transaction explanation.
/// </summary>
public sealed class CreateCategoryBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal string CategoryNominalCode { get; private init; } = string.Empty;

    /// <summary>
    /// Creates a category-based explanation against an existing bank transaction.
    /// </summary>
    public static CreateCategoryBankTransactionExplanationRequest ForExistingTransaction(
        long bankTransactionId,
        string categoryNominalCode,
        DateOnly datedOn,
        decimal grossValue,
        string? description = null,
        BankTransactionExplanationEcStatus? ecStatus = null,
        string? placeOfSupply = null,
        decimal? salesTaxRate = null,
        BillSalesTaxStatus? salesTaxStatus = null,
        BankTransactionExplanationAttachmentWritePayload? attachment = null) =>
        Create(bankAccountId: null, bankTransactionId, categoryNominalCode, datedOn, grossValue, description, ecStatus, placeOfSupply, salesTaxRate, salesTaxStatus, attachment);

    /// <summary>
    /// Creates a category-based explanation and matching bank transaction on the given bank account.
    /// </summary>
    public static CreateCategoryBankTransactionExplanationRequest WithNewTransaction(
        long bankAccountId,
        string categoryNominalCode,
        DateOnly datedOn,
        decimal grossValue,
        string? description = null,
        BankTransactionExplanationEcStatus? ecStatus = null,
        string? placeOfSupply = null,
        decimal? salesTaxRate = null,
        BillSalesTaxStatus? salesTaxStatus = null,
        BankTransactionExplanationAttachmentWritePayload? attachment = null) =>
        Create(bankAccountId, bankTransactionId: null, categoryNominalCode, datedOn, grossValue, description, ecStatus, placeOfSupply, salesTaxRate, salesTaxStatus, attachment);

    private static CreateCategoryBankTransactionExplanationRequest Create(
        long? bankAccountId,
        long? bankTransactionId,
        string categoryNominalCode,
        DateOnly datedOn,
        decimal grossValue,
        string? description,
        BankTransactionExplanationEcStatus? ecStatus,
        string? placeOfSupply,
        decimal? salesTaxRate,
        BillSalesTaxStatus? salesTaxStatus,
        BankTransactionExplanationAttachmentWritePayload? attachment)
    {
        ValidateBankTarget(bankAccountId, bankTransactionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryNominalCode);

        return new CreateCategoryBankTransactionExplanationRequest
        {
            BankAccountId = bankAccountId,
            BankTransactionId = bankTransactionId,
            CategoryNominalCode = categoryNominalCode,
            DatedOn = datedOn,
            GrossValue = grossValue,
            Description = description,
            EcStatus = ecStatus,
            PlaceOfSupply = placeOfSupply,
            SalesTaxRate = salesTaxRate,
            SalesTaxStatus = salesTaxStatus,
            Attachment = attachment
        };
    }
}

/// <summary>
/// Request attributes for creating a payment or refund explanation with project rebilling.
/// </summary>
public sealed class CreatePaymentOrRefundBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal long ProjectId { get; private init; }

    internal BillRebillType RebillType { get; private init; }

    internal decimal? RebillFactor { get; private init; }

    internal string? ReceiptReference { get; private init; }

    internal string? CategoryNominalCode { get; private init; }

    /// <summary>
    /// Creates a payment or refund explanation with project rebilling.
    /// </summary>
    public static CreatePaymentOrRefundBankTransactionExplanationRequest Create(
        long bankTransactionId,
        long projectId,
        BillRebillType rebillType,
        DateOnly datedOn,
        decimal grossValue,
        decimal? rebillFactor = null,
        string? categoryNominalCode = null,
        string? description = null,
        string? receiptReference = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(projectId);

        return new CreatePaymentOrRefundBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            ProjectId = projectId,
            RebillType = rebillType,
            RebillFactor = rebillFactor,
            CategoryNominalCode = categoryNominalCode,
            DatedOn = datedOn,
            GrossValue = grossValue,
            Description = description,
            ReceiptReference = receiptReference
        };
    }
}

/// <summary>
/// Request attributes for creating an invoice receipt or credit note refund explanation.
/// </summary>
public sealed class CreateInvoiceReceiptBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal long? PaidInvoiceId { get; private init; }

    internal long? PaidCreditNoteId { get; private init; }

    internal decimal? ForeignCurrencyValue { get; private init; }

    /// <summary>
    /// Creates an invoice receipt explanation.
    /// </summary>
    public static CreateInvoiceReceiptBankTransactionExplanationRequest Create(
        long bankTransactionId,
        long paidInvoiceId,
        DateOnly datedOn,
        decimal grossValue,
        decimal? foreignCurrencyValue = null,
        string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(paidInvoiceId);

        return new CreateInvoiceReceiptBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            PaidInvoiceId = paidInvoiceId,
            DatedOn = datedOn,
            GrossValue = grossValue,
            ForeignCurrencyValue = foreignCurrencyValue,
            Description = description
        };
    }

    /// <summary>
    /// Creates a credit note refund explanation.
    /// </summary>
    public static CreateInvoiceReceiptBankTransactionExplanationRequest CreateCreditNoteRefund(
        long bankTransactionId,
        long paidCreditNoteId,
        DateOnly datedOn,
        decimal grossValue,
        decimal? foreignCurrencyValue = null,
        string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(paidCreditNoteId);

        return new CreateInvoiceReceiptBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            PaidCreditNoteId = paidCreditNoteId,
            DatedOn = datedOn,
            GrossValue = grossValue,
            ForeignCurrencyValue = foreignCurrencyValue,
            Description = description
        };
    }
}

/// <summary>
/// Request attributes for creating a bill payment or bill refund explanation.
/// </summary>
public sealed class CreateBillPaymentBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal long PaidBillId { get; private init; }

    internal decimal? ForeignCurrencyValue { get; private init; }

    /// <summary>
    /// Creates a bill payment or bill refund explanation.
    /// </summary>
    public static CreateBillPaymentBankTransactionExplanationRequest Create(
        long bankTransactionId,
        long paidBillId,
        DateOnly datedOn,
        decimal grossValue,
        decimal? foreignCurrencyValue = null,
        string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(paidBillId);

        return new CreateBillPaymentBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            PaidBillId = paidBillId,
            DatedOn = datedOn,
            GrossValue = grossValue,
            ForeignCurrencyValue = foreignCurrencyValue,
            Description = description
        };
    }
}

/// <summary>
/// Request attributes for creating a smart user payment (paid user without category).
/// </summary>
public sealed class CreateSmartUserPaymentBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal long PaidUserId { get; private init; }

    /// <summary>
    /// Creates a smart user payment explanation.
    /// </summary>
    public static CreateSmartUserPaymentBankTransactionExplanationRequest Create(
        long bankTransactionId,
        long paidUserId,
        DateOnly datedOn,
        decimal grossValue,
        string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(paidUserId);

        return new CreateSmartUserPaymentBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            PaidUserId = paidUserId,
            DatedOn = datedOn,
            GrossValue = grossValue,
            Description = description
        };
    }
}

/// <summary>
/// Request attributes for creating a categorised user payment explanation.
/// </summary>
public sealed class CreateCategorisedUserPaymentBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal long PaidUserId { get; private init; }

    internal string CategoryNominalCode { get; private init; } = string.Empty;

    /// <summary>
    /// Creates a categorised user payment explanation.
    /// </summary>
    public static CreateCategorisedUserPaymentBankTransactionExplanationRequest Create(
        long bankTransactionId,
        long paidUserId,
        string categoryNominalCode,
        DateOnly datedOn,
        decimal grossValue,
        string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(paidUserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryNominalCode);

        return new CreateCategorisedUserPaymentBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            PaidUserId = paidUserId,
            CategoryNominalCode = categoryNominalCode,
            DatedOn = datedOn,
            GrossValue = grossValue,
            Description = description
        };
    }
}

/// <summary>
/// Request attributes for creating a transfer between bank accounts.
/// </summary>
public sealed class CreateTransferBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal long TransferBankAccountId { get; private init; }

    /// <summary>
    /// Creates a transfer explanation between bank accounts.
    /// </summary>
    public static CreateTransferBankTransactionExplanationRequest Create(
        long bankTransactionId,
        long transferBankAccountId,
        DateOnly datedOn,
        decimal grossValue,
        string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(transferBankAccountId);

        return new CreateTransferBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            TransferBankAccountId = transferBankAccountId,
            DatedOn = datedOn,
            GrossValue = grossValue,
            Description = description
        };
    }
}

/// <summary>
/// Request attributes for creating a stock purchase or sale explanation.
/// </summary>
public sealed class CreateStockBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal long StockItemId { get; private init; }

    internal int StockAlteringQuantity { get; private init; }

    internal string? CategoryNominalCode { get; private init; }

    /// <summary>
    /// Creates a stock purchase or sale explanation.
    /// </summary>
    public static CreateStockBankTransactionExplanationRequest Create(
        long bankTransactionId,
        long stockItemId,
        int stockAlteringQuantity,
        DateOnly datedOn,
        decimal grossValue,
        string? categoryNominalCode = null,
        string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(stockItemId);

        return new CreateStockBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            StockItemId = stockItemId,
            StockAlteringQuantity = stockAlteringQuantity,
            CategoryNominalCode = categoryNominalCode,
            DatedOn = datedOn,
            GrossValue = grossValue,
            Description = description
        };
    }
}

/// <summary>
/// Request attributes for creating a capital asset purchase explanation.
/// </summary>
public sealed class CreateCapitalAssetPurchaseBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal string CategoryNominalCode { get; private init; } = string.Empty;

    internal string? AssetLifeYears { get; private init; }

    /// <summary>
    /// Creates a capital asset purchase explanation.
    /// </summary>
    public static CreateCapitalAssetPurchaseBankTransactionExplanationRequest Create(
        long bankTransactionId,
        string categoryNominalCode,
        DateOnly datedOn,
        decimal grossValue,
        string? assetLifeYears = null,
        string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryNominalCode);

        return new CreateCapitalAssetPurchaseBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            CategoryNominalCode = categoryNominalCode,
            AssetLifeYears = assetLifeYears,
            DatedOn = datedOn,
            GrossValue = grossValue,
            Description = description
        };
    }
}

/// <summary>
/// Request attributes for creating a capital asset disposal explanation.
/// </summary>
public sealed class CreateCapitalAssetDisposalBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal long DisposedAssetId { get; private init; }

    /// <summary>
    /// Creates a capital asset disposal explanation.
    /// </summary>
    public static CreateCapitalAssetDisposalBankTransactionExplanationRequest Create(
        long bankTransactionId,
        long disposedAssetId,
        DateOnly datedOn,
        decimal grossValue,
        string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(disposedAssetId);

        return new CreateCapitalAssetDisposalBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            DisposedAssetId = disposedAssetId,
            DatedOn = datedOn,
            GrossValue = grossValue,
            Description = description
        };
    }
}

/// <summary>
/// Request attributes for creating a landlord property-linked explanation.
/// </summary>
public sealed class CreateLandlordPropertyBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal string CategoryNominalCode { get; private init; } = string.Empty;

    internal string PropertyUri { get; private init; } = string.Empty;

    /// <summary>
    /// Creates a landlord property-linked explanation.
    /// </summary>
    public static CreateLandlordPropertyBankTransactionExplanationRequest Create(
        long bankTransactionId,
        string categoryNominalCode,
        string propertyUri,
        DateOnly datedOn,
        decimal grossValue,
        string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryNominalCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyUri);

        return new CreateLandlordPropertyBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            CategoryNominalCode = categoryNominalCode,
            PropertyUri = propertyUri,
            DatedOn = datedOn,
            GrossValue = grossValue,
            Description = description
        };
    }
}

/// <summary>
/// Request attributes for creating an opening balance debtor or creditor explanation.
/// </summary>
public sealed class CreateOpeningBalanceBankTransactionExplanationRequest : BankTransactionExplanationCreateRequestBase
{
    internal long DirectContactId { get; private init; }

    internal string CategoryNominalCode { get; private init; } = string.Empty;

    /// <summary>
    /// Creates an opening balance explanation for categories 681 or 796.
    /// </summary>
    public static CreateOpeningBalanceBankTransactionExplanationRequest Create(
        long bankTransactionId,
        long directContactId,
        string categoryNominalCode,
        DateOnly datedOn,
        decimal grossValue,
        string? description = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(directContactId);
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryNominalCode);

        return new CreateOpeningBalanceBankTransactionExplanationRequest
        {
            BankTransactionId = bankTransactionId,
            DirectContactId = directContactId,
            CategoryNominalCode = categoryNominalCode,
            DatedOn = datedOn,
            GrossValue = grossValue,
            Description = description
        };
    }
}
