#if NET10_0
using FreeAgent.Client.Models.Estimates;
using FreeAgent.Client.Samples.Shared.Turpinverse;

namespace FreeAgent.Client.Tests.Samples.Turpinverse;

public class TurpinverseQuoteMapperTests
{
    private static readonly DateOnly MinimumDate = new(2026, 1, 1);

    [Theory]
    [InlineData("Draft", EstimateStatus.Draft)]
    [InlineData("Sent", EstimateStatus.Sent)]
    [InlineData("Accepted", EstimateStatus.Approved)]
    [InlineData("Declined", EstimateStatus.Rejected)]
    [InlineData("Expired", EstimateStatus.Rejected)]
    public void MapStatus_MapsTurpinverseStatuses(string canonStatus, EstimateStatus expected)
    {
        Assert.Equal(expected, TurpinverseQuoteMapper.MapStatus(canonStatus));
    }

    [Fact]
    public void ToFreeAgentEstimate_ClampsHistoricalIssueDateToCompanyMinimum()
    {
        var estimate = TurpinverseQuoteMapper.ToFreeAgentEstimate(
            new TurpinverseQuote
            {
                QuoteId = "quote-005",
                IssueDate = "1737-09-15",
                Currency = "GBP",
                Lines = []
            },
            contactId: 1,
            projectId: null,
            minimumDocumentDate: MinimumDate);

        Assert.Equal(MinimumDate, estimate.DatedOn);
    }

    [Theory]
    [InlineData(EstimateStatus.Sent, EstimateStatus.Open, true)]
    [InlineData(EstimateStatus.Sent, EstimateStatus.Sent, true)]
    [InlineData(EstimateStatus.Approved, EstimateStatus.Open, false)]
    public void StatusesEquivalent_TreatsOpenAsSent(EstimateStatus target, EstimateStatus current, bool expected)
    {
        Assert.Equal(expected, TurpinverseEstimateStatusSupport.StatusesEquivalent(current, target));
    }
}

#endif
