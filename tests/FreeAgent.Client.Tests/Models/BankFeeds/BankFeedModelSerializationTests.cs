using System.Globalization;
using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankFeeds;

namespace FreeAgent.Client.Tests.Models.BankFeeds;

public class BankFeedModelSerializationTests
{
    [Fact]
    public void Deserialise_ListEnvelope_MapsLinkedResourceIdsAndFeedType()
    {
        const string json = """
        {
          "bank_feeds": [
            {
              "url": "https://api.freeagent.com/v2/bank_feeds/7",
              "bank_account": "https://api.freeagent.com/v2/bank_accounts/22",
              "state": "enabled",
              "created_at": "2020-11-17T09:50:02.000Z",
              "updated_at": "2020-11-17T09:50:02.000Z",
              "feed_type": "open_banking",
              "bank_service_name": "HSBC"
            }
          ]
        }
        """;

        var response = JsonSerializer.Deserialize<BankFeedsListResponse>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(response?.BankFeeds);
        var bankFeed = Assert.Single(response.BankFeeds);
        Assert.Equal(7, bankFeed.ResourceId);
        Assert.Equal(22, bankFeed.BankAccountId);
        Assert.Equal(BankFeedType.OpenBanking, bankFeed.FeedType);
        Assert.Equal("HSBC", bankFeed.BankServiceName);
        Assert.Null(bankFeed.ScaExpiresAt);
    }

    [Fact]
    public void Deserialise_SingleEnvelope_MapsApiFeedAndScaExpiry()
    {
        const string json = """
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
        """;

        var response = JsonSerializer.Deserialize<BankFeedResponse>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(response?.BankFeed);
        Assert.Equal(BankFeedType.Api, response.BankFeed.FeedType);
        Assert.Equal(
            DateTimeOffset.Parse("2021-01-01T01:01:01.000Z", CultureInfo.InvariantCulture),
            response.BankFeed.ScaExpiresAt);
    }
}
