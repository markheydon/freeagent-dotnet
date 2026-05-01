using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Infrastructure.Http;

public class FreeAgentHttpClientRetryTests
{
    [Fact]
    public async Task GetAsync_WhenServerError_RetriesAndSucceeds()
    {
        var attemptCount = 0;
        var handler = new QueueHttpMessageHandler(
            _ =>
            {
                attemptCount++;
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("{\"error\":\"temporary\"}")
                };
            },
            _ =>
            {
                attemptCount++;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"value\":\"ok\"}")
                };
            });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions
        {
            MaxNetworkRetries = 2,
            BaseRetryDelay = TimeSpan.Zero,
            MaxRetryDelay = TimeSpan.Zero,
            MinimumRequestSpacing = TimeSpan.Zero,
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);

        var response = await client.GetAsync<RetryTestPayload>("company");

        Assert.Equal(2, attemptCount);
        Assert.Equal("ok", response.Value);
    }

    [Fact]
    public async Task GetAsync_WhenNetworkFailure_RetriesAndSucceeds()
    {
        var attemptCount = 0;
        var handler = new QueueHttpMessageHandler(
            _ =>
            {
                attemptCount++;
                throw new HttpRequestException("Transient network failure");
            },
            _ =>
            {
                attemptCount++;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"value\":\"ok\"}")
                };
            });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions
        {
            MaxNetworkRetries = 2,
            BaseRetryDelay = TimeSpan.Zero,
            MaxRetryDelay = TimeSpan.Zero,
            MinimumRequestSpacing = TimeSpan.Zero,
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);

        var response = await client.GetAsync<RetryTestPayload>("company");

        Assert.Equal(2, attemptCount);
        Assert.Equal("ok", response.Value);
    }

    [Fact]
    public async Task GetAsync_WhenRateLimited_ThrowsFreeAgentRateLimitExceptionWithRetryAfter()
    {
        var handler = new QueueHttpMessageHandler(_ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("rate limited")
            };
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(5));
            return response;
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions
        {
            MaxNetworkRetries = 0,
            MinimumRequestSpacing = TimeSpan.Zero,
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);

        var exception = await Assert.ThrowsAsync<FreeAgentRateLimitException>(() => client.GetAsync<RetryTestPayload>("company"));

        Assert.Equal(1, exception.AttemptCount);
        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
        Assert.Equal(TimeSpan.FromSeconds(5), exception.RetryAfter);
    }

    [Fact]
    public async Task GetAsync_WhenTimeout_ThrowsFreeAgentTimeoutException()
    {
        var handler = new QueueHttpMessageHandler(_ => throw new OperationCanceledException("simulated timeout"));

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions
        {
            MaxNetworkRetries = 0,
            MinimumRequestSpacing = TimeSpan.Zero,
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);

        var exception = await Assert.ThrowsAsync<FreeAgentTimeoutException>(() => client.GetAsync<RetryTestPayload>("company"));

        Assert.Equal(1, exception.AttemptCount);
        Assert.Equal("company", exception.RequestPath);
    }

    [Fact]
    public async Task PostAsync_WhenServerError_DoesNotRetryByDefault()
    {
        var attemptCount = 0;
        var handler = new QueueHttpMessageHandler(_ =>
        {
            attemptCount++;
            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("boom")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions
        {
            MaxNetworkRetries = 3,
            BaseRetryDelay = TimeSpan.Zero,
            MaxRetryDelay = TimeSpan.Zero,
            MinimumRequestSpacing = TimeSpan.Zero,
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => client.PostAsync<RetryTestPayload>("contacts", new StringContent("{}")));

        Assert.Equal(1, attemptCount);
        Assert.Equal(1, exception.AttemptCount);
        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    private sealed class RetryTestPayload
    {
        public string Value { get; set; } = string.Empty;
    }
}
