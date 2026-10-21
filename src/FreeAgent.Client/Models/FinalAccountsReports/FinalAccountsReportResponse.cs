using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.FinalAccountsReports;

/// <summary>
/// Response envelope for a single final accounts report.
/// </summary>
internal sealed class FinalAccountsReportResponse
{
    /// <summary>
    /// Final accounts report payload.
    /// </summary>
    [JsonPropertyName("final_accounts_report")]
    public FinalAccountsReport? FinalAccountsReport { get; set; }
}
