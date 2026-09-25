#if NET10_0
using System.Net;
using System.Text;
using FreeAgent.Client;
using FreeAgent.Client.Samples.Shared.Turpinverse;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Samples.Turpinverse;

public class TurpinverseCompanyDatesTests
{
    [Fact]
    public async Task GetMinimumDocumentDateAsync_UsesCompanyStartDate()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """
                {
                  "company": {
                    "company_start_date": "2026-03-15"
                  }
                }
                """,
                Encoding.UTF8,
                "application/json")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.sandbox.freeagent.com/v2/") };
        using var client = new FreeAgentClient(
            httpClient,
            "test-token",
            new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var companyDates = new TurpinverseCompanyDates();

        var minimumDate = await companyDates.GetMinimumDocumentDateAsync(client);

        Assert.Equal(new DateOnly(2026, 3, 15), minimumDate);
    }

    [Fact]
    public async Task ClearCache_RefetchesCompanyStartDate()
    {
        HttpResponseMessage CreateResponse() => new(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """{ "company": { "company_start_date": "2026-02-01" } }""",
                Encoding.UTF8,
                "application/json")
        };

        var handler = new QueueHttpMessageHandler(
            Enumerable.Range(0, 3)
                .Select(_ => (Func<HttpRequestMessage, HttpResponseMessage>)(_ => CreateResponse()))
                .ToArray());

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.sandbox.freeagent.com/v2/") };
        using var client = new FreeAgentClient(
            httpClient,
            "test-token",
            new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var companyDates = new TurpinverseCompanyDates();

        var first = await companyDates.GetMinimumDocumentDateAsync(client);
        var cached = await companyDates.GetMinimumDocumentDateAsync(client);
        Assert.Equal(first, cached);

        companyDates.ClearCache();

        var refreshed = await companyDates.GetMinimumDocumentDateAsync(client);
        Assert.Equal(first, refreshed);
    }
}

#endif
