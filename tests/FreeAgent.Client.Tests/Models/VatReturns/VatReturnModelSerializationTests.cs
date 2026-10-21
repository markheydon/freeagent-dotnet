using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Tests.Models.VatReturns;

public class VatReturnModelSerializationTests
{
    [Fact]
    public void Deserialize_MapsDocumentedVatReturn()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/vat_returns/2023-07-31",
              "period_ends_on": "2023-07-31",
              "period_starts_on": "2023-05-01",
              "payments": [
                {
                  "label": "Payment Due",
                  "amount_due": "5450.91",
                  "due_on": "2023-09-07",
                  "status": "unpaid"
                }
              ],
              "filing_due_on": "2023-09-07",
              "filing_status": "filed",
              "filed_at": "2023-10-10T10:32:49.000+00:00",
              "filed_reference": "00000000001"
            }
            """;

        var vatReturn = JsonSerializer.Deserialize<VatReturn>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(vatReturn);
        Assert.Equal(VatReturnFilingStatus.Filed, vatReturn.FilingStatus);
        Assert.Equal(VatReturnPaymentStatus.Unpaid, vatReturn.Payments![0].Status);
        Assert.Equal(5450.91m, vatReturn.Payments[0].AmountDue);
    }
}
