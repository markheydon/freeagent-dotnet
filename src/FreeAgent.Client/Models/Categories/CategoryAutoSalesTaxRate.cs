using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Automatic sales tax rate assigned to a category.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CategoryAutoSalesTaxRate>))]
public enum CategoryAutoSalesTaxRate
{
    /// <summary>Outside of the scope of VAT.</summary>
    [JsonStringEnumMemberName("Outside of the scope of VAT")]
    OutsideOfTheScopeOfVat,

    /// <summary>Zero rate.</summary>
    [JsonStringEnumMemberName("Zero rate")]
    ZeroRate,

    /// <summary>Reduced rate.</summary>
    [JsonStringEnumMemberName("Reduced rate")]
    ReducedRate,

    /// <summary>Standard rate.</summary>
    [JsonStringEnumMemberName("Standard rate")]
    StandardRate,

    /// <summary>Exempt (income categories only).</summary>
    [JsonStringEnumMemberName("Exempt")]
    Exempt
}
