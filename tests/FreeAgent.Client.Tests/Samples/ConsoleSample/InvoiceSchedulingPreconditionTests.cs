#if NET10_0
using System.Net;
using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample;

namespace FreeAgent.Client.Tests.Samples.ConsoleSample;

public class InvoiceSchedulingPreconditionTests
{
    [Theory]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.UnprocessableEntity)]
    public void IsFailure_MarkAsScheduledPath_ReturnsTrue(HttpStatusCode statusCode)
    {
        var exception = new FreeAgentApiException(
            "Request failed",
            "/v2/invoices/4/mark_as_scheduled",
            1,
            statusCode);

        Assert.True(InvoiceSchedulingPrecondition.IsFailure(exception));
    }

    [Fact]
    public void IsFailure_CannotBeMarkedAsScheduledMessage_ReturnsTrue()
    {
        var exception = new FreeAgentApiException(
            "Invoice cannot be marked as scheduled",
            "/v2/invoices/4",
            1,
            HttpStatusCode.UnprocessableEntity);

        Assert.True(InvoiceSchedulingPrecondition.IsFailure(exception));
    }

    [Fact]
    public void IsFailure_MissingInvoiceEmailTemplateOnCreate_ReturnsTrue()
    {
        var exception = new FreeAgentApiException(
            "No email template configured for send_new_invoice_emails",
            "/v2/invoices",
            1,
            HttpStatusCode.UnprocessableEntity);

        Assert.True(InvoiceSchedulingPrecondition.IsFailure(exception));
    }

    [Fact]
    public void IsFailure_MissingEmailTemplateOnNonInvoicePath_ReturnsFalse()
    {
        var exception = new FreeAgentApiException(
            "No email template configured for send_new_invoice_emails",
            "/v2/estimates/9/send_email",
            1,
            HttpStatusCode.UnprocessableEntity);

        Assert.False(InvoiceSchedulingPrecondition.IsFailure(exception));
    }

    [Fact]
    public void IsFailure_OtherStatusCode_ReturnsFalse()
    {
        var exception = new FreeAgentApiException(
            "Server error",
            "/v2/invoices/4/mark_as_scheduled",
            1,
            HttpStatusCode.InternalServerError);

        Assert.False(InvoiceSchedulingPrecondition.IsFailure(exception));
    }
}
#endif
