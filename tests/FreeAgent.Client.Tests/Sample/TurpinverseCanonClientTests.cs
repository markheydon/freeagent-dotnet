using System.Net;
using FreeAgent.Client.BlazorSample.Services.Turpinverse;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FreeAgent.Client.Tests.Sample;

public class TurpinverseCanonClientTests
{
    [Fact]
    public async Task LoadJsonAsync_UsesLocalFallbackWhenGitHubIsUnreachable()
    {
        using var tempRoot = new TempDirectory();
        var fallbackDirectory = Path.Combine(tempRoot.Root, "Data", "canon-fallback");
        Directory.CreateDirectory(fallbackDirectory);
        await File.WriteAllTextAsync(
            Path.Combine(fallbackDirectory, "organisations.json"),
            """
            [
              {
                "id": "turpin-enterprises",
                "tradingName": "Turpin Enterprises",
                "primaryContactId": "dick-turpin"
              }
            ]
            """);

        var handler = new StaticResponseHandler(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        var client = CreateClient(handler, tempRoot.Root);

        var organisations = await client.LoadJsonAsync<TurpinverseOrganisation[]>("organisations.json");

        Assert.NotNull(organisations);
        Assert.Single(organisations!);
        Assert.Equal("turpin-enterprises", organisations![0].Id);
    }

    [Fact]
    public async Task LoadJsonAsync_ThrowsTurpinverseCanonFetchExceptionWhenGitHubAndFallbackFail()
    {
        using var tempRoot = new TempDirectory();
        var handler = new StaticResponseHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var client = CreateClient(handler, tempRoot.Root);

        var exception = await Assert.ThrowsAsync<TurpinverseCanonFetchException>(() =>
            client.LoadJsonAsync<TurpinverseOrganisation[]>("organisations.json"));

        Assert.Contains("organisations.json", exception.Message, StringComparison.Ordinal);
        Assert.Contains("raw.githubusercontent.com", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ClearCache_ForcesSubsequentReload()
    {
        using var tempRoot = new TempDirectory();
        var callCount = 0;
        var handler = new StaticResponseHandler(_ =>
        {
            callCount++;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            };
        });

        var client = CreateClient(handler, tempRoot.Root);

        await client.LoadJsonAsync<TurpinverseOrganisation[]>("organisations.json");
        client.ClearCache();
        await client.LoadJsonAsync<TurpinverseOrganisation[]>("organisations.json");

        Assert.Equal(2, callCount);
    }

    private static TurpinverseCanonClient CreateClient(HttpMessageHandler handler, string contentRootPath)
    {
        var options = Options.Create(new TurpinverseCanonOptions
        {
            Repository = "markheydon/turpinverse",
            CanonRef = "main"
        });

        var environment = new TestWebHostEnvironment
        {
            ContentRootPath = contentRootPath,
            ContentRootFileProvider = new PhysicalFileProvider(contentRootPath)
        };

        return new TurpinverseCanonClient(new HttpClient(handler), environment, options);
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

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "FreeAgent.Client.Tests";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = Environments.Development;
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }

    private sealed class TempDirectory : IDisposable
    {
        public TempDirectory()
        {
            Root = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))).FullName;
        }

        public string Root { get; }

        public void Dispose() => Directory.Delete(Root, recursive: true);
    }
}
