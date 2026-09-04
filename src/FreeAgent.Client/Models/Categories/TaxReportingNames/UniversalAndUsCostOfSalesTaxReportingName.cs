using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>Tax reporting name wire values for cost of sales categories on universal and US companies.</summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<UniversalAndUsCostOfSalesTaxReportingName>))]
public enum UniversalAndUsCostOfSalesTaxReportingName
{
    /// <summary>Cost of labor.</summary>
    [JsonStringEnumMemberName("cost_of_labor")]
    CostOfLabor,

    /// <summary>Materials and supplies.</summary>
    [JsonStringEnumMemberName("materials_and_supplies")]
    MaterialsAndSupplies,

    /// <summary>Other costs.</summary>
    [JsonStringEnumMemberName("other_costs")]
    OtherCosts,

    /// <summary>Purchases.</summary>
    [JsonStringEnumMemberName("purchases")]
    Purchases,

}
