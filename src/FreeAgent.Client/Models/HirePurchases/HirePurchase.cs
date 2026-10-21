using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Represents a FreeAgent hire purchase linked to an originating bill.
/// </summary>
/// <remarks>
/// Hire purchases are read-only in the FreeAgent API and are created or removed via the Bills API.
/// </remarks>
public sealed class HirePurchase : IFreeAgentResource
{
    /// <summary>
    /// Hire purchase resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Free-text description taken from the originating bill.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Wire representation of the originating bill link.
    /// </summary>
    [JsonPropertyName("bill")]
    [JsonInclude]
    internal ExpandableField<Bill>? BillLink { get; set; }

    /// <summary>
    /// Originating bill when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Bill? Bill => BillLink?.Value;

    /// <summary>
    /// Originating bill identifier parsed from the hire purchase response.
    /// </summary>
    [JsonIgnore]
    public long? BillId => BillLink?.Id;

    /// <summary>
    /// Wire representation of the liabilities over one year category link.
    /// </summary>
    [JsonPropertyName("liabilities_over_one_year_category")]
    [JsonInclude]
    internal ExpandableField<Category>? LiabilitiesOverOneYearCategoryLink { get; set; }

    /// <summary>
    /// Liabilities over one year category nominal code when returned as a URI link.
    /// </summary>
    [JsonIgnore]
    public string? LiabilitiesOverOneYearCategoryNominalCode => LiabilitiesOverOneYearCategoryLink?.Uri is string uri
        ? CategoryReference.ExtractNominalCode(uri)
        : null;

    /// <summary>
    /// Liabilities over one year category when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Category? LiabilitiesOverOneYearCategory => LiabilitiesOverOneYearCategoryLink?.Value;

    /// <summary>
    /// Wire representation of the liabilities under one year category link.
    /// </summary>
    [JsonPropertyName("liabilities_under_one_year_category")]
    [JsonInclude]
    internal ExpandableField<Category>? LiabilitiesUnderOneYearCategoryLink { get; set; }

    /// <summary>
    /// Liabilities under one year category nominal code when returned as a URI link.
    /// </summary>
    [JsonIgnore]
    public string? LiabilitiesUnderOneYearCategoryNominalCode => LiabilitiesUnderOneYearCategoryLink?.Uri is string uri
        ? CategoryReference.ExtractNominalCode(uri)
        : null;

    /// <summary>
    /// Liabilities under one year category when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Category? LiabilitiesUnderOneYearCategory => LiabilitiesUnderOneYearCategoryLink?.Value;

    internal void AttachBill(Bill bill)
    {
        ArgumentNullException.ThrowIfNull(bill);
        BillLink = new ExpandableField<Bill>(bill.Url, bill);
    }

    internal void AttachLiabilitiesOverOneYearCategory(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        LiabilitiesOverOneYearCategoryLink = new ExpandableField<Category>(category.Url, category);
    }

    internal void AttachLiabilitiesUnderOneYearCategory(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        LiabilitiesUnderOneYearCategoryLink = new ExpandableField<Category>(category.Url, category);
    }
}
