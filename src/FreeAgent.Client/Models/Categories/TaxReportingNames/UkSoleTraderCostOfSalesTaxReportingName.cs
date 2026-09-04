using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>Tax reporting name wire values for cost of sales categories on UK sole traders.</summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<UkSoleTraderCostOfSalesTaxReportingName>))]
public enum UkSoleTraderCostOfSalesTaxReportingName
{
    /// <summary>Cost of goods.</summary>
    [JsonStringEnumMemberName("cost_of_goods")]
    CostOfGoods,

    /// <summary>Subcontractor costs.</summary>
    [JsonStringEnumMemberName("subcontractor_costs")]
    SubcontractorCosts,

    /// <summary>Construction industry scheme subcontractor costs.</summary>
    /// <remarks>Available only when CIS for Contractors is enabled.</remarks>
    [JsonStringEnumMemberName("construction_industry_scheme_subcontractor_costs")]
    ConstructionIndustrySchemeSubcontractorCosts,

}
