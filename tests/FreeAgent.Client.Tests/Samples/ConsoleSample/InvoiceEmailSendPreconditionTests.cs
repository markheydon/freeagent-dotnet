#if NET10_0
using System.Net;
using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample;

namespace FreeAgent.Client.Tests.Samples.ConsoleSample;

public class InvoiceEmailSendPreconditionTests
{
    [Fact]
    public void IsFailure_WhenTrialEmailAllowanceExceeded_ReturnsTrue()
    {
        var exception = new FreeAgentApiException(
            "Sending this email would exceed your allowance of 20 emails throughout the free trial period.",
            "/v2/invoices/1/send_email",
            1,
            HttpStatusCode.UnprocessableEntity);

        Assert.True(InvoiceEmailSendPrecondition.IsFailure(exception));
    }

    [Fact]
    public void IsFailure_WhenUnrelatedUnprocessableEntity_ReturnsFalse()
    {
        var exception = new FreeAgentApiException(
            "contact is required",
            "/v2/invoices",
            1,
            HttpStatusCode.UnprocessableEntity);

        Assert.False(InvoiceEmailSendPrecondition.IsFailure(exception));
    }
}
#endif
