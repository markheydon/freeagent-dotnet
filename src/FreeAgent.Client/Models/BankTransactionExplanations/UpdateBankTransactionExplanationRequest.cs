using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Models.DepreciationProfiles;
using FreeAgent.Client.Models.EcMoss;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.BankTransactionExplanations;

/// <summary>
/// Request attributes for updating a bank transaction explanation.
/// </summary>
public sealed class UpdateBankTransactionExplanationRequest
{
    /// <summary>
    /// Category nominal code to set.
    /// </summary>
    public string? CategoryNominalCode { get; set; }

    /// <summary>
    /// Date of the explanation.
    /// </summary>
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Gross value in the bank account currency.
    /// </summary>
    public decimal? GrossValue { get; set; }

    /// <summary>
    /// Free-text description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// VAT status for reporting purposes.
    /// </summary>
    public BankTransactionExplanationEcStatus? EcStatus { get; set; }

    /// <summary>
    /// Place of supply when <see cref="EcStatus"/> is EC VAT MOSS.
    /// </summary>
    public EcMossPlaceOfSupply? PlaceOfSupply { get; set; }

    /// <summary>
    /// Sales tax rate.
    /// </summary>
    public decimal? SalesTaxRate { get; set; }

    /// <summary>
    /// Second sales tax rate for universal accounts.
    /// </summary>
    public decimal? SecondSalesTaxRate { get; set; }

    /// <summary>
    /// Sales tax value.
    /// </summary>
    public decimal? SalesTaxValue { get; set; }

    /// <summary>
    /// Second sales tax value for universal accounts.
    /// </summary>
    public decimal? SecondSalesTaxValue { get; set; }

    /// <summary>
    /// Sales tax status.
    /// </summary>
    public BillSalesTaxStatus? SalesTaxStatus { get; set; }

    /// <summary>
    /// Second sales tax status for universal accounts.
    /// </summary>
    public SecondSalesTaxStatus? SecondSalesTaxStatus { get; set; }

    /// <summary>
    /// Cheque number.
    /// </summary>
    public string? ChequeNumber { get; set; }

    /// <summary>
    /// Attachment upload payload.
    /// </summary>
    public BankTransactionExplanationAttachmentWritePayload? Attachment { get; set; }

    /// <summary>
    /// Project identifier for payment or refund rebilling.
    /// </summary>
    public long? ProjectId { get; set; }

    /// <summary>
    /// Rebill type.
    /// </summary>
    public BillRebillType? RebillType { get; set; }

    /// <summary>
    /// Rebill factor.
    /// </summary>
    public decimal? RebillFactor { get; set; }

    /// <summary>
    /// Receipt reference.
    /// </summary>
    public string? ReceiptReference { get; set; }

    /// <summary>
    /// Paid invoice identifier.
    /// </summary>
    public long? PaidInvoiceId { get; set; }

    /// <summary>
    /// Refunded credit note identifier (serialised to the <c>paid_invoice</c> wire field).
    /// </summary>
    public long? PaidCreditNoteId { get; set; }

    /// <summary>
    /// Foreign currency value.
    /// </summary>
    public decimal? ForeignCurrencyValue { get; set; }

    /// <summary>
    /// Paid bill identifier.
    /// </summary>
    public long? PaidBillId { get; set; }

    /// <summary>
    /// Paid user identifier.
    /// </summary>
    public long? PaidUserId { get; set; }

    /// <summary>
    /// Transfer bank account identifier.
    /// </summary>
    public long? TransferBankAccountId { get; set; }

    /// <summary>
    /// Stock item identifier.
    /// </summary>
    public long? StockItemId { get; set; }

    /// <summary>
    /// Stock quantity change.
    /// </summary>
    public int? StockAlteringQuantity { get; set; }

    /// <summary>
    /// Deprecated asset life years for capital purchases.
    /// </summary>
    public string? AssetLifeYears { get; set; }

    /// <summary>
    /// Capital asset purchase attributes when updating with a depreciation profile.
    /// </summary>
    public CapitalAssetPurchaseWrite? CapitalAssetPurchase { get; set; }

    /// <summary>
    /// Disposed capital asset identifier.
    /// </summary>
    public long? DisposedAssetId { get; set; }

    /// <summary>
    /// Property URI for landlord companies.
    /// </summary>
    public string? PropertyUri { get; set; }

    /// <summary>
    /// Opening balance contact identifier.
    /// </summary>
    public long? DirectContactId { get; set; }
}
