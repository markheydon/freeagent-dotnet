#if NET10_0
using System.Net;
using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample;

namespace FreeAgent.Client.Tests.Samples.ConsoleSample;

public class LinkedResourceVerificationCriteriaTests
{
    [Theory]
    [InlineData(HttpStatusCode.BadRequest, "contact_id can't be blank")]
    [InlineData(HttpStatusCode.UnprocessableEntity, "Contact cannot be blank")]
    [InlineData(HttpStatusCode.BadRequest, "billing contact_id cannot be empty")]
    public void IsProjectContactIdBlankRejection_MatchingMessageAndStatus_ReturnsTrue(
        HttpStatusCode statusCode,
        string message)
    {
        var exception = new FreeAgentApiException(message, "/v2/projects/1", 1, statusCode);

        Assert.True(LinkedResourceVerificationCriteria.IsProjectContactIdBlankRejection(exception));
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, "project name can't be blank")]
    [InlineData(HttpStatusCode.UnprocessableEntity, "contact is required")]
    [InlineData(HttpStatusCode.InternalServerError, "contact_id can't be blank")]
    [InlineData(HttpStatusCode.Forbidden, "contact_id can't be blank")]
    public void IsProjectContactIdBlankRejection_NonMatching_ReturnsFalse(
        HttpStatusCode statusCode,
        string message)
    {
        var exception = new FreeAgentApiException(message, "/v2/projects/1", 1, statusCode);

        Assert.False(LinkedResourceVerificationCriteria.IsProjectContactIdBlankRejection(exception));
    }

    [Fact]
    public void IsProjectContactIdBlankRejection_NullException_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => LinkedResourceVerificationCriteria.IsProjectContactIdBlankRejection(null!));
    }
}
#endif
