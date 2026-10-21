using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.CapitalAssetTypes;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.CapitalAssetTypes;

public class CapitalAssetTypesServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsCapitalAssetTypes()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/capital_asset_types", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "capital_asset_types": [
                    {
                      "url": "https://api.freeagent.com/v2/capital_asset_types/397",
                      "name": "Computer Equipment",
                      "system_default": true,
                      "created_at": "2020-04-22T11:32:24.000Z",
                      "updated_at": "2020-04-22T11:32:24.000Z"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var items = await service.ListAsync();

        Assert.Single(items);
        Assert.Equal("Computer Equipment", items[0].Name);
        Assert.True(items[0].SystemDefault);
        Assert.Equal(397, items[0].ResourceId);
    }

    [Fact]
    public async Task GetCapitalAssetTypeAsync_ReturnsCapitalAssetType()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/capital_asset_types/397", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "capital_asset_type": {
                    "url": "https://api.freeagent.com/v2/capital_asset_types/397",
                    "name": "Computer Equipment",
                    "system_default": true
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var item = await service.GetCapitalAssetTypeAsync(397);

        Assert.Equal("Computer Equipment", item.Name);
        Assert.Equal(397, item.ResourceId);
    }

    [Fact]
    public async Task CreateCapitalAssetTypeAsync_SerialisesName()
    {
        var handler = CreatePostHandler(body =>
        {
            Assert.Contains("\"capital_asset_type\"", body, StringComparison.Ordinal);
            Assert.Contains("\"name\":\"Spaceships\"", body, StringComparison.Ordinal);
        }, """
            {
              "capital_asset_type": {
                "url": "https://api.freeagent.com/v2/capital_asset_types/401",
                "name": "Spaceships",
                "system_default": false
              }
            }
            """);

        var service = CreateService(handler);
        var created = await service.CreateCapitalAssetTypeAsync(new CreateCapitalAssetTypeRequest { Name = "Spaceships" });

        Assert.Equal("Spaceships", created.Name);
        Assert.False(created.SystemDefault);
    }

    [Fact]
    public async Task UpdateCapitalAssetTypeAsync_SerialisesName()
    {
        var handler = CreatePutHandler(401, body =>
        {
            Assert.Contains("\"name\":\"Spacetrains\"", body, StringComparison.Ordinal);
        }, """
            {
              "capital_asset_type": {
                "url": "https://api.freeagent.com/v2/capital_asset_types/401",
                "name": "Spacetrains",
                "system_default": false
              }
            }
            """);

        var service = CreateService(handler);
        var updated = await service.UpdateCapitalAssetTypeAsync(401, new UpdateCapitalAssetTypeRequest { Name = "Spacetrains" });

        Assert.Equal("Spacetrains", updated.Name);
    }

    [Fact]
    public async Task DeleteCapitalAssetTypeAsync_UsesExpectedPath()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/capital_asset_types/401", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var service = CreateService(handler);
        await service.DeleteCapitalAssetTypeAsync(401);
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
    public async Task GetCapitalAssetTypeAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetCapitalAssetTypeAsync(397));
    }

    private static CapitalAssetTypesService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new CapitalAssetTypesService(client);
    }

    private static QueueHttpMessageHandler CreatePostHandler(Action<string> assertBody, string responseJson) =>
        new([(Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/capital_asset_types", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = await request.Content!.ReadAsStringAsync();
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(responseJson)
            };
        })]);

    private static QueueHttpMessageHandler CreatePutHandler(long capitalAssetTypeId, Action<string> assertBody, string responseJson) =>
        new([(Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith($"/capital_asset_types/{capitalAssetTypeId}", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = await request.Content!.ReadAsStringAsync();
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson)
            };
        })]);
}
