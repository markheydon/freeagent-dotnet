using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.FinalAccountsReports;

namespace FreeAgent.Client.Tests.Models.FinalAccountsReports;

public class FinalAccountsReportModelSerializationTests
{
    [Fact]
    public void Deserialize_MapsDocumentedFinalAccountsReport()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/final_accounts_reports/2022-12-31",
              "period_ends_on": "2022-12-31",
              "period_starts_on": "2022-01-01",
              "filing_due_on": "2023-09-30",
              "filing_status": "pending",
              "filed_at": "2023-10-09T17:13:34.119+00:00",
              "filed_reference": "REF123"
            }
            """;

        var report = JsonSerializer.Deserialize<FinalAccountsReport>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(report);
        Assert.Equal(FinalAccountsReportFilingStatus.Pending, report.FilingStatus);
        Assert.Equal("REF123", report.FiledReference);
    }
}
