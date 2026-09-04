using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>Tax reporting name wire values for current asset categories (all company types).</summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CurrentAssetTaxReportingName>))]
public enum CurrentAssetTaxReportingName
{
    /// <summary>Debtors.</summary>
    [JsonStringEnumMemberName("debtors")]
    Debtors,

    /// <summary>Money in transit.</summary>
    [JsonStringEnumMemberName("money_in_transit")]
    MoneyInTransit,

    /// <summary>Prepayments and accrued income.</summary>
    [JsonStringEnumMemberName("prepayments_and_accrued_income")]
    PrepaymentsAndAccruedIncome,

}
