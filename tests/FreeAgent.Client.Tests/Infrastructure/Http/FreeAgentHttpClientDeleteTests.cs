using System.Net;
using System.Net.Http;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Timeslips;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Infrastructure.Http;

public class FreeAgentHttpClientDeleteTests
{
    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WithResponseBody_DeserializesPayload()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/timeslips/25/timer", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "timeslip": {
                    "url": "https://api.freeagent.com/v2/timeslips/25",
                    "hours": "12.5"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });

        var response = await client.DeleteAsync<TimeslipResponse>("timeslips/25/timer");

        Assert.Equal(12.5m, response.Timeslip!.Hours);
    }
}
