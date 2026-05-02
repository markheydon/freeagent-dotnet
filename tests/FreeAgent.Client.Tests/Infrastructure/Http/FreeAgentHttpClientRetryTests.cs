using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FreeAgent.Client.Infrastructure.Authentication;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Infrastructure.Http;

public class FreeAgentHttpClientRetryTests
{
    [Fact]
    public void FreeAgentHttpClientOptions_MinimumRequestSpacing_DefaultsToZero()
    {
        var options = new FreeAgentHttpClientOptions();

        Assert.Equal(TimeSpan.Zero, options.MinimumRequestSpacing);
    }

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
    public async Task GetAsync_WhenServerErrorRetryExhausted_ThrowsFreeAgentApiExceptionWithAttemptMetadata()
    {
        var attemptCount = 0;
        var handler = new QueueHttpMessageHandler(
            _ =>
            {
                attemptCount++;
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("temporary")
                };
            },
            _ =>
            {
                attemptCount++;
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("temporary")
                };
            },
            _ =>
            {
                attemptCount++;
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("temporary")
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

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => client.GetAsync<RetryTestPayload>("company"));

        Assert.Equal(3, attemptCount);
        Assert.Equal(3, exception.AttemptCount);
        Assert.Equal("company", exception.RequestPath);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
    }

    [Fact]
    public async Task GetAsync_WhenRateLimitedWithoutRetryAfter_UsesDefaultRetryAfterFallback()
    {
        var handler = new QueueHttpMessageHandler(_ =>
        {
            return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("rate limited")
            };
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
        Assert.Equal(TimeSpan.FromSeconds(60), exception.RetryAfter);
    }

    [Fact]
    public async Task GetAsync_WhenRateLimitedWithRetryAfterDate_ParsesToNonNegativeDelay()
    {
        var retryAfterDate = DateTimeOffset.UtcNow.AddSeconds(2);
        var handler = new QueueHttpMessageHandler(_ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("rate limited")
            };
            response.Headers.RetryAfter = new RetryConditionHeaderValue(retryAfterDate);
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

        Assert.NotNull(exception.RetryAfter);
        Assert.True(exception.RetryAfter >= TimeSpan.Zero);
        Assert.True(exception.RetryAfter <= TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetAsync_WhenNotFound_DoesNotRetry()
    {
        var attemptCount = 0;
        var handler = new QueueHttpMessageHandler(_ =>
        {
            attemptCount++;
            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("missing")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions
        {
            MaxNetworkRetries = 5,
            BaseRetryDelay = TimeSpan.Zero,
            MaxRetryDelay = TimeSpan.Zero,
            MinimumRequestSpacing = TimeSpan.Zero,
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => client.GetAsync<RetryTestPayload>("company"));

        Assert.Equal(1, attemptCount);
        Assert.Equal(1, exception.AttemptCount);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task GetAsync_WhenCancellationRequestedDuringRetryDelay_ThrowsOperationCanceledException()
    {
        var attemptCount = 0;
        using var cancellationTokenSource = new CancellationTokenSource();

        var handler = new QueueHttpMessageHandler(_ =>
        {
            attemptCount++;
            cancellationTokenSource.Cancel();
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent("temporary")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions
        {
            MaxNetworkRetries = 3,
            BaseRetryDelay = TimeSpan.FromSeconds(30),
            MaxRetryDelay = TimeSpan.FromSeconds(30),
            MinimumRequestSpacing = TimeSpan.Zero,
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.GetAsync<RetryTestPayload>("company", cancellationToken: cancellationTokenSource.Token));

        Assert.Equal(1, attemptCount);
    }

    [Fact]
    public async Task GetAsync_WhenCancellationRequestedDuringRateLimitSpacing_ThrowsOperationCanceledException()
    {
        var attemptCount = 0;
        var handler = new QueueHttpMessageHandler(
            _ =>
            {
                attemptCount++;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"value\":\"ok\"}")
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
            MaxNetworkRetries = 0,
            MinimumRequestSpacing = TimeSpan.FromSeconds(30),
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);

        var first = await client.GetAsync<RetryTestPayload>("company");
        Assert.Equal("ok", first.Value);

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            client.GetAsync<RetryTestPayload>("company", cancellationToken: cancellationTokenSource.Token));

        Assert.Equal(1, attemptCount);
    }

    [Fact]
    public async Task GetAsync_WhenTokenIsExpiringSoon_RefreshesBeforeApiCall()
    {
        var refreshCallCount = 0;
        var apiCallCount = 0;
        string? observedAuthorizationHeader = null;

        using var oauthHttpClient = new HttpClient(new LambdaHttpMessageHandler(request =>
        {
            if (!request.RequestUri!.AbsoluteUri.Contains("token_endpoint", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Expected token endpoint call.");
            }

            Interlocked.Increment(ref refreshCallCount);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"access_token\":\"refreshed-access\",\"token_type\":\"Bearer\",\"refresh_token\":\"refreshed-refresh\",\"expires_in\":3600}",
                    Encoding.UTF8,
                    "application/json")
            };
        }));

        var oauthClient = new FreeAgentOAuthClient(
            "client-id",
            "client-secret",
            "https://localhost/callback",
            oauthHttpClient,
            FreeAgentEnvironment.Production);

        var token = new OAuthTokenResponse
        {
            AccessToken = "stale-access",
            TokenType = "Bearer",
            RefreshToken = "refresh-token",
            ExpiresIn = 60,
            IssuedAt = DateTime.UtcNow
        };

        using var apiHttpClient = new HttpClient(new LambdaHttpMessageHandler(request =>
        {
            Interlocked.Increment(ref apiCallCount);
            observedAuthorizationHeader = request.Headers.Authorization?.ToString();

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"value\":\"ok\"}")
            };
        }))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions
        {
            MaxNetworkRetries = 0,
            MinimumRequestSpacing = TimeSpan.Zero,
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(apiHttpClient, oauthClient, token, FreeAgentEnvironment.Production, options);

        var response = await client.GetAsync<RetryTestPayload>("company");

        Assert.Equal("ok", response.Value);
        Assert.Equal(1, refreshCallCount);
        Assert.Equal(1, apiCallCount);
        Assert.Equal("Bearer refreshed-access", observedAuthorizationHeader);
    }

    [Fact]
    public async Task GetAsync_WhenMultipleCallsRaceTokenRefresh_RefreshesOnlyOnce()
    {
        var refreshCallCount = 0;
        var apiCallCount = 0;
        var observedAuthorizations = new List<string>();
        var syncLock = new object();

        using var oauthHttpClient = new HttpClient(new AsyncLambdaHttpMessageHandler(async request =>
        {
            if (!request.RequestUri!.AbsoluteUri.Contains("token_endpoint", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Expected token endpoint call.");
            }

            Interlocked.Increment(ref refreshCallCount);
            await Task.Delay(100);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"access_token\":\"refreshed-access\",\"token_type\":\"Bearer\",\"refresh_token\":\"refreshed-refresh\",\"expires_in\":3600}",
                    Encoding.UTF8,
                    "application/json")
            };
        }));

        var oauthClient = new FreeAgentOAuthClient(
            "client-id",
            "client-secret",
            "https://localhost/callback",
            oauthHttpClient,
            FreeAgentEnvironment.Production);

        var token = new OAuthTokenResponse
        {
            AccessToken = "stale-access",
            TokenType = "Bearer",
            RefreshToken = "refresh-token",
            ExpiresIn = 60,
            IssuedAt = DateTime.UtcNow
        };

        using var apiHttpClient = new HttpClient(new LambdaHttpMessageHandler(request =>
        {
            Interlocked.Increment(ref apiCallCount);
            var value = request.Headers.Authorization?.ToString() ?? string.Empty;

            lock (syncLock)
            {
                observedAuthorizations.Add(value);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"value\":\"ok\"}")
            };
        }))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions
        {
            MaxNetworkRetries = 0,
            MinimumRequestSpacing = TimeSpan.Zero,
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(apiHttpClient, oauthClient, token, FreeAgentEnvironment.Production, options);

        await Task.WhenAll(
            client.GetAsync<RetryTestPayload>("company"),
            client.GetAsync<RetryTestPayload>("company"),
            client.GetAsync<RetryTestPayload>("company"));

        Assert.Equal(1, refreshCallCount);
        Assert.Equal(3, apiCallCount);
        Assert.All(observedAuthorizations, value => Assert.Equal("Bearer refreshed-access", value));
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

    [Fact]
    public async Task GetAsync_WithPerRequestHeaders_AppliesHeadersOnEveryRetryAttempt()
    {
        var attemptCount = 0;
        var observedHeaderValues = new List<string>();

        var handler = new QueueHttpMessageHandler(
            request =>
            {
                attemptCount++;
                if (request.Headers.TryGetValues("X-Correlation-Id", out var values))
                {
                    observedHeaderValues.Add(values.Single());
                }

                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("{\"error\":\"temporary\"}")
                };
            },
            request =>
            {
                attemptCount++;
                if (request.Headers.TryGetValues("X-Correlation-Id", out var values))
                {
                    observedHeaderValues.Add(values.Single());
                }

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

        var response = await client.GetAsync<RetryTestPayload>(
            "company",
            [new KeyValuePair<string, string>("X-Correlation-Id", "retry-test")]);

        Assert.Equal(2, attemptCount);
        Assert.Equal("ok", response.Value);
        Assert.Equal(2, observedHeaderValues.Count);
        Assert.All(observedHeaderValues, value => Assert.Equal("retry-test", value));
    }

    [Fact]
    public async Task GetAsync_WithRateLimitTestHeader_InSandbox_AllowsRequest()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            var hasHeader = request.Headers.TryGetValues("X-RateLimit-Test", out var values);
            Assert.True(hasHeader);
            Assert.Equal("true", values?.Single());

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"value\":\"ok\"}")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.sandbox.freeagent.com/v2/")
        };

        var options = new FreeAgentHttpClientOptions
        {
            MinimumRequestSpacing = TimeSpan.Zero,
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);

        var response = await client.GetAsync<RetryTestPayload>(
            "company",
            [new KeyValuePair<string, string>("X-RateLimit-Test", "true")]);

        Assert.Equal("ok", response.Value);
    }

    [Fact]
    public async Task GetAsync_WithRateLimitTestHeader_OutsideSandbox_ThrowsInvalidOperationException()
    {
        var handler = new QueueHttpMessageHandler(_ =>
        {
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
            MinimumRequestSpacing = TimeSpan.Zero,
            UseRetryJitter = false
        };

        using var client = new FreeAgentHttpClient(httpClient, "test-token", options);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetAsync<RetryTestPayload>(
            "company",
            [new KeyValuePair<string, string>("X-RateLimit-Test", "true")]));

        Assert.Contains("only supported when targeting the sandbox environment", exception.Message, StringComparison.Ordinal);
    }

    private sealed class RetryTestPayload
    {
        public string Value { get; set; } = string.Empty;
    }

    private sealed class LambdaHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public LambdaHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_handler(request));
        }
    }

    private sealed class AsyncLambdaHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public AsyncLambdaHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _handler(request);
        }
    }
}
