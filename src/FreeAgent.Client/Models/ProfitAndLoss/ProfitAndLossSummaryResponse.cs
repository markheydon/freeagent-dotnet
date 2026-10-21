using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.ProfitAndLoss;

/// <summary>
/// Response envelope for profit and loss summary requests.
/// </summary>
internal sealed class ProfitAndLossSummaryResponse
{
    /// <summary>
    /// Profit and loss summary payload.
    /// </summary>
    [JsonPropertyName("profit_and_loss_summary")]
    public ProfitAndLossSummary? ProfitAndLossSummary { get; set; }
}
