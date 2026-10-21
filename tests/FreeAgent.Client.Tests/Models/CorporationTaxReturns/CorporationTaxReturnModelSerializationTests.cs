using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.CorporationTaxReturns;

namespace FreeAgent.Client.Tests.Models.CorporationTaxReturns;

public class CorporationTaxReturnModelSerializationTests
{
    [Fact]
    public void Deserialize_MapsDocumentedCorporationTaxReturn()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31",
              "period_ends_on": "2022-12-31",
              "period_starts_on": "2022-01-01",
              "amount_due": "0.0",
              "payment_due_on": "2023-10-01",
              "filing_due_on": "2023-12-31",
              "filing_status": "filed",
              "filed_at": "2023-10-09T17:13:34.119+00:00",
              "filed_reference": "4CQXAEAVNA656F6XM3DHYBF6Y37I3S43"
            }
            """;

        var corporationTaxReturn = JsonSerializer.Deserialize<CorporationTaxReturn>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(corporationTaxReturn);
        Assert.Equal(CorporationTaxReturnFilingStatus.Filed, corporationTaxReturn.FilingStatus);
        Assert.Equal(0.0m, corporationTaxReturn.AmountDue);
        Assert.Equal(new DateOnly(2023, 10, 1), corporationTaxReturn.PaymentDueOn);
        Assert.Equal("4CQXAEAVNA656F6XM3DHYBF6Y37I3S43", corporationTaxReturn.FiledReference);
    }
}
