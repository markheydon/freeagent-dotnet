namespace FreeAgent.Client.Models.SalesTaxPeriods;

/// <summary>
/// Attributes for creating a sales tax period.
/// </summary>
public sealed class CreateSalesTaxPeriodRequest
{
    /// <summary>
    /// Name of the sales tax (for example VAT, GST, or Sales Tax).
    /// </summary>
    public required string SalesTaxName { get; set; }

    /// <summary>
    /// Registration status for this sales tax period.
    /// </summary>
    public required SalesTaxPeriodRegistrationStatus SalesTaxRegistrationStatus { get; set; }

    /// <summary>
    /// First rate of the sales tax.
    /// </summary>
    public required decimal SalesTaxRate1 { get; set; }

    /// <summary>
    /// Whether tax paid on purchases can be reclaimed.
    /// </summary>
    public required bool SalesTaxIsValueAdded { get; set; }

    /// <summary>
    /// Date from which this sales tax period takes effect.
    /// </summary>
    public required DateOnly EffectiveDate { get; set; }

    /// <summary>
    /// Second rate of the sales tax.
    /// </summary>
    public decimal? SalesTaxRate2 { get; set; }

    /// <summary>
    /// Third rate of the sales tax.
    /// </summary>
    public decimal? SalesTaxRate3 { get; set; }

    /// <summary>
    /// Sales tax registration number when required.
    /// </summary>
    public string? SalesTaxRegistrationNumber { get; set; }

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
