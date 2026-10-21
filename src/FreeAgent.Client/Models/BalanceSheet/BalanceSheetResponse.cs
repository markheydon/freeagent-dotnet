using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.BalanceSheet;

/// <summary>
/// API envelope for balance sheet responses.
/// </summary>
public sealed class BalanceSheetResponse
{
    /// <summary>
    /// Balance sheet payload.
    /// </summary>
    [JsonPropertyName("balance_sheet")]
    public BalanceSheet? BalanceSheet { get; set; }
}
