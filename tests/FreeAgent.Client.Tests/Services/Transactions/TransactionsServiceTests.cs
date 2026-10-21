using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.Transactions;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Transactions;

public class TransactionsServiceTests
{
    [Fact]
    public async Task ListAsync_WithoutFilters_CallsTransactionsEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/accounting/transactions", request.RequestUri!.AbsolutePath);
            Assert.Equal(string.Empty, request.RequestUri.Query);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(SampleListPayload)
            };
        });

        var service = CreateService(handler);
        var transactions = await service.ListAsync();

        Assert.Equal(3, transactions.Count);

        var sale = transactions[0];
        Assert.Equal(1, sale.ResourceId);
        Assert.Equal(new DateOnly(2023, 1, 2), sale.DatedOn);
        Assert.Equal(new DateTimeOffset(2023, 4, 20, 10, 12, 44, TimeSpan.Zero), sale.CreatedAt);
        Assert.Equal(new DateTimeOffset(2023, 4, 20, 10, 12, 44, TimeSpan.Zero), sale.UpdatedAt);
        Assert.Equal("Sale", sale.Description);
        Assert.Equal("750", sale.CategoryNominalCode);
        Assert.Equal("Bank Account", sale.CategoryName);
        Assert.Equal("750-1", sale.NominalCode);
        Assert.Equal(30.0m, sale.DebitValue);
        Assert.Equal("https://api.freeagent.com/v2/bank_transaction_explanations/1", sale.SourceItemUrl);
        Assert.Equal(1, sale.SourceItemId);
        Assert.NotNull(sale.ForeignCurrency);
        Assert.Null(sale.ForeignCurrency.CurrencyCode);
        Assert.Null(sale.ForeignCurrency.DebitValue);

        var bill = transactions[1];
        Assert.Equal(new DateOnly(2023, 6, 22), bill.DatedOn);
        Assert.Equal(new DateTimeOffset(2023, 7, 23, 15, 13, 24, TimeSpan.Zero), bill.UpdatedAt);
        Assert.Equal("359", bill.CategoryNominalCode);
        Assert.Equal(-30.0m, bill.DebitValue);
        Assert.Equal("https://api.freeagent.com/v2/bills/2", bill.SourceItemUrl);
        Assert.Equal(2, bill.SourceItemId);
        Assert.Equal(CurrencyCode.USD, bill.ForeignCurrency?.CurrencyCode);
        Assert.Equal(-37.61m, bill.ForeignCurrency?.DebitValue);

        var vat = transactions[2];
        Assert.Equal("819", vat.CategoryNominalCode);
        Assert.Equal(-120.0m, vat.DebitValue);
        Assert.Null(vat.SourceItemUrl);
        Assert.Null(vat.SourceItemId);
    }

    [Fact]
    public async Task ListAsync_WithFilters_BuildsQueryString()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/accounting/transactions", request.RequestUri!.AbsolutePath);
            Assert.Contains("from_date=2023-01-01", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("to_date=2023-06-30", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("nominal_code=359", request.RequestUri.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "transactions": [] }""")
            };
        });

        var service = CreateService(handler);
        var transactions = await service.ListAsync(
            fromDate: new DateOnly(2023, 1, 1),
            toDate: new DateOnly(2023, 6, 30),
            nominalCode: "359");

        Assert.Empty(transactions);
    }

    [Fact]
    public async Task ListAsync_ThrowsWhenTransactionsMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync());
        Assert.Contains("Transactions", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_CallsTransactionEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/accounting/transactions/1", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "transaction": {
                    "url": "https://api.freeagent.com/v2/accounting/transactions/1",
                    "dated_on": "2023-01-02",
                    "created_at": "2023-04-20T10:12:44.000Z",
                    "updated_at": "2023-04-20T10:12:44.000Z",
                    "description": "Sale",
                    "category": "https://api.freeagent.com/v2/categories/750",
                    "category_name": "Bank Account",
                    "nominal_code": "750-1",
                    "debit_value": "30.0",
                    "source_item_url": "https://api.freeagent.com/v2/bank_transaction_explanations/1",
                    "foreign_currency_data": {}
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var transaction = await service.GetAsync(1);

        Assert.Equal(1, transaction.ResourceId);
        Assert.Equal(new DateOnly(2023, 1, 2), transaction.DatedOn);
        Assert.Equal(30.0m, transaction.DebitValue);
        Assert.Equal("750", transaction.CategoryNominalCode);
    }

    [Fact]
    public async Task GetAsync_ThrowsWhenTransactionMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetAsync(1));
        Assert.Contains("Transaction", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_RejectsNonPositiveId()
    {
        var service = CreateService(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetAsync(0));
    }

    private static TransactionsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new TransactionsService(client);
    }

    private const string SampleListPayload = """
        {
          "transactions": [
            {
              "url": "https://api.freeagent.com/v2/accounting/transactions/1",
              "dated_on": "2023-01-02",
              "created_at": "2023-04-20T10:12:44.000Z",
              "updated_at": "2023-04-20T10:12:44.000Z",
              "description": "Sale",
              "category": "https://api.freeagent.com/v2/categories/750",
              "category_name": "Bank Account",
              "nominal_code": "750-1",
              "debit_value": "30.0",
              "source_item_url": "https://api.freeagent.com/v2/bank_transaction_explanations/1",
              "foreign_currency_data": {}
            },
            {
              "url": "https://api.freeagent.com/v2/accounting/transactions/2",
              "dated_on": "2023-06-22",
              "created_at": "2023-04-20T10:12:44.000Z",
              "updated_at": "2023-07-23T15:13:24.000Z",
              "description": "Jane Doe - Bill TEST001",
              "category": "https://api.freeagent.com/v2/categories/359",
              "category_name": "Books and Journals",
              "nominal_code": "359",
              "debit_value": "-30.0",
              "source_item_url": "https://api.freeagent.com/v2/bills/2",
              "foreign_currency_data": {
                "currency_code": "USD",
                "debit_value": "-37.61"
              }
            },
            {
              "url": "https://api.freeagent.com/v2/accounting/transactions/3",
              "dated_on": "2023-04-30",
              "created_at": "2023-04-20T13:04:22.000Z",
              "updated_at": "2023-04-20T13:04:22.000Z",
              "description": "VAT Return 04 23*",
              "category": "https://api.freeagent.com/v2/categories/819",
              "category_name": "VAT Charged",
              "nominal_code": "819",
              "debit_value": "-120.0",
              "foreign_currency_data": {}
            }
          ]
        }
        """;
}
