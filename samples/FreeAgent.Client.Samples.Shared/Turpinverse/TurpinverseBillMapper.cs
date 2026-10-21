namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Maps Turpinverse bill canon entries to FreeAgent bill payloads.
/// </summary>
internal static class TurpinverseBillMapper
{
    public const string ReferencePrefix = "turpinverse:";

    public static Bill ToFreeAgentBill(
        TurpinverseBill bill,
        long contactId,
        long? projectId,
        string categoryNominalCode,
        DateOnly minimumDocumentDate,
        IReadOnlyDictionary<string, long?> lineProjectIds)
    {
        ArgumentNullException.ThrowIfNull(bill);

        var issueDate = TurpinverseSalesDateSupport.ParseCanonDate(bill.IssueDate, nameof(bill.IssueDate));
        var dueDate = TurpinverseSalesDateSupport.ParseCanonDate(bill.DueDate, nameof(bill.DueDate));
        var datedOn = TurpinverseSalesDateSupport.ClampToMinimum(issueDate, minimumDocumentDate);
        var dueOn = TurpinverseSalesDateSupport.ClampToMinimum(dueDate, minimumDocumentDate);
        if (dueOn < datedOn)
        {
            dueOn = datedOn;
        }

        return new Bill
        {
            ContactId = contactId,
            ProjectId = projectId,
            Reference = BuildReference(bill.BillId),
            DatedOn = datedOn,
            DueOn = dueOn,
            Currency = ParseCurrency(bill.Currency),
            Comments = bill.Notes,
            BillItems = bill.Lines
                .Select(line => ToBillItem(line, categoryNominalCode, lineProjectIds))
                .ToList()
        };
    }

    public static string BuildReference(string billId) =>
        $"{ReferencePrefix}{billId}";

    private static BillItem ToBillItem(
        TurpinverseBillLine line,
        string categoryNominalCode,
        IReadOnlyDictionary<string, long?> lineProjectIds)
    {
        var (salesTaxRate, salesTaxStatus) = MapTax(line.TaxRateId);
        long? projectId = null;
        if (!string.IsNullOrWhiteSpace(line.ProjectId)
            && lineProjectIds.TryGetValue(line.ProjectId, out var resolvedProjectId))
        {
            projectId = resolvedProjectId;
        }

        return new BillItem
        {
            Description = line.Description,
            Quantity = line.Quantity,
            TotalValue = line.LineTotal,
            SalesTaxRate = PurchaseSalesTaxRate.FromPercentage(salesTaxRate),
            SalesTaxStatus = salesTaxStatus,
            CategoryNominalCode = categoryNominalCode,
            ProjectId = projectId
        };
    }

    private static (decimal Rate, BillSalesTaxStatus Status) MapTax(string? taxRateId) =>
        taxRateId switch
        {
            "tax-reduced" => (5m, BillSalesTaxStatus.Taxable),
            "tax-zero" => (0m, BillSalesTaxStatus.Exempt),
            _ => (20m, BillSalesTaxStatus.Taxable)
        };

    private static CurrencyCode ParseCurrency(string currency) =>
        Enum.TryParse<CurrencyCode>(currency, ignoreCase: true, out var parsed)
            ? parsed
            : CurrencyCode.GBP;
}
