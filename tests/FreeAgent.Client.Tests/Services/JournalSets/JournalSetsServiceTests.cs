using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.JournalSets;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.JournalSets;

public class JournalSetsServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsJournalSets()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/journal_sets", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            Assert.Contains("from_date=2012-01-01", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("tag=MYAPPTAG", request.RequestUri.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "journal_sets": [
                    {
                      "url": "https://api.freeagent.com/v2/journal_sets/1",
                      "dated_on": "2011-07-28",
                      "description": "An example journal set",
                      "updated_at": "2020-02-06T11:08:28.000Z",
                      "tag": "MYAPPTAG"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var items = await service.ListAsync(
            fromDate: new DateOnly(2012, 1, 1),
            tag: "MYAPPTAG");

        Assert.Single(items);
        Assert.Equal("An example journal set", items[0].Description);
        Assert.Equal(new DateOnly(2011, 7, 28), items[0].DatedOn);
        Assert.Equal(1, items[0].ResourceId);
    }

    [Fact]
    public async Task GetJournalSetAsync_ReturnsJournalSet()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/journal_sets/12", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "journal_set": {
                    "url": "https://api.freeagent.com/v2/journal_sets/12",
                    "description": "Detail set"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var item = await service.GetJournalSetAsync(12);

        Assert.Equal("Detail set", item.Description);
        Assert.Equal(12, item.ResourceId);
    }

    [Fact]
    public async Task GetOpeningBalancesAsync_UsesOpeningBalancesPath()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/journal_sets/opening_balances", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "journal_set": {
                    "url": "https://api.freeagent.com/v2/journal_sets/1",
                    "description": "Opening Balances Journal Set",
                    "bank_accounts": [
                      {
                        "url": "https://api.freeagent.com/v2/bank_accounts/1",
                        "description": "Default bank account",
                        "debit_value": "-1000.0"
                      }
                    ]
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var opening = await service.GetOpeningBalancesAsync();

        Assert.Equal("Opening Balances Journal Set", opening.Description);
        Assert.NotNull(opening.BankAccounts);
        Assert.Single(opening.BankAccounts!);
        Assert.Equal(1, opening.BankAccounts![0].BankAccountId);
    }

    [Fact]
    public async Task CreateJournalSetAsync_SerialisesBalancedEntries()
    {
        var handler = CreatePostHandler(body =>
        {
            Assert.Contains("\"journal_set\"", body, StringComparison.Ordinal);
            Assert.Contains("\"category\":\"https://api.freeagent.com/v2/categories/001\"", body, StringComparison.Ordinal);
            Assert.Contains("\"category\":\"https://api.freeagent.com/v2/categories/901\"", body, StringComparison.Ordinal);
            Assert.Contains("\"debit_value\":-123.45", body, StringComparison.Ordinal);
            Assert.Contains("\"user\":\"https://api.freeagent.com/v2/users/1\"", body, StringComparison.Ordinal);
        }, """
            {
              "journal_set": {
                "url": "https://api.freeagent.com/v2/journal_sets/12",
                "description": "Created"
              }
            }
            """);

        var service = CreateService(handler);
        var created = await service.CreateJournalSetAsync(new CreateJournalSetRequest
        {
            Description = "Created",
            DatedOn = new DateOnly(2011, 7, 28),
            Tag = "MYAPPTAG",
            JournalEntries =
            [
                new JournalEntry
                {
                    CategoryNominalCode = "001",
                    Description = "A Sales Correction",
                    DebitValue = -123.45m
                },
                new JournalEntry
                {
                    CategoryNominalCode = "901",
                    UserId = 1,
                    Description = "Director's Capital Introduced",
                    DebitValue = 123.45m
                }
            ]
        });

        Assert.Equal("Created", created.Description);
    }

    [Fact]
    public async Task UpdateJournalSetAsync_SerialisesDestroyAndNewEntry()
    {
        var handler = CreatePutHandler(37, body =>
        {
            Assert.Contains("\"_destroy\":true", body, StringComparison.Ordinal);
            Assert.Contains("journal_sets/37/journal_entries/17", body, StringComparison.Ordinal);
            Assert.Contains("\"debit_value\":-20.0", body, StringComparison.Ordinal);
            Assert.Contains("\"category\":\"https://api.freeagent.com/v2/categories/001\"", body, StringComparison.Ordinal);
        }, """
            {
              "journal_set": {
                "url": "https://api.freeagent.com/v2/journal_sets/37",
                "description": "Updated"
              }
            }
            """);

        var service = CreateService(handler);
        var updated = await service.UpdateJournalSetAsync(37, new UpdateJournalSetRequest
        {
            Description = "Updated",
            JournalEntries =
            [
                new JournalEntry
                {
                    Url = "https://api.freeagent.com/v2/journal_sets/37/journal_entries/17",
                    Destroy = true
                },
                new JournalEntry
                {
                    Url = "https://api.freeagent.com/v2/journal_sets/37/journal_entries/18",
                    DebitValue = -20.0m
                },
                new JournalEntry
                {
                    CategoryNominalCode = "001",
                    DebitValue = -20.0m
                }
            ]
        });

        Assert.Equal("Updated", updated.Description);
    }

    [Fact]
    public async Task CreateJournalSetAsync_ThrowsWhenCapitalAssetCategoryMissingType()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var service = CreateService(handler);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateJournalSetAsync(new CreateJournalSetRequest
        {
            Description = "Invalid",
            JournalEntries =
            [
                new JournalEntry { CategoryNominalCode = "601", DebitValue = 1m },
                new JournalEntry { CategoryNominalCode = "901", DebitValue = -1m }
            ]
        }));
    }

    [Fact]
    public async Task DeleteJournalSetAsync_UsesExpectedPath()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/journal_sets/12", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var service = CreateService(handler);
        await service.DeleteJournalSetAsync(12);
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

    private static JournalSetsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new JournalSetsService(client);
    }

    private static QueueHttpMessageHandler CreatePostHandler(Action<string> assertBody, string responseJson) =>
        new([(Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/journal_sets", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = await request.Content!.ReadAsStringAsync();
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(responseJson)
            };
        })]);

    private static QueueHttpMessageHandler CreatePutHandler(long journalSetId, Action<string> assertBody, string responseJson) =>
        new([(Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith($"/journal_sets/{journalSetId}", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = await request.Content!.ReadAsStringAsync();
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson)
            };
        })]);
}
