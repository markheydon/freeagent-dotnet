using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.SelfAssessmentReturns;

namespace FreeAgent.Client.Tests.Models.SelfAssessmentReturns;

public class SelfAssessmentReturnModelSerializationTests
{
    [Fact]
    public void Deserialize_MapsDocumentedSelfAssessmentReturn()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2022-04-05",
              "period_ends_on": "2022-04-05",
              "period_starts_on": "2021-04-06",
              "payments": [
                {
                  "label": "Balancing Payment",
                  "amount_due": "-2250.0",
                  "due_on": "2023-01-31"
                }
              ],
              "filing_due_on": "2023-01-31",
              "filing_status": "filed",
              "filed_at": "2023-10-10T10:32:49.000+00:00",
              "filed_reference": "YFM7CAZY2AQXUCRCQJOVS45UZUS4JWQN"
            }
            """;

        var incomeTaxReturn = JsonSerializer.Deserialize<SelfAssessmentReturn>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(incomeTaxReturn);
        Assert.Equal(SelfAssessmentReturnFilingStatus.Filed, incomeTaxReturn.FilingStatus);
        Assert.Equal(-2250.0m, incomeTaxReturn.Payments![0].AmountDue);
        Assert.Null(incomeTaxReturn.Payments[0].Status);
        Assert.Equal("YFM7CAZY2AQXUCRCQJOVS45UZUS4JWQN", incomeTaxReturn.FiledReference);
    }
}
