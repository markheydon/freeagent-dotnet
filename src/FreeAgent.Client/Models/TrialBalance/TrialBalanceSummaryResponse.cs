using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.TrialBalance;

/// <summary>
/// API envelope for trial balance summary responses.
/// </summary>
public sealed class TrialBalanceSummaryResponse
{
    /// <summary>
    /// Trial balance summary lines.
    /// </summary>
    [JsonPropertyName("trial_balance_summary")]
    public IReadOnlyList<TrialBalanceLine>? TrialBalanceSummary { get; set; }
}
