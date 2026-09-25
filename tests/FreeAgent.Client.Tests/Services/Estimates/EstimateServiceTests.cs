using System.Net;
using System.Net.Http;
using System.Text;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Estimates;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.Estimates;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Estimates;

public class EstimateServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsPaginatedResponse()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("page=1", request.RequestUri!.Query);
            Assert.Contains("per_page=2", request.RequestUri.Query);
            Assert.Contains("view=draft", request.RequestUri.Query);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "estimates": [
                    {
                      "url": "https://api.freeagent.com/v2/estimates/1",
                      "reference": "001",
                      "status": "Draft"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/estimates/2",
                      "reference": "002",
                      "status": "Sent"
                    }
                  ]
                }
                """)
            };
            response.Headers.TryAddWithoutValidation("X-Total-Count", "5");
            return response;
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        var page = await service.ListAsync(page: 1, perPage: 2, view: EstimateViews.Draft);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal("001", page.Items[0].Reference);
        Assert.Equal(EstimateStatus.Sent, page.Items[1].Status);
    }

    [Fact]
    public async Task ListAsync_IncludesFiltersAndNestedItems()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("from_date=2024-01-01", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("to_date=2024-03-31", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("contact=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fcontacts%2F2", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F3", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("invoice=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Finvoices%2F4", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("nested_estimate_items=true", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "estimates": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        await service.ListAsync(
            fromDate: new DateOnly(2024, 1, 1),
            toDate: new DateOnly(2024, 3, 31),
            contactId: 2,
            projectId: 3,
            invoiceId: 4,
            nestedEstimateItems: true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task ListAsync_InvalidPerPage_Throws(int perPage)
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.ListAsync(perPage: perPage));
    }

    [Fact]
    public async Task GetEstimateAsync_ReturnsEstimate()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/estimates/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "estimate": {
                    "url": "https://api.freeagent.com/v2/estimates/42",
                    "reference": "007",
                    "status": "Draft",
                    "estimate_type": "Quote"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        var estimate = await service.GetEstimateAsync(42);

        Assert.Equal("007", estimate.Reference);
        Assert.Equal(EstimateStatus.Draft, estimate.Status);
        Assert.Equal(EstimateType.Quote, estimate.EstimateType);
    }

    [Fact]
    public async Task GetEstimateAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "estimate": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetEstimateAsync(1));
    }

    [Fact]
    public async Task GetEstimatePdfAsync_DecodesBase64Content()
    {
        var expected = new byte[] { 0x25, 0x50, 0x44, 0x46 };
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/estimates/7/pdf", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($$"""
                {
                  "pdf": {
                    "content": "{{Convert.ToBase64String(expected)}}"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        var pdf = await service.GetEstimatePdfAsync(7);

        Assert.Equal(expected, pdf);
    }

    [Fact]
    public async Task CreateEstimateAsync_PostsEstimatePayload()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/estimates", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "estimate": {
                    "url": "https://api.freeagent.com/v2/estimates/3",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        var created = await service.CreateEstimateAsync(new Estimate
        {
            BillingContact = ContactReference.Parse("https://api.freeagent.com/v2/contacts/2"),
            DatedOn = new DateOnly(2024, 3, 18),
            Reference = "EST-001",
            EstimateType = EstimateType.Estimate,
            EstimateItems =
            [
                new EstimateItem
                {
                    Description = "Development",
                    ItemType = EstimateItemType.Hours,
                    Quantity = 2,
                    Price = 100
                }
            ]
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"contact\":\"https://api.freeagent.com/v2/contacts/2\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"estimate_type\":\"Estimate\"", postedJson, StringComparison.Ordinal);
        Assert.Equal(EstimateStatus.Draft, created.Status);
    }

    [Fact]
    public async Task CreateEstimateItemAsync_PostsDualRootPayload()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/estimate_items", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "estimate_item": {
                    "url": "https://api.freeagent.com/v2/estimate_items/2",
                    "description": "Development"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        var item = await service.CreateEstimateItemAsync(9, new EstimateItem
        {
            Description = "Development",
            ItemType = EstimateItemType.Hours,
            Quantity = 1,
            Price = 120
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"estimate\":\"https://api.freeagent.com/v2/estimates/9\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"item_type\":\"Hours\"", postedJson, StringComparison.Ordinal);
        Assert.Equal("Development", item.Description);
    }

    [Fact]
    public async Task UpdateEstimateAsync_IncludesDestroyOnLineItem()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/estimates/5", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "estimate": {
                    "url": "https://api.freeagent.com/v2/estimates/5",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        await service.UpdateEstimateAsync(5, new Estimate
        {
            BillingContact = ContactReference.Parse("https://api.freeagent.com/v2/contacts/2"),
            EstimateItems =
            [
                new EstimateItem
                {
                    ItemId = 12,
                    Destroy = 1
                }
            ]
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"id\":12", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"_destroy\":1", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task MarkEstimateAsSentAsync_PutsTransitionEndpoint()
    {
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            if (request.Method == HttpMethod.Put)
            {
                Assert.EndsWith("/estimates/5/transitions/mark_as_sent", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            Assert.EndsWith("/estimates/5", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "estimate": {
                    "url": "https://api.freeagent.com/v2/estimates/5",
                    "status": "Sent"
                  }
                }
                """)
            };
        };

        var handler = new QueueHttpMessageHandler(respond, respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        var estimate = await service.MarkEstimateAsSentAsync(5);

        Assert.Equal(EstimateStatus.Sent, estimate.Status);
    }

    [Fact]
    public async Task SendEstimateEmailAsync_PostsTemplateRequest()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/estimates/8/send_email", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        await service.SendEstimateEmailAsync(8, new SendEstimateEmailRequest
        {
            Email = new EstimateEmailDetails { UseTemplate = true }
        });

        Assert.Contains("\"use_template\":true", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ConvertToInvoiceAsync_ReturnsInvoicedEstimate()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/estimates/9/transitions/convert_to_invoice", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "estimate": {
                    "url": "https://api.freeagent.com/v2/estimates/9",
                    "status": "Invoiced",
                    "invoice": "https://api.freeagent.com/v2/invoices/15"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        var estimate = await service.ConvertToInvoiceAsync(9);

        Assert.Equal(EstimateStatus.Invoiced, estimate.Status);
        Assert.Equal(15, estimate.InvoiceId);
    }

    [Fact]
    public async Task GetDefaultAdditionalTextAsync_ReturnsText()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/estimates/default_additional_text", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "default_additional_text": "Please respond within 21 working days"
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        var text = await service.GetDefaultAdditionalTextAsync();

        Assert.Equal("Please respond within 21 working days", text);
    }

    [Fact]
    public async Task DeleteEstimateAsync_DeletesResource()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/estimates/3", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new EstimateService(client);

        await service.DeleteEstimateAsync(3);
    }
}
