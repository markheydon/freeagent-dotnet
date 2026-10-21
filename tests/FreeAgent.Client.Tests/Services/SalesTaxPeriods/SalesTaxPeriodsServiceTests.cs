using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.SalesTaxPeriods;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.SalesTaxPeriods;

public class SalesTaxPeriodsServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsSalesTaxPeriods()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/sales_tax_periods", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "sales_tax_periods": [
                    {
                      "url": "https://api.freeagent.com/v2/sales_tax_periods/1",
                      "sales_tax_name": "First Tax",
                      "sales_tax_rate_1": "2.0",
                      "sales_tax_is_value_added": true,
                      "sales_tax_registration_status": "Registered",
                      "effective_date": "2016-09-21"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var periods = await service.ListAsync();

        Assert.Single(periods);
        Assert.Equal("First Tax", periods[0].SalesTaxName);
        Assert.Equal(SalesTaxPeriodRegistrationStatus.Registered, periods[0].SalesTaxRegistrationStatus);
        Assert.Equal(new DateOnly(2016, 9, 21), periods[0].EffectiveDate);
    }

    [Fact]
    public async Task GetSalesTaxPeriodAsync_ReturnsSalesTaxPeriod()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/sales_tax_periods/17", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "sales_tax_period": {
                    "url": "https://api.freeagent.com/v2/sales_tax_periods/17",
                    "sales_tax_name": "First Tax",
                    "sales_tax_rate_1": "2.0",
                    "sales_tax_is_value_added": true,
                    "sales_tax_registration_status": "Registered",
                    "effective_date": "2016-09-21"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var period = await service.GetSalesTaxPeriodAsync(17);

        Assert.Equal("First Tax", period.SalesTaxName);
        Assert.Equal(17, period.ResourceId);
    }

    [Fact]
    public async Task CreateSalesTaxPeriodAsync_SerialisesRequiredFields()
    {
        var handler = CreatePostHandler(body =>
        {
            Assert.Contains("\"sales_tax_name\":\"SDK Tax\"", body, StringComparison.Ordinal);
            Assert.Contains("\"sales_tax_registration_status\":\"Registered\"", body, StringComparison.Ordinal);
            Assert.Contains("\"sales_tax_rate_1\":2", body, StringComparison.Ordinal);
            Assert.Contains("\"sales_tax_is_value_added\":true", body, StringComparison.Ordinal);
            Assert.Contains("\"effective_date\":\"2016-09-21\"", body, StringComparison.Ordinal);
            Assert.DoesNotContain("second_sales_tax", body, StringComparison.Ordinal);
        }, """
            {
              "sales_tax_period": {
                "url": "https://api.freeagent.com/v2/sales_tax_periods/17",
                "sales_tax_name": "SDK Tax",
                "sales_tax_rate_1": "2.0",
                "sales_tax_is_value_added": true,
                "sales_tax_registration_status": "Registered",
                "effective_date": "2016-09-21"
              }
            }
            """);

        var service = CreateService(handler);
        var created = await service.CreateSalesTaxPeriodAsync(new CreateSalesTaxPeriodRequest
        {
            SalesTaxName = "SDK Tax",
            SalesTaxRegistrationStatus = SalesTaxPeriodRegistrationStatus.Registered,
            SalesTaxRate1 = 2m,
            SalesTaxIsValueAdded = true,
            EffectiveDate = new DateOnly(2016, 9, 21)
        });

        Assert.Equal("SDK Tax", created.SalesTaxName);
    }

    [Fact]
    public async Task UpdateSalesTaxPeriodAsync_SerialisesSuppliedFields()
    {
        var handler = CreatePutHandler(17, body =>
        {
            Assert.Contains("\"sales_tax_name\":\"Updated Tax\"", body, StringComparison.Ordinal);
            Assert.Contains("\"sales_tax_rate_1\":5", body, StringComparison.Ordinal);
            Assert.DoesNotContain("effective_date", body, StringComparison.Ordinal);
        }, """
            {
              "sales_tax_period": {
                "url": "https://api.freeagent.com/v2/sales_tax_periods/17",
                "sales_tax_name": "Updated Tax",
                "sales_tax_rate_1": "5.0"
              }
            }
            """);

        var service = CreateService(handler);
        var updated = await service.UpdateSalesTaxPeriodAsync(17, new UpdateSalesTaxPeriodRequest
        {
            SalesTaxName = "Updated Tax",
            SalesTaxRate1 = 5m
        });

        Assert.Equal("Updated Tax", updated.SalesTaxName);
    }

    [Fact]
    public async Task DeleteSalesTaxPeriodAsync_UsesExpectedPath()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/sales_tax_periods/17", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var service = CreateService(handler);
        await service.DeleteSalesTaxPeriodAsync(17);
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
    public async Task GetSalesTaxPeriodAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetSalesTaxPeriodAsync(17));
    }

    private static SalesTaxPeriodsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new SalesTaxPeriodsService(client);
    }

    private static QueueHttpMessageHandler CreatePostHandler(Action<string> assertBody, string responseJson) =>
        new([(Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/sales_tax_periods", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = await request.Content!.ReadAsStringAsync();
            Assert.Contains("\"sales_tax_period\"", body, StringComparison.Ordinal);
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(responseJson)
            };
        })]);

    private static QueueHttpMessageHandler CreatePutHandler(long salesTaxPeriodId, Action<string> assertBody, string responseJson) =>
        new([(Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith($"/sales_tax_periods/{salesTaxPeriodId}", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = await request.Content!.ReadAsStringAsync();
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson)
            };
        })]);
}
