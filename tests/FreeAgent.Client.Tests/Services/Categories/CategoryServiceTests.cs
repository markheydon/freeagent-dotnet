using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Services.Categories;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Categories;

public class CategoryServiceTests
{
    [Fact]
    public async Task GetCategoriesAsync_ReturnsAllSets()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/categories", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            Assert.DoesNotContain("sub_accounts", request.RequestUri.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "admin_expenses_categories": [
                    {
                      "url": "https://api.freeagent.com/v2/categories/285",
                      "description": "Accommodation and Meals",
                      "nominal_code": "285",
                      "allowable_for_tax": true,
                      "tax_reporting_name": "Travel and subsistence expenses",
                      "auto_sales_tax_rate": "Standard rate"
                    }
                  ],
                  "cost_of_sales_categories": [],
                  "income_categories": [
                    {
                      "url": "https://api.freeagent.com/v2/categories/001",
                      "description": "Sales",
                      "nominal_code": "001",
                      "auto_sales_tax_rate": "Standard rate"
                    }
                  ],
                  "general_categories": []
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        var categories = await service.GetCategoriesAsync();

        Assert.Single(categories.AdminExpensesCategories);
        Assert.Empty(categories.CostOfSalesCategories);
        Assert.Single(categories.IncomeCategories);
        Assert.Empty(categories.GeneralCategories);
        Assert.Equal(2, categories.AllCategories.Count);
        Assert.Equal("285", categories.AdminExpensesCategories[0].NominalCode);
        Assert.Equal(CategoryAutoSalesTaxRate.StandardRate, categories.AdminExpensesCategories[0].AutoSalesTaxRate);
    }

    [Fact]
    public async Task GetCategoriesAsync_WhenIncludeSubAccounts_AddsQueryParameter()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("sub_accounts=true", request.RequestUri!.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "admin_expenses_categories": [],
                  "cost_of_sales_categories": [],
                  "income_categories": [],
                  "general_categories": []
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        await service.GetCategoriesAsync(includeSubAccounts: true);
    }

    [Fact]
    public async Task GetCategoryAsync_ReturnsCategory()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/categories/001", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "income_categories": {
                    "url": "https://api.freeagent.com/v2/categories/001",
                    "description": "Sales",
                    "nominal_code": "001",
                    "auto_sales_tax_rate": "Standard rate"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        var category = await service.GetCategoryAsync("001");

        Assert.Equal("Sales", category.Description);
        Assert.Equal(CategoryAutoSalesTaxRate.StandardRate, category.AutoSalesTaxRate);
    }

    [Fact]
    public async Task GetCategoryAsync_WithSubAccountNominalCode_UsesPathSegment()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/categories/602-1", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "cost_of_sales_categories": {
                    "url": "https://api.freeagent.com/v2/categories/602-1",
                    "description": "Sub account",
                    "nominal_code": "602-1"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        var category = await service.GetCategoryAsync("602-1");

        Assert.Equal("602-1", category.NominalCode);
    }

    [Fact]
    public async Task CreateCategoryAsync_PostsCategoryEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/categories", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"category\"", body, StringComparison.Ordinal);
            Assert.Contains("\"category_group\":\"income\"", body, StringComparison.Ordinal);
            Assert.Contains("\"nominal_code\":\"047\"", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "income_categories": {
                    "url": "https://api.freeagent.com/v2/categories/047",
                    "description": "Custom Income Category",
                    "group_description": "Income (normally VATable)",
                    "nominal_code": "047",
                    "auto_sales_tax_rate": "Standard rate"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        var created = await service.CreateCategoryAsync(new CategoryWritePayload
        {
            Description = "Custom Income Category",
            NominalCode = "047",
            CategoryGroup = CategoryGroup.Income
        });

        Assert.Equal("047", created.NominalCode);
        Assert.Equal("Custom Income Category", created.Description);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenCategoryGroupMissing_ThrowsArgumentException()
    {
        using var httpClient = new HttpClient(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Created)))
        {
            BaseAddress = new Uri("https://api.freeagent.com/v2/")
        };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCategoryAsync(new CategoryWritePayload
        {
            Description = "Missing group",
            NominalCode = "047"
        }));
    }

    [Fact]
    public async Task UpdateCategoryAsync_PutsCategoryEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/categories/047", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"description\":\"Renamed\"", body, StringComparison.Ordinal);
            Assert.DoesNotContain("category_group", body, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "income_categories": {
                    "url": "https://api.freeagent.com/v2/categories/047",
                    "description": "Renamed",
                    "nominal_code": "047",
                    "auto_sales_tax_rate": "Standard rate"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        var updated = await service.UpdateCategoryAsync("047", new CategoryWritePayload
        {
            Description = "Renamed",
            NominalCode = "047"
        });

        Assert.Equal("Renamed", updated.Description);
    }

    [Fact]
    public async Task DeleteCategoryAsync_SendsDelete()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/categories/047", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        await service.DeleteCategoryAsync("047");
    }

    [Fact]
    public async Task GetCategoriesAsync_WhenPayloadMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetCategoriesAsync());
    }

    [Fact]
    public async Task GetCategoryAsync_WhenPayloadMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "income_categories": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetCategoryAsync("001"));
    }

    [Fact]
    public async Task GetCategoriesAsync_DeserializesAllowableForTaxFromString()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "admin_expenses_categories": [
                {
                  "url": "https://api.freeagent.com/v2/categories/213",
                  "description": "Custom Admin Expenses Category",
                  "nominal_code": "213",
                  "allowable_for_tax": "true",
                  "auto_sales_tax_rate": "Standard rate"
                }
              ],
              "cost_of_sales_categories": [],
              "income_categories": [],
              "general_categories": []
            }
            """)
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        var categories = await service.GetCategoriesAsync();

        Assert.True(categories.AdminExpensesCategories[0].AllowableForTax);
    }
}
