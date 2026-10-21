namespace FreeAgent.Client.Models.SalesTaxPeriods;

/// <summary>
/// Attributes for updating a sales tax period.
/// </summary>
public sealed class UpdateSalesTaxPeriodRequest
{
    /// <summary>
    /// Name of the sales tax (for example VAT, GST, or Sales Tax).
    /// </summary>
    public string? SalesTaxName { get; set; }

    /// <summary>
    /// Registration status for this sales tax period.
    /// </summary>
    public SalesTaxPeriodRegistrationStatus? SalesTaxRegistrationStatus { get; set; }

    /// <summary>
    /// First rate of the sales tax.
    /// </summary>
    public decimal? SalesTaxRate1 { get; set; }

    /// <summary>
    /// Second rate of the sales tax.
    /// </summary>
    public decimal? SalesTaxRate2 { get; set; }

    /// <summary>
    /// Third rate of the sales tax.
    /// </summary>
    public decimal? SalesTaxRate3 { get; set; }

    /// <summary>
    /// Whether tax paid on purchases can be reclaimed.
    /// </summary>
    public bool? SalesTaxIsValueAdded { get; set; }

    /// <summary>
    /// Sales tax registration number when required.
    /// </summary>
    public string? SalesTaxRegistrationNumber { get; set; }

    /// <summary>
    /// Date from which this sales tax period takes effect.
    /// </summary>
    public DateOnly? EffectiveDate { get; set; }

    /// <summary>
    /// Name of the second sales tax for universal companies (for example PST).
    /// </summary>
    public string? SecondSalesTaxName { get; set; }

    /// <summary>
    /// First rate of the second sales tax.
    /// </summary>
    public decimal? SecondSalesTaxRate1 { get; set; }

    /// <summary>
    /// Second rate of the second sales tax.
    /// </summary>
    public decimal? SecondSalesTaxRate2 { get; set; }

    /// <summary>
    /// Third rate of the second sales tax.
    /// </summary>
    public decimal? SecondSalesTaxRate3 { get; set; }

    /// <summary>
    /// Whether the second sales tax is compounded on the main sales tax.
    /// </summary>
    public bool? SecondSalesTaxIsCompound { get; set; }
}
