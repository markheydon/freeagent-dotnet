using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Properties;
using FreeAgent.Client.Services.Properties;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Properties;

public class PropertiesServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsProperties()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/properties", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "properties": [
                    {
                      "url": "https://api.freeagent.com/v2/properties/4",
                      "address1": "Apartment 1901",
                      "address2": "Elliott Bay Towers",
                      "town": "Seattle",
                      "region": "Washington",
                      "postcode": "98101",
                      "country": "United Kingdom"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var items = await service.ListAsync();

        Assert.Single(items);
        Assert.Equal("Apartment 1901", items[0].Address1);
        Assert.Equal(4, items[0].ResourceId);
    }

    [Fact]
    public async Task GetPropertyAsync_ReturnsProperty()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/properties/3", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "property": {
                    "url": "https://api.freeagent.com/v2/properties/3",
                    "address1": "Wayne Manor",
                    "address2": "Wayne Manor Estate",
                    "town": "Gotham City",
                    "postcode": "12345",
                    "country": "United Kingdom"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var item = await service.GetPropertyAsync(3);

        Assert.Equal("Wayne Manor", item.Address1);
        Assert.Equal(3, item.ResourceId);
    }

    [Fact]
    public async Task CreatePropertyAsync_SerialisesAddress1()
    {
        var handler = CreatePostHandler(body =>
        {
            Assert.Contains("\"property\"", body, StringComparison.Ordinal);
            Assert.Contains("\"address1\":\"12334 Maple Boulevard\"", body, StringComparison.Ordinal);
            Assert.Contains("\"town\":\"Middleton\"", body, StringComparison.Ordinal);
        }, """
            {
              "property": {
                "url": "https://api.freeagent.com/v2/properties/6",
                "address1": "12334 Maple Boulevard",
                "town": "Middleton",
                "region": "Wisconsin",
                "country": "United Kingdom"
              }
            }
            """);

        var service = CreateService(handler);
        var created = await service.CreatePropertyAsync(new CreatePropertyRequest
        {
            Address1 = "12334 Maple Boulevard",
            Town = "Middleton",
            Region = "Wisconsin"
        });

        Assert.Equal("12334 Maple Boulevard", created.Address1);
        Assert.Equal(6, created.ResourceId);
    }

    [Fact]
    public async Task CreatePropertyAsync_OmitsCountryFromRequestBody()
    {
        var handler = CreatePostHandler(body =>
        {
            Assert.DoesNotContain("\"country\"", body, StringComparison.Ordinal);
        }, """
            {
              "property": {
                "url": "https://api.freeagent.com/v2/properties/7",
                "address1": "221B Baker Street",
                "country": "United Kingdom"
              }
            }
            """);

        var service = CreateService(handler);
        await service.CreatePropertyAsync(new CreatePropertyRequest
        {
            Address1 = "221B Baker Street",
            Town = "London",
            Postcode = "NW1 6XE"
        });
    }

    [Fact]
    public async Task UpdatePropertyAsync_SerialisesAddress2()
    {
        var handler = CreatePutHandler(6, body =>
        {
            Assert.Contains("\"address2\":\"Wilkerson Residence\"", body, StringComparison.Ordinal);
        }, """
            {
              "property": {
                "url": "https://api.freeagent.com/v2/properties/6",
                "address1": "12334 Maple Boulevard",
                "address2": "Wilkerson Residence",
                "town": "Middleton",
                "country": "United Kingdom"
              }
            }
            """);

        var service = CreateService(handler);
        var updated = await service.UpdatePropertyAsync(6, new UpdatePropertyRequest
        {
            Address2 = "Wilkerson Residence"
        });

        Assert.Equal("Wilkerson Residence", updated.Address2);
    }

    [Fact]
    public async Task DeletePropertyAsync_UsesExpectedPath()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/properties/6", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var service = CreateService(handler);
        await service.DeletePropertyAsync(6);
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
    public async Task GetPropertyAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetPropertyAsync(3));
    }

    private static PropertiesService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new PropertiesService(client);
    }

    private static QueueHttpMessageHandler CreatePostHandler(Action<string> assertBody, string responseJson) =>
        new([(Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/properties", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = await request.Content!.ReadAsStringAsync();
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(responseJson)
            };
        })]);

    private static QueueHttpMessageHandler CreatePutHandler(long propertyId, Action<string> assertBody, string responseJson) =>
        new([(Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith($"/properties/{propertyId}", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = await request.Content!.ReadAsStringAsync();
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson)
            };
        })]);
}
