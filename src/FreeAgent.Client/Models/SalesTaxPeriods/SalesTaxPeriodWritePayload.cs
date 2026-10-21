using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Writable sales tax period attributes for create and update requests.
/// </summary>
internal sealed class SalesTaxPeriodWritePayload
{
    [JsonPropertyName("sales_tax_name")]
    public string? SalesTaxName { get; set; }

    [JsonPropertyName("sales_tax_registration_status")]
    public SalesTaxPeriodRegistrationStatus? SalesTaxRegistrationStatus { get; set; }

    [JsonPropertyName("sales_tax_rate_1")]
    public decimal? SalesTaxRate1 { get; set; }

    [JsonPropertyName("sales_tax_rate_2")]
    public decimal? SalesTaxRate2 { get; set; }

    [JsonPropertyName("sales_tax_rate_3")]
    public decimal? SalesTaxRate3 { get; set; }

    [JsonPropertyName("sales_tax_is_value_added")]
    public bool? SalesTaxIsValueAdded { get; set; }

    [JsonPropertyName("sales_tax_registration_number")]
    public string? SalesTaxRegistrationNumber { get; set; }

    [JsonPropertyName("effective_date")]
    public DateOnly? EffectiveDate { get; set; }

    [JsonPropertyName("second_sales_tax_name")]
    public string? SecondSalesTaxName { get; set; }

    [JsonPropertyName("second_sales_tax_rate_1")]
    public decimal? SecondSalesTaxRate1 { get; set; }

    [JsonPropertyName("second_sales_tax_rate_2")]
    public decimal? SecondSalesTaxRate2 { get; set; }

    [JsonPropertyName("second_sales_tax_rate_3")]
    public decimal? SecondSalesTaxRate3 { get; set; }

    [JsonPropertyName("second_sales_tax_is_compound")]
    public bool? SecondSalesTaxIsCompound { get; set; }

    public static SalesTaxPeriodWritePayload FromCreate(CreateSalesTaxPeriodRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new SalesTaxPeriodWritePayload
        {
            SalesTaxName = request.SalesTaxName,
            SalesTaxRegistrationStatus = request.SalesTaxRegistrationStatus,
            SalesTaxRate1 = request.SalesTaxRate1,
            SalesTaxRate2 = request.SalesTaxRate2,
            SalesTaxRate3 = request.SalesTaxRate3,
            SalesTaxIsValueAdded = request.SalesTaxIsValueAdded,
            SalesTaxRegistrationNumber = request.SalesTaxRegistrationNumber,
            EffectiveDate = request.EffectiveDate,
            SecondSalesTaxName = request.SecondSalesTaxName,
            SecondSalesTaxRate1 = request.SecondSalesTaxRate1,
            SecondSalesTaxRate2 = request.SecondSalesTaxRate2,
            SecondSalesTaxRate3 = request.SecondSalesTaxRate3,
            SecondSalesTaxIsCompound = request.SecondSalesTaxIsCompound
        };
    }

    public static SalesTaxPeriodWritePayload FromUpdate(UpdateSalesTaxPeriodRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new SalesTaxPeriodWritePayload
        {
            SalesTaxName = request.SalesTaxName,
            SalesTaxRegistrationStatus = request.SalesTaxRegistrationStatus,
            SalesTaxRate1 = request.SalesTaxRate1,
            SalesTaxRate2 = request.SalesTaxRate2,
            SalesTaxRate3 = request.SalesTaxRate3,
            SalesTaxIsValueAdded = request.SalesTaxIsValueAdded,
            SalesTaxRegistrationNumber = request.SalesTaxRegistrationNumber,
            EffectiveDate = request.EffectiveDate,
            SecondSalesTaxName = request.SecondSalesTaxName,
            SecondSalesTaxRate1 = request.SecondSalesTaxRate1,
            SecondSalesTaxRate2 = request.SecondSalesTaxRate2,
            SecondSalesTaxRate3 = request.SecondSalesTaxRate3,
            SecondSalesTaxIsCompound = request.SecondSalesTaxIsCompound
        };
    }
}
