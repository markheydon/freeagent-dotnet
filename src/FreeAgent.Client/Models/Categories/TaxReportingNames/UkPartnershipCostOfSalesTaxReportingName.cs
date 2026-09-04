using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>Tax reporting name wire values for cost of sales categories on UK partnerships.</summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<UkPartnershipCostOfSalesTaxReportingName>))]
public enum UkPartnershipCostOfSalesTaxReportingName
{
    /// <summary>Cost of sales.</summary>
    [JsonStringEnumMemberName("cost_of_sales")]
    CostOfSales,

    /// <summary>Subcontractor costs.</summary>
    [JsonStringEnumMemberName("subcontractor_costs")]
    SubcontractorCosts,

}
