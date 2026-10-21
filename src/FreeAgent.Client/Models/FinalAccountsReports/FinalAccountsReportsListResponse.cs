using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for listing final accounts reports.
/// </summary>
internal sealed class FinalAccountsReportsListResponse
{
    /// <summary>
    /// Final accounts reports for the company.
    /// </summary>
    [JsonPropertyName("final_accounts_reports")]
    public List<FinalAccountsReport>? FinalAccountsReports { get; set; }
}
