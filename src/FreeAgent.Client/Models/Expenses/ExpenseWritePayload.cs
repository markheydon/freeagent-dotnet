using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Writable expense attributes for create and update requests.
/// </summary>
internal sealed class ExpenseWritePayload
{
    [JsonPropertyName("user")]
    [JsonConverter(typeof(WriteLinkJsonConverter<UserReference>))]
    public WriteLink<UserReference>? User { get; set; }

    [JsonPropertyName("category")]
    [JsonConverter(typeof(ExpenseCategoryWriteJsonConverter))]
    public ExpenseCategoryWriteValue? Category { get; set; }

    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    [JsonPropertyName("gross_value")]
    public decimal? GrossValue { get; set; }

    [JsonPropertyName("native_gross_value")]
    public decimal? NativeGrossValue { get; set; }

    [JsonPropertyName("sales_tax_rate")]
    public PurchaseSalesTaxRate? SalesTaxRate { get; set; }

    [JsonPropertyName("sales_tax_value")]
    public decimal? SalesTaxValue { get; set; }

    [JsonPropertyName("native_sales_tax_value")]
    public decimal? NativeSalesTaxValue { get; set; }

    [JsonPropertyName("second_sales_tax_rate")]
    public decimal? SecondSalesTaxRate { get; set; }

    [JsonPropertyName("manual_sales_tax_amount")]
    public decimal? ManualSalesTaxAmount { get; set; }

    [JsonPropertyName("sales_tax_status")]
    public BillSalesTaxStatus? SalesTaxStatus { get; set; }

    [JsonPropertyName("second_sales_tax_status")]
    public SecondSalesTaxStatus? SecondSalesTaxStatus { get; set; }

    [JsonPropertyName("ec_status")]
    public BillEcStatus? EcStatus { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("receipt_reference")]
    public string? ReceiptReference { get; set; }

    [JsonPropertyName("stock_item")]
    [JsonConverter(typeof(WriteLinkJsonConverter<StockItemReference>))]
    public WriteLink<StockItemReference>? StockItem { get; set; }

    [JsonPropertyName("stock_altering_quantity")]
    public decimal? StockAlteringQuantity { get; set; }

    [JsonPropertyName("project")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ProjectReference>))]
    public WriteLink<ProjectReference>? Project { get; set; }

    [JsonPropertyName("rebill_type")]
    public BillRebillType? RebillType { get; set; }

    [JsonPropertyName("rebill_factor")]
    public decimal? RebillFactor { get; set; }

    [JsonPropertyName("rebill_to_project")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ProjectReference>))]
    public WriteLink<ProjectReference>? RebillToProject { get; set; }

    [JsonPropertyName("property")]
    public string? Property { get; set; }

    [JsonPropertyName("recurring")]
    public BillRecurringSchedule? Recurring { get; set; }

    [JsonPropertyName("next_recurs_on")]
    public DateOnly? NextRecursOn { get; set; }

    [JsonPropertyName("recurring_end_date")]
    public DateOnly? RecurringEndDate { get; set; }

    [JsonPropertyName("attachment")]
    public ExpenseAttachmentWritePayload? Attachment { get; set; }

    [JsonPropertyName("mileage")]
    public decimal? Mileage { get; set; }

    [JsonPropertyName("vehicle_type")]
    public ExpenseVehicleType? VehicleType { get; set; }

    [JsonPropertyName("engine_type")]
    public ExpenseEngineType? EngineType { get; set; }

    [JsonPropertyName("engine_size")]
    public string? EngineSize { get; set; }

    [JsonPropertyName("reclaim_mileage")]
    public ExpenseReclaimMileage? ReclaimMileage { get; set; }

    [JsonPropertyName("initial_rate_mileage")]
    public decimal? InitialRateMileage { get; set; }

    [JsonPropertyName("reclaim_mileage_rate")]
    public decimal? ReclaimMileageRate { get; set; }

    [JsonPropertyName("rebill_mileage_rate")]
    public decimal? RebillMileageRate { get; set; }

    [JsonPropertyName("have_vat_receipt")]
    public bool? HaveVatReceipt { get; set; }

    [JsonPropertyName("capital_asset")]
    public CapitalAssetPurchaseWritePayload? CapitalAsset { get; set; }

    public static ExpenseWritePayload FromExpense(Expense expense, FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(expense);

        var category = LinkedResourceWriteMapper.ResolveExpenseCategoryWrite(
            environment,
            expense.IsMileageCategoryBacking,
            expense.CategoryLinkIsMileage,
            expense.CategoryNominalCodeBacking,
            expense.CategoryLinkNominalCode);

        if (expense.IsMileageCategoryBacking == false && category is null)
        {
            throw new ArgumentException(
                "A category nominal code is required when IsMileageCategory is false.",
                nameof(expense));
        }

        return new ExpenseWritePayload
        {
            User = LinkedResourceWriteMapper.ResolveUserReference(
                environment,
                expense.UserIdBacking,
                expense.UserLinkId),
            Category = category,
            DatedOn = expense.DatedOn,
            Currency = expense.Currency,
            GrossValue = expense.GrossValue,
            NativeGrossValue = expense.NativeGrossValue,
            SalesTaxRate = expense.SalesTaxRate,
            SalesTaxValue = expense.SalesTaxValue,
            NativeSalesTaxValue = expense.NativeSalesTaxValue,
            SecondSalesTaxRate = expense.SecondSalesTaxRate,
            ManualSalesTaxAmount = expense.ManualSalesTaxAmount,
            SalesTaxStatus = expense.SalesTaxStatus,
            SecondSalesTaxStatus = expense.SecondSalesTaxStatus,
            EcStatus = expense.EcStatus,
            Description = expense.Description,
            ReceiptReference = expense.ReceiptReference,
            StockItem = LinkedResourceWriteMapper.ResolveStockItemReference(
                environment,
                expense.StockItemIdBacking,
                expense.StockItemLinkId),
            StockAlteringQuantity = expense.StockAlteringQuantity,
            Project = LinkedResourceWriteMapper.ResolveProjectReference(
                environment,
                expense.ProjectIdBacking,
                expense.ProjectLinkId),
            RebillType = expense.RebillType,
            RebillFactor = expense.RebillFactor,
            RebillToProject = LinkedResourceWriteMapper.ResolveProjectReference(
                environment,
                expense.RebillToProjectIdBacking,
                expense.RebillToProjectLinkId),
            Property = expense.PropertyUri,
            Recurring = expense.Recurring,
            NextRecursOn = expense.NextRecursOn,
            RecurringEndDate = expense.RecurringEndDate,
            Attachment = expense.AttachmentWrite,
            Mileage = expense.Mileage,
            VehicleType = expense.VehicleType,
            EngineType = expense.EngineType,
            EngineSize = expense.EngineSize,
            ReclaimMileage = expense.ReclaimMileage,
            InitialRateMileage = expense.InitialRateMileage,
            ReclaimMileageRate = expense.ReclaimMileageRate,
            RebillMileageRate = expense.RebillMileageRate,
            HaveVatReceipt = expense.HaveVatReceipt,
            CapitalAsset = CapitalAssetPurchaseWritePayload.FromWrite(expense.CapitalAssetPurchase)
        };
    }
}
