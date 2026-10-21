using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Transactions;

/// <summary>
/// A ledger transaction from the FreeAgent accounting transactions report.
/// </summary>
public sealed class AccountingTransaction : IFreeAgentResource
{
    /// <summary>
    /// Transaction resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Date of the transaction.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// When the transaction was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// When the transaction was last updated.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Description of the source of the transaction.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Wire representation of the category.
    /// </summary>
    [JsonPropertyName("category")]
    [JsonInclude]
    internal ExpandableField<Category>? CategoryLink { get; set; }

    /// <summary>
    /// Category when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Category? Category => CategoryLink?.Value;

    /// <summary>
    /// Category nominal code parsed from the category link when present.
    /// </summary>
    [JsonIgnore]
    public string? CategoryNominalCode => CategoryLink?.Uri is string uri
        ? CategoryReference.ExtractNominalCode(uri)
        : CategoryLink?.Value?.NominalCode;

    /// <summary>
    /// Name of the category associated with the transaction.
    /// </summary>
    [JsonPropertyName("category_name")]
    public string? CategoryName { get; set; }

    /// <summary>
    /// Nominal code of the category, which may include a sub-account suffix.
    /// </summary>
    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; set; }

    /// <summary>
    /// Debit value for this transaction. Credit values are negative.
    /// </summary>
    [JsonPropertyName("debit_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DebitValue { get; set; }

    /// <summary>
    /// URI of the source item that posted this transaction, when FreeAgent provides one.
    /// </summary>
    /// <remarks>
    /// The path identifies different resource types (for example bills or bank transaction explanations).
    /// Use <see cref="SourceItemId"/> when the final path segment is numeric.
    /// </remarks>
    [JsonPropertyName("source_item_url")]
    public string? SourceItemUrl { get; set; }

    /// <summary>
    /// Numeric identifier parsed from <see cref="SourceItemUrl"/> when the final path segment is numeric.
    /// </summary>
    [JsonIgnore]
    public long? SourceItemId =>
        !string.IsNullOrWhiteSpace(SourceItemUrl) && FreeAgentResourceId.TryParse(SourceItemUrl, out var id)
            ? id
            : null;

    /// <summary>
    /// Foreign-currency amounts. Empty on the wire when the transaction uses the company currency.
    /// </summary>
    [JsonPropertyName("foreign_currency_data")]
    public AccountingTransactionForeignCurrency? ForeignCurrency { get; set; }
}
