using System.Globalization;
using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.BankFeeds;
using FreeAgent.Client.Services.BankFeeds;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.BankFeeds;

public class BankFeedsServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsBankFeeds()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/bank_feeds", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_feeds": [
                    {
                      "url": "https://api.freeagent.com/v2/bank_feeds/7",
                      "bank_account": "https://api.freeagent.com/v2/bank_accounts/22",
                      "state": "enabled",
                      "created_at": "2020-11-17T09:50:02.000Z",
                      "updated_at": "2020-11-17T09:50:02.000Z",
                      "feed_type": "api",
                      "bank_service_name": "Mettle",
                      "sca_expires_at": "2021-01-01T01:01:01.000Z"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/bank_feeds/8",
                      "bank_account": "https://api.freeagent.com/v2/bank_accounts/23",
                      "state": "enabled",
                      "created_at": "2020-11-17T09:50:02.000Z",
                      "updated_at": "2020-11-17T09:50:02.000Z",
                      "feed_type": "open_banking",
                      "bank_service_name": "HSBC"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var bankFeeds = await service.ListAsync();

        Assert.Equal(2, bankFeeds.Count);
        Assert.Equal(7, bankFeeds[0].ResourceId);
        Assert.Equal(22, bankFeeds[0].BankAccountId);
        Assert.Equal(BankFeedState.Enabled, bankFeeds[0].State);
        Assert.Equal(BankFeedType.Api, bankFeeds[0].FeedType);
        Assert.Equal("Mettle", bankFeeds[0].BankServiceName);
        Assert.Equal(
            DateTimeOffset.Parse("2021-01-01T01:01:01.000Z", CultureInfo.InvariantCulture),
            bankFeeds[0].ScaExpiresAt);
        Assert.Equal(BankFeedType.OpenBanking, bankFeeds[1].FeedType);
        Assert.Null(bankFeeds[1].ScaExpiresAt);
    }

    [Fact]
    public async Task ListAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    [Fact]
    public async Task GetBankFeedAsync_ReturnsBankFeed()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/bank_feeds/7", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_feed": {
                    "url": "https://api.freeagent.com/v2/bank_feeds/7",
                    "bank_account": "https://api.freeagent.com/v2/bank_accounts/22",
                    "state": "enabled",
                    "created_at": "2020-11-17T09:50:02.000Z",
                    "updated_at": "2020-11-17T09:50:02.000Z",
                    "feed_type": "api",
                    "bank_service_name": "Mettle",
                    "sca_expires_at": "2021-01-01T01:01:01.000Z"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var bankFeed = await service.GetBankFeedAsync(7);

        Assert.Equal(7, bankFeed.ResourceId);
        Assert.Equal(22, bankFeed.BankAccountId);
        Assert.Equal(BankFeedType.Api, bankFeed.FeedType);
    }

    [Fact]
    public async Task GetBankFeedAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetBankFeedAsync(1));
    }

    [Fact]
    public async Task GetBankFeedAsync_WithHydration_FetchesBankAccount()
    {
        var handler = new QueueHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_feed": {
                    "url": "https://api.freeagent.com/v2/bank_feeds/7",
                    "bank_account": "https://api.freeagent.com/v2/bank_accounts/22"
                  }
                }
                """)
            },
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_account": {
                    "url": "https://api.freeagent.com/v2/bank_accounts/22",
                    "name": "Business account"
                  }
                }
                """)
            });

        var service = CreateService(handler);
        var bankFeed = await service.GetBankFeedAsync(7, new BankFeedGetOptions { IncludeBankAccount = true });

        Assert.Equal("Business account", bankFeed.BankAccount?.Name);
    }

    [Fact]
    public async Task GetBankFeedAsync_InvalidId_Throws()
    {
        var service = CreateService(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetBankFeedAsync(0));
    }

    private static BankFeedsService CreateService(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        return new BankFeedsService(new FreeAgentHttpClient(httpClient, "token"));
    }
}
