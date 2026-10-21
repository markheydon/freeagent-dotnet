using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Tests.Models.TrialBalance;

public class TrialBalanceLineModelSerializationTests
{
    [Fact]
    public void Deserialize_MapsDocumentedTrialBalanceLine()
    {
        const string json = """
            {
              "category": "https://api.freeagent.com/v2/categories/750",
              "nominal_code": "750-65345",
              "display_nominal_code": "750-1",
              "name": "Bank Account Default bank account",
              "total": "-12860.0",
              "bank_account": "https://api.freeagent.com/v2/bank_accounts/10"
            }
            """;

        var line = JsonSerializer.Deserialize<TrialBalanceLine>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(line);
        Assert.Equal("750-1", line.DisplayNominalCode);
        Assert.Equal(-12860m, line.Total);
        Assert.Equal(10L, line.BankAccountId);
    }
}
