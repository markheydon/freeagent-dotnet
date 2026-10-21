using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using CashflowModel = FreeAgent.Client.Models.Cashflow.Cashflow;

namespace FreeAgent.Client.Tests.Models.Cashflow;

public class CashflowModelSerializationTests
{
    [Fact]
    public void Deserialize_MapsDocumentedCashflow()
    {
        const string json = """
            {
              "from": "2019-07-01",
              "to": "2019-09-30",
              "incoming": {
                "total": "68869.76",
                "months": [
                  { "month": 7, "year": 2019, "total": "21387.76" }
                ]
              },
              "outgoing": {
                "total": "56276.55",
                "months": [
                  { "month": 7, "year": 2019, "total": "16322.05" }
                ]
              },
              "balance": "12593.21"
            }
            """;

        var cashflow = JsonSerializer.Deserialize<CashflowModel>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(cashflow);
        Assert.Equal(new DateOnly(2019, 7, 1), cashflow.From);
        Assert.Equal(68869.76m, cashflow.Incoming?.Total);
        Assert.Single(cashflow.Incoming?.Months!);
        Assert.Equal(7, cashflow.Incoming!.Months![0].Month);
    }
}
