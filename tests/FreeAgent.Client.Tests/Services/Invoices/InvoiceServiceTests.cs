using System.Net;
using System.Net.Http;
using System.Text;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.Invoices;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Invoices;

public class InvoiceServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsPaginatedResponse()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("page=1", request.RequestUri!.Query);
            Assert.Contains("per_page=2", request.RequestUri.Query);
            Assert.Contains("view=open", request.RequestUri.Query);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoices": [
                    {
                      "url": "https://api.freeagent.com/v2/invoices/1",
                      "reference": "001",
                      "status": "Open"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/invoices/2",
                      "reference": "002",
                      "status": "Draft"
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
        var service = new InvoiceService(client);

        var page = await service.ListAsync(page: 1, perPage: 2, view: InvoiceViews.Open);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal("001", page.Items[0].Reference);
        Assert.Equal(InvoiceStatus.Draft, page.Items[1].Status);
    }

    [Fact]
    public async Task ListAsync_IncludesFiltersAndNestedItems()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("sort=-updated_at", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("contact=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fcontacts%2F2", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F3", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("nested_invoice_items=true", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "invoices": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        await service.ListAsync(
            sort: "-updated_at",
            contactId: 2,
            projectId: 3,
            nestedInvoiceItems: true);
    }

    [Fact]
    public async Task GetInvoiceAsync_ReturnsInvoice()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/invoices/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/42",
                    "reference": "007",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var invoice = await service.GetInvoiceAsync(42);

        Assert.Equal("007", invoice.Reference);
        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
    }

    [Fact]
    public async Task GetInvoiceAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "invoice": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetInvoiceAsync(1));
    }

    [Fact]
    public async Task GetInvoicePdfAsync_DecodesBase64Content()
    {
        var expected = new byte[] { 0x25, 0x50, 0x44, 0x46 };
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/invoices/7/pdf", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
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
        var service = new InvoiceService(client);

        var pdf = await service.GetInvoicePdfAsync(7);

        Assert.Equal(expected, pdf);
    }

    [Fact]
    public async Task CreateInvoiceAsync_PostsInvoicePayload()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/invoices", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/3",
                    "status": "Draft"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var created = await service.CreateInvoiceAsync(new Invoice
        {
            BillingContact = ContactReference.Parse("https://api.freeagent.com/v2/contacts/2"),
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 14,
            InvoiceItems =
            [
                new InvoiceItem
                {
                    Description = "Consulting",
                    ItemType = InvoiceItemType.Hours,
                    Quantity = 2,
                    Price = 100
                }
            ]
        });

        Assert.NotNull(postedJson);
        Assert.Contains("\"contact\":\"https://api.freeagent.com/v2/contacts/2\"", postedJson, StringComparison.Ordinal);
        Assert.Contains("\"payment_terms_in_days\":14", postedJson, StringComparison.Ordinal);
        Assert.Equal(InvoiceStatus.Draft, created.Status);
    }

    [Fact]
    public async Task MarkInvoiceAsSentAsync_PutsTransitionEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/invoices/5/transitions/mark_as_sent", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/5",
                    "status": "Open"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var invoice = await service.MarkInvoiceAsSentAsync(5);

        Assert.Equal(InvoiceStatus.Open, invoice.Status);
    }

    [Fact]
    public async Task SendInvoiceEmailAsync_PostsTemplateRequest()
    {
        string? postedJson = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/invoices/8/send_email", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            postedJson = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        await service.SendInvoiceEmailAsync(8, new SendInvoiceEmailRequest
        {
            Email = new InvoiceEmailDetails { UseTemplate = true }
        });

        Assert.Contains("\"use_template\":true", postedJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ConvertToCreditNoteAsync_ReturnsCreditNote()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/invoices/9/transitions/convert_to_credit_note", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "credit_note": {
                    "url": "https://api.freeagent.com/v2/credit_notes/11"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var creditNote = await service.ConvertToCreditNoteAsync(9);

        Assert.Equal(11, creditNote.ResourceId);
    }

    [Fact]
    public async Task ListTimelineAsync_ReturnsItems()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/invoices/timeline", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice_timeline_items": [
                    {
                      "reference": "007",
                      "summary": "Payment received",
                      "dated_on": "2011-09-02",
                      "amount": "14.4"
                    }
                  ]
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var items = await service.ListTimelineAsync();

        Assert.Single(items);
        Assert.Equal("007", items[0].Reference);
    }

    [Fact]
    public async Task DefaultAdditionalText_GetUpdateDelete_UseExpectedRoutes()
    {
        var calls = new List<string>();
        HttpResponseMessage Respond(HttpRequestMessage request)
        {
            calls.Add($"{request.Method} {request.RequestUri!.AbsolutePath}");
            if (request.Method == HttpMethod.Get)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{ "default_additional_text": "Pay within 21 days" }""")
                };
            }

            if (request.Method == HttpMethod.Put)
            {
                var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
                Assert.Contains("Pay within 7 days", body, StringComparison.Ordinal);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{ "default_additional_text": "Pay within 7 days" }""")
                };
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };
        }

        var handler = new QueueHttpMessageHandler(Respond, Respond, Respond, Respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new InvoiceService(client);

        var current = await service.GetDefaultAdditionalTextAsync();
        var updated = await service.UpdateDefaultAdditionalTextAsync("Pay within 7 days");
        await service.DeleteDefaultAdditionalTextAsync();

        Assert.Equal("Pay within 21 days", current);
        Assert.Equal("Pay within 7 days", updated);
        Assert.Contains("GET /v2/invoices/default_additional_text", calls);
        Assert.Contains("PUT /v2/invoices/default_additional_text", calls);
        Assert.Contains("DELETE /v2/invoices/default_additional_text", calls);
    }
}
