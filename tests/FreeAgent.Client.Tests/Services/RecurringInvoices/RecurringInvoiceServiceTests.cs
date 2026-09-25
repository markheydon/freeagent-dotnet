using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.RecurringInvoices;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.RecurringInvoices;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.RecurringInvoices;

public class RecurringInvoiceServiceTests
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
                  "recurring_invoices": [
                    {
                      "url": "https://api.freeagent.com/v2/recurring_invoices/1",
                      "reference": "001",
                      "recurring_status": "Draft",
                      "frequency": "Weekly"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/recurring_invoices/2",
                      "reference": "002",
                      "recurring_status": "Active",
                      "frequency": "Monthly"
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
        var service = new RecurringInvoiceService(client);

        var page = await service.ListAsync(page: 1, perPage: 2, view: RecurringInvoiceViews.Draft);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal("001", page.Items[0].Reference);
        Assert.Equal(RecurringInvoiceStatus.Active, page.Items[1].RecurringStatus);
        Assert.Equal(RecurringInvoiceFrequency.Monthly, page.Items[1].Frequency);
    }

    [Fact]
    public async Task ListAsync_IncludesContactFilter()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("contact=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fcontacts%2F2", request.RequestUri!.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "recurring_invoices": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new RecurringInvoiceService(client);

        await service.ListAsync(contactId: 2);
    }

    [Fact]
    public async Task ListAsync_ContactAndContactId_Throws()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new RecurringInvoiceService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(
            contact: ContactReference.Parse("https://api.freeagent.com/v2/contacts/1"),
            contactId: 2));
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
        var service = new RecurringInvoiceService(client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.ListAsync(perPage: perPage));
    }

    [Fact]
    public async Task ListAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "recurring_invoices": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new RecurringInvoiceService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }

    [Fact]
    public async Task GetRecurringInvoiceAsync_ReturnsRecurringInvoice()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/recurring_invoices/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "recurring_invoice": {
                    "url": "https://api.freeagent.com/v2/recurring_invoices/42",
                    "reference": "007",
                    "recurring_status": "Draft",
                    "frequency": "Two Weekly",
                    "next_recurs_on": "2012-03-07",
                    "recurring_end_date": "2012-05-16"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new RecurringInvoiceService(client);

        var recurringInvoice = await service.GetRecurringInvoiceAsync(42);

        Assert.Equal("007", recurringInvoice.Reference);
        Assert.Equal(RecurringInvoiceStatus.Draft, recurringInvoice.RecurringStatus);
        Assert.Equal(RecurringInvoiceFrequency.TwoWeekly, recurringInvoice.Frequency);
        Assert.Equal(new DateOnly(2012, 3, 7), recurringInvoice.NextRecursOn);
        Assert.Equal(new DateOnly(2012, 5, 16), recurringInvoice.RecurringEndDate);
    }

    [Fact]
    public async Task GetRecurringInvoiceAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "recurring_invoice": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new RecurringInvoiceService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetRecurringInvoiceAsync(1));
    }

    [Fact]
    public async Task GetRecurringInvoiceAsync_HydratesLinkedResources()
    {
        var calls = new List<string>();
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            calls.Add(request.RequestUri!.AbsolutePath);
            if (request.RequestUri.AbsolutePath.EndsWith("/recurring_invoices/5", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "recurring_invoice": {
                        "url": "https://api.freeagent.com/v2/recurring_invoices/5",
                        "contact": "https://api.freeagent.com/v2/contacts/2",
                        "project": "https://api.freeagent.com/v2/projects/3"
                      }
                    }
                    """)
                };
            }

            if (request.RequestUri.AbsolutePath.EndsWith("/contacts/2", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "contact": {
                        "url": "https://api.freeagent.com/v2/contacts/2",
                        "organisation_name": "Example Ltd"
                      }
                    }
                    """)
                };
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "project": {
                    "url": "https://api.freeagent.com/v2/projects/3",
                    "name": "Example project"
                  }
                }
                """)
            };
        };

        var handler = new QueueHttpMessageHandler(respond, respond, respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new RecurringInvoiceService(client);

        var recurringInvoice = await service.GetRecurringInvoiceAsync(
            5,
            new RecurringInvoiceGetOptions { IncludeContact = true, IncludeProject = true });

        Assert.Equal("Example Ltd", recurringInvoice.Contact?.OrganisationName);
        Assert.Equal("Example project", recurringInvoice.Project?.Name);
        Assert.Contains(calls, path => path.Contains("/contacts/2", StringComparison.Ordinal));
        Assert.Contains(calls, path => path.Contains("/projects/3", StringComparison.Ordinal));
    }

    [Fact]
    public async Task ListAutoPagingAsync_YieldsAllPages()
    {
        var page = 0;
        Func<HttpRequestMessage, HttpResponseMessage> respond = request =>
        {
            page++;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(page == 1
                    ? """
                      {
                        "recurring_invoices": [
                          { "url": "https://api.freeagent.com/v2/recurring_invoices/1", "reference": "001" },
                          { "url": "https://api.freeagent.com/v2/recurring_invoices/2", "reference": "002" }
                        ]
                      }
                      """
                    : """
                      {
                        "recurring_invoices": [
                          { "url": "https://api.freeagent.com/v2/recurring_invoices/3", "reference": "003" }
                        ]
                      }
                      """)
            };

            if (page == 1)
            {
                response.Headers.TryAddWithoutValidation("X-Total-Count", "3");
            }

            return response;
        };

        var handler = new QueueHttpMessageHandler(respond, respond);

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new RecurringInvoiceService(client);

        var references = new List<string>();
        await foreach (var recurringInvoice in service.ListAutoPagingAsync(perPage: 2))
        {
            references.Add(recurringInvoice.Reference!);
        }

        Assert.Equal(["001", "002", "003"], references);
    }

    [Fact]
    public async Task ListAutoPagingAsync_CancellationRequested_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "recurring_invoices": [
                { "url": "https://api.freeagent.com/v2/recurring_invoices/1", "reference": "001" }
              ]
            }
            """)
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new RecurringInvoiceService(client);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in service.ListAutoPagingAsync(cancellationToken: cts.Token))
            {
            }
        });
    }
}
