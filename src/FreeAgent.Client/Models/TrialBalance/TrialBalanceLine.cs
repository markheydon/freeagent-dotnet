using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankAccounts;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.Users;

namespace FreeAgent.Client.Models.TrialBalance;

/// <summary>
/// A single trial balance summary line for a category account.
/// </summary>
public sealed class TrialBalanceLine
{
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
    /// Legacy nominal code (may include resource identifiers for bank and user categories).
    /// </summary>
    [JsonPropertyName("nominal_code")]
    public string? NominalCode { get; set; }

    /// <summary>
    /// Nominal code as displayed on FreeAgent accounting reports.
    /// </summary>
    [JsonPropertyName("display_nominal_code")]
    public string? DisplayNominalCode { get; set; }

    /// <summary>
    /// Account name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Trial balance total for the line.
    /// </summary>
    [JsonPropertyName("total")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Total { get; set; }

    /// <summary>
    /// Wire representation of the related bank account when present.
    /// </summary>
    [JsonPropertyName("bank_account")]
    [JsonInclude]
    internal ExpandableField<BankAccount>? BankAccountLink { get; set; }

    /// <summary>
    /// Bank account when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public BankAccount? BankAccount => BankAccountLink?.Value;

    /// <summary>
    /// Bank account identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? BankAccountId => BankAccountLink?.Id;

    /// <summary>
    /// Wire representation of the related user when present.
    /// </summary>
    [JsonPropertyName("user")]
    [JsonInclude]
    internal ExpandableField<User>? UserLink { get; set; }

    /// <summary>
    /// User when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public User? User => UserLink?.Value;

    /// <summary>
    /// User identifier parsed from the response.
    /// </summary>
    [JsonIgnore]
    public long? UserId => UserLink?.Id;
}
