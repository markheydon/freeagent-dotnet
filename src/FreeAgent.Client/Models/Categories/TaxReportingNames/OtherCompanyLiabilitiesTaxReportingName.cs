using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>Tax reporting name wire values for liabilities categories on all company types except UK limited companies.</summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<OtherCompanyLiabilitiesTaxReportingName>))]
public enum OtherCompanyLiabilitiesTaxReportingName
{
    /// <summary>Accruals and deferred income.</summary>
    [JsonStringEnumMemberName("accruals_and_deferred_income")]
    AccrualsAndDeferredIncome,

    /// <summary>Creditors.</summary>
    [JsonStringEnumMemberName("creditors")]
    Creditors,

}
