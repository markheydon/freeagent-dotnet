using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.AccountLocks;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.AccountLocks;

public class AccountLocksServiceTests
{
    [Fact]
    public async Task ListAsync_CallsAccountLocksEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/account_locks", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "account_locks": [
                    {
                      "locked_to_date": "2024-03-31",
                      "user_lock": true,
                      "locked_by_description": "Nathan Barley",
                      "locked_by_url": "https://api.freeagent.com/v2/users/1"
                    }
                  ],
                  "earliest_lock_date": "2024-01-01",
                  "latest_lock_date": "2024-12-31"
                }
                """)
            };
        });

        var service = CreateService(handler);
        var result = await service.ListAsync();

        Assert.Single(result.AccountLocks);
        Assert.Equal(new DateOnly(2024, 3, 31), result.AccountLocks[0].LockedToDate);
        Assert.True(result.AccountLocks[0].UserLock);
        Assert.Equal(1L, result.AccountLocks[0].LockedByUserId);
        Assert.Equal(new DateOnly(2024, 1, 1), result.EarliestLockDate);
        Assert.Equal(new DateOnly(2024, 12, 31), result.LatestLockDate);
    }

    [Fact]
    public async Task ListAsync_ThrowsWhenAccountLocksMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    [Fact]
    public async Task SetUserAccountLockAsync_SendsDocumentedPutPayload()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal("/v2/account_locks", request.RequestUri!.AbsolutePath);

            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"locked_to_date\":\"2024-01-01\"", body, StringComparison.Ordinal);
            Assert.Contains("\"account_lock\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });

        var service = CreateService(handler);
        await service.SetUserAccountLockAsync(new DateOnly(2024, 1, 1));
    }

    [Fact]
    public async Task DeleteUserAccountLockAsync_CallsDeleteEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.Equal("/v2/account_locks", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });

        var service = CreateService(handler);
        await service.DeleteUserAccountLockAsync();
    }

    private static AccountLocksService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new AccountLocksService(client);
    }
}
