using FreeAgent.Client.Infrastructure.Http;

namespace FreeAgent.Client.Tests.Infrastructure.Http;

public class FreeAgentQueryStringBuilderTests
{
    [Fact]
    public void BuildEndpoint_WithNullQueryValue_ThrowsArgumentException()
    {
        var queryParameters = new[]
        {
            new KeyValuePair<string, string>("view", null!)
        };

        var exception = Assert.Throws<ArgumentException>(() => FreeAgentQueryStringBuilder.BuildEndpoint("contacts", queryParameters));

        Assert.Equal("queryParameters", exception.ParamName);
    }
}
