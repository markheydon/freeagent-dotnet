using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Models.DepreciationProfiles;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.BankTransactionExplanations;

/// <summary>
/// Writable bank transaction explanation attributes for create and update requests.
/// </summary>
internal sealed class BankTransactionExplanationWritePayload
{
    [JsonPropertyName("bank_account")]
    public BankAccountReference? BankAccount { get; set; }

    [JsonPropertyName("bank_transaction")]
    public BankTransactionReference? BankTransaction { get; set; }

    [JsonPropertyName("category")]
    public CategoryReference? Category { get; set; }

    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    [JsonPropertyName("gross_value")]
    public decimal? GrossValue { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("ec_status")]
    public BankTransactionExplanationEcStatus? EcStatus { get; set; }

    [JsonPropertyName("place_of_supply")]
    public string? PlaceOfSupply { get; set; }

    [JsonPropertyName("sales_tax_rate")]
    public decimal? SalesTaxRate { get; set; }

    [JsonPropertyName("second_sales_tax_rate")]
    public decimal? SecondSalesTaxRate { get; set; }

    [JsonPropertyName("sales_tax_value")]
    public decimal? SalesTaxValue { get; set; }

    [JsonPropertyName("second_sales_tax_value")]
    public decimal? SecondSalesTaxValue { get; set; }

    [JsonPropertyName("sales_tax_status")]
    public BillSalesTaxStatus? SalesTaxStatus { get; set; }

    [JsonPropertyName("second_sales_tax_status")]
    public string? SecondSalesTaxStatus { get; set; }

    [JsonPropertyName("cheque_number")]
    public string? ChequeNumber { get; set; }

    [JsonPropertyName("attachment")]
    public BankTransactionExplanationAttachmentWritePayload? Attachment { get; set; }

    [JsonPropertyName("project")]
    public ProjectReference? Project { get; set; }

    [JsonPropertyName("rebill_type")]
    public BillRebillType? RebillType { get; set; }

    [JsonPropertyName("rebill_factor")]
    public decimal? RebillFactor { get; set; }

    [JsonPropertyName("receipt_reference")]
    public string? ReceiptReference { get; set; }

    [JsonPropertyName("paid_invoice")]
    public string? PaidInvoice { get; set; }

    [JsonPropertyName("foreign_currency_value")]
    public decimal? ForeignCurrencyValue { get; set; }

    [JsonPropertyName("paid_bill")]
    public BillReference? PaidBill { get; set; }

    [JsonPropertyName("paid_user")]
    public UserReference? PaidUser { get; set; }

    [JsonPropertyName("transfer_bank_account")]
    public BankAccountReference? TransferBankAccount { get; set; }

    [JsonPropertyName("stock_item")]
    public StockItemReference? StockItem { get; set; }

    [JsonPropertyName("stock_altering_quantity")]
    public int? StockAlteringQuantity { get; set; }

    [JsonPropertyName("asset_life_years")]
    public string? AssetLifeYears { get; set; }

    [JsonPropertyName("capital_asset")]
    public CapitalAssetPurchaseWritePayload? CapitalAsset { get; set; }

    [JsonPropertyName("disposed_asset")]
    public CapitalAssetReference? DisposedAsset { get; set; }

    [JsonPropertyName("property")]
    public string? PropertyUri { get; set; }

    [JsonPropertyName("direct_contact")]
    public ContactReference? DirectContact { get; set; }

    internal static BankTransactionExplanationWritePayload FromUpdate(
        UpdateBankTransactionExplanationRequest request,
        FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new BankTransactionExplanationWritePayload
        {
            Category = LinkedResourceWriteMapper.ToCategoryReference(environment, request.CategoryNominalCode),
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
            Attachment = request.Attachment,
            Project = LinkedResourceWriteMapper.ToProjectReference(environment, request.ProjectId),
            RebillType = request.RebillType,
            RebillFactor = request.RebillFactor,
            ReceiptReference = request.ReceiptReference,
            PaidInvoice = LinkedResourceWriteMapper.ToPaidInvoiceWireUri(
                environment,
                request.PaidInvoiceId,
                request.PaidCreditNoteId),
            ForeignCurrencyValue = request.ForeignCurrencyValue,
            PaidBill = LinkedResourceWriteMapper.ToBillReference(environment, request.PaidBillId),
            PaidUser = LinkedResourceWriteMapper.ToUserReference(environment, request.PaidUserId),
            TransferBankAccount = LinkedResourceWriteMapper.ToBankAccountReference(environment, request.TransferBankAccountId),
            StockItem = LinkedResourceWriteMapper.ToStockItemReference(environment, request.StockItemId),
            StockAlteringQuantity = request.StockAlteringQuantity,
            AssetLifeYears = request.CapitalAssetPurchase is null ? request.AssetLifeYears : null,
            CapitalAsset = CapitalAssetPurchaseWritePayload.FromWrite(request.CapitalAssetPurchase),
            DisposedAsset = LinkedResourceWriteMapper.ToCapitalAssetReference(environment, request.DisposedAssetId),
            PropertyUri = request.PropertyUri,
            DirectContact = LinkedResourceWriteMapper.ToContactReference(environment, request.DirectContactId)
        };
    }
}
