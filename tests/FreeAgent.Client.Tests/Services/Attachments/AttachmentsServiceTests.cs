using System.Globalization;
using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.Attachments;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Attachments;

public class AttachmentsServiceTests
{
    [Fact]
    public async System.Threading.Tasks.Task GetAttachmentAsync_ReturnsAttachment()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/attachments/3", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "attachment": {
                    "url": "https://api.freeagent.com/v2/attachments/3",
                    "content_src": "https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png",
                    "content_src_medium": "https://example.com/medium",
                    "content_src_small": "https://example.com/small",
                    "expires_at": "2020-12-03T10:27:52.000Z",
                    "content_type": "image/png",
                    "file_name": "barcode.png",
                    "file_size": 7673,
                    "description": "Receipt for coffee"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new AttachmentsService(client);

        var attachment = await service.GetAttachmentAsync(3);

        Assert.Equal(3, attachment.ResourceId);
        Assert.Equal("image/png", attachment.ContentType);
        Assert.Equal("barcode.png", attachment.FileName);
        Assert.Equal(7673, attachment.FileSize);
        Assert.Equal(
            DateTimeOffset.Parse("2020-12-03T10:27:52.000Z", CultureInfo.InvariantCulture),
            attachment.ExpiresAt);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAttachmentAsync_MissingAttachmentBranch_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new AttachmentsService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetAttachmentAsync(3));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAttachmentAsync_InvalidAttachmentId_ThrowsArgumentOutOfRangeException()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new AttachmentsService(client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetAttachmentAsync(0));
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAttachmentAsync_DeletesAttachment()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/attachments/3", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new AttachmentsService(client);

        await service.DeleteAttachmentAsync(3);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAttachmentAsync_InvalidAttachmentId_ThrowsArgumentOutOfRangeException()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new AttachmentsService(client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.DeleteAttachmentAsync(0));
    }
}
