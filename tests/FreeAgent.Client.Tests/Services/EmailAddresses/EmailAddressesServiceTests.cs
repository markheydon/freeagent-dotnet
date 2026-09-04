using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.EmailAddresses;
using FreeAgent.Client.Tests.TestSupport;
using Xunit;

namespace FreeAgent.Client.Tests.Services.EmailAddresses;

public class EmailAddressesServiceTests
{
    [Fact]
    public async Task GetEmailAddressesAsync_RequestsEmailAddressesEndpoint()
    {
        string? requestedPath = null;

        var handler = new QueueHttpMessageHandler(request =>
        {
            requestedPath = request.RequestUri?.PathAndQuery;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "email_addresses": [
                    "John Smith <jsmith@example.com>"
                  ]
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new EmailAddressesService(client);

        await service.GetEmailAddressesAsync();

        Assert.Equal("/v2/email_addresses", requestedPath);
    }

    [Fact]
    public async Task GetEmailAddressesAsync_ReturnsEmailAddresses()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "email_addresses": [
                "John Smith <jsmith@example.com>",
                "Jane Doe <jane@example.com>"
              ]
            }
            """)
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new EmailAddressesService(client);

        var emailAddresses = await service.GetEmailAddressesAsync();

        Assert.Equal(2, emailAddresses.Count);
        Assert.Contains("John Smith <jsmith@example.com>", emailAddresses);
        Assert.Contains("Jane Doe <jane@example.com>", emailAddresses);
    }

    [Fact]
    public async Task GetEmailAddressesAsync_WhenEmailAddressesMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token");
        var service = new EmailAddressesService(client);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetEmailAddressesAsync());

        Assert.Contains("Email addresses missing", exception.Message);
    }
}
