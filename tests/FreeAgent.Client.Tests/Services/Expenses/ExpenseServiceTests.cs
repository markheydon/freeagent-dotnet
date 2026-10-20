using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Expenses;
using FreeAgent.Client.Services.Expenses;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Expenses;

public class ExpenseServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsPaginatedResponse()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("page=1", request.RequestUri!.Query);
            Assert.Contains("per_page=2", request.RequestUri.Query);
            Assert.Contains("view=recent", request.RequestUri.Query);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "expenses": [
                    {
                      "url": "https://api.freeagent.com/v2/expenses/1",
                      "description": "First"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/expenses/2",
                      "description": "Second"
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
        var service = new ExpenseService(client);

        var page = await service.ListAsync(page: 1, perPage: 2, view: ExpenseViews.Recent);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal("First", page.Items[0].Description);
    }

    [Fact]
    public async Task ListAsync_IncludesDateAndProjectFilters()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("from_date=2024-01-01", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("to_date=2024-12-31", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("updated_since=", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("project=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fprojects%2F3", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "expenses": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ExpenseService(client);

        await service.ListAsync(
            fromDate: new DateOnly(2024, 1, 1),
            toDate: new DateOnly(2024, 12, 31),
            updatedSince: new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            projectId: 3);
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
                        "expenses": [
                          { "url": "https://api.freeagent.com/v2/expenses/1" },
                          { "url": "https://api.freeagent.com/v2/expenses/2" }
                        ]
                      }
                      """
                    : """
                      {
                        "expenses": [
                          { "url": "https://api.freeagent.com/v2/expenses/3" }
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
        var service = new ExpenseService(client);

        var ids = new List<long>();
        await foreach (var expense in service.ListAutoPagingAsync(perPage: 2))
        {
            ids.Add(expense.ResourceId);
        }

        Assert.Equal([1, 2, 3], ids);
    }

    [Fact]
    public async Task CreateExpenseAsync_PostsMileageCategory()
    {
        string? body = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "expense": {
                    "url": "https://api.freeagent.com/v2/expenses/9",
                    "category": "Mileage"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ExpenseService(client);

        var created = await service.CreateExpenseAsync(new Expense
        {
            UserId = 1,
            IsMileageCategory = true,
            DatedOn = new DateOnly(2024, 1, 2),
            Mileage = 5m,
            VehicleType = ExpenseVehicleType.Car,
            Description = "Mileage probe"
        });

        Assert.Equal(9, created.ResourceId);
        Assert.NotNull(body);
        Assert.Contains("\"category\":\"Mileage\"", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CreateExpensesAsync_PostsBatchEnvelope()
    {
        string? body = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "expenses": [
                    { "url": "https://api.freeagent.com/v2/expenses/10" },
                    { "url": "https://api.freeagent.com/v2/expenses/11" }
                  ]
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ExpenseService(client);

        var created = await service.CreateExpensesAsync(
        [
            new Expense
            {
                UserId = 1,
                CategoryNominalCode = "285",
                DatedOn = new DateOnly(2024, 1, 1),
                GrossValue = -10m,
                Description = "One"
            },
            new Expense
            {
                UserId = 1,
                CategoryNominalCode = "285",
                DatedOn = new DateOnly(2024, 1, 2),
                GrossValue = -5m,
                Description = "Two"
            }
        ]);

        Assert.Equal(2, created.Count);
        Assert.NotNull(body);
        Assert.Contains("\"expenses\":[", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetMileageSettingsAsync_ThrowsWhenPayloadMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ExpenseService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetMileageSettingsAsync());
    }

    [Fact]
    public async Task ListAsync_MissingExpensesBranch_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new ExpenseService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
    }
}
