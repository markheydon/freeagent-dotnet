using System.Net;
using FreeAgent.Client.BlazorSample.Services.Turpinverse;
using Microsoft.Extensions.Options;

namespace FreeAgent.Client.Tests.Sample;

public class TurpinverseCanonClientTests
{
    [Fact]
    public async Task LoadJsonAsync_ThrowsTurpinverseCanonFetchExceptionWhenGitHubReturnsNotFound()
    {
        var handler = new StaticResponseHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var client = CreateClient(handler);

        var exception = await Assert.ThrowsAsync<TurpinverseCanonFetchException>(() =>
            client.LoadJsonAsync<TurpinverseOrganisation[]>("organisations.json"));

        Assert.Contains("organisations.json", exception.Message, StringComparison.Ordinal);
        Assert.Contains("raw.githubusercontent.com", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task LoadJsonAsync_ThrowsTurpinverseCanonFetchExceptionWhenGitHubIsUnreachable()
    {
        var handler = new StaticResponseHandler(_ => throw new HttpRequestException("Network unreachable."));
        var client = CreateClient(handler);

        var exception = await Assert.ThrowsAsync<TurpinverseCanonFetchException>(() =>
            client.LoadJsonAsync<TurpinverseOrganisation[]>("organisations.json"));

        Assert.Contains("Could not reach GitHub", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ClearCache_ForcesSubsequentReload()
    {
        var callCount = 0;
        var handler = new StaticResponseHandler(_ =>
        {
            callCount++;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            };
        });

        var client = CreateClient(handler);

        await client.LoadJsonAsync<TurpinverseOrganisation[]>("organisations.json");
        client.ClearCache();
        await client.LoadJsonAsync<TurpinverseOrganisation[]>("organisations.json");

        Assert.Equal(2, callCount);
    }

    private static TurpinverseCanonClient CreateClient(HttpMessageHandler handler)
    {
        var options = Options.Create(new TurpinverseCanonOptions
        {
            Repository = "markheydon/turpinverse",
            CanonRef = "main"
        });

        return new TurpinverseCanonClient(new HttpClient(handler), options);
    }

    private sealed class StaticResponseHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public StaticResponseHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(_responder(request));
    }
}
