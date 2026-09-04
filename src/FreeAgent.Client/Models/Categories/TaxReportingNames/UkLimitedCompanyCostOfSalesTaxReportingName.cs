using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>Tax reporting name wire values for cost of sales categories on UK limited companies.</summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<UkLimitedCompanyCostOfSalesTaxReportingName>))]
public enum UkLimitedCompanyCostOfSalesTaxReportingName
{
    /// <summary>Commissions payable.</summary>
    [JsonStringEnumMemberName("commissions_payable")]
    CommissionsPayable,

    /// <summary>Material costs.</summary>
    [JsonStringEnumMemberName("material_costs")]
    MaterialCosts,

    /// <summary>Purchases.</summary>
    [JsonStringEnumMemberName("purchases")]
    Purchases,

    /// <summary>Subcontractor costs.</summary>
    [JsonStringEnumMemberName("subcontractor_costs")]
    SubcontractorCosts,

    /// <summary>Construction industry scheme subcontractor costs.</summary>
    /// <remarks>Available only when CIS for Contractors is enabled.</remarks>
    [JsonStringEnumMemberName("construction_industry_scheme_subcontractor_costs")]
    ConstructionIndustrySchemeSubcontractorCosts,

}
