using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Category group used when creating a category.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CategoryGroup>))]
public enum CategoryGroup
{
    /// <summary>Income category.</summary>
    [JsonStringEnumMemberName("income")]
    Income,

    /// <summary>Cost of sales category.</summary>
    [JsonStringEnumMemberName("cost_of_sales")]
    CostOfSales,

    /// <summary>Admin expenses category.</summary>
    [JsonStringEnumMemberName("admin_expenses")]
    AdminExpenses,

    /// <summary>Current assets category.</summary>
    [JsonStringEnumMemberName("current_assets")]
    CurrentAssets,

    /// <summary>Liabilities category.</summary>
    [JsonStringEnumMemberName("liabilities")]
    Liabilities,

    /// <summary>Equities category.</summary>
    [JsonStringEnumMemberName("equities")]
    Equities
}
