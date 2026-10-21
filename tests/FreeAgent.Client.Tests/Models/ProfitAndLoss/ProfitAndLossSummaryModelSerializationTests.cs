using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Tests.Models.ProfitAndLoss;

public class ProfitAndLossSummaryModelSerializationTests
{
    [Fact]
    public void Deserialize_MapsDocumentedProfitAndLossSummary()
    {
        const string json = """
            {
              "from": "2016-06-01",
              "to": "2016-09-05",
              "income": "3800",
              "expenses": "8200",
              "operating_profit": "-4400",
              "less": [
                { "title": "Corp. Tax", "total": "0" },
                { "title": "Dividends", "total": "50" }
              ],
              "retained_profit": "-4450",
              "retained_profit_brought_forward": "15817",
              "retained_profit_carried_forward": "11367"
            }
            """;

        var summary = JsonSerializer.Deserialize<ProfitAndLossSummary>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(summary);
        Assert.Equal(new DateOnly(2016, 6, 1), summary.From);
        Assert.Equal(new DateOnly(2016, 9, 5), summary.To);
        Assert.Equal(3800m, summary.Income);
        Assert.Equal(-4400m, summary.OperatingProfit);
        Assert.Equal(2, summary.Less?.Count);
        Assert.Equal("Dividends", summary.Less![1].Title);
        Assert.Equal(50m, summary.Less[1].Total);
        Assert.Equal(11367m, summary.RetainedProfitCarriedForward);
    }
}
