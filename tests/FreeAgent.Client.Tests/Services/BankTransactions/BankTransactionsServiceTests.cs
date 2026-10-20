using System.Net;
using System.Net.Http;
using System.Text.Json;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.BankTransactions;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Services.BankTransactions;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.BankTransactions;

public class BankTransactionsServiceTests
{
    [Fact]
    public async Task ListAsync_RequiresBankAccountAndReturnsPage()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("bank_account=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fbank_accounts%2F1", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("page=1", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("per_page=2", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("view=unexplained", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("from_date=2020-07-01", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("to_date=2020-07-31", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("updated_since=", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("last_uploaded=true", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.StartsWith("/v2/bank_transactions", request.RequestUri.AbsolutePath, StringComparison.Ordinal);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_transactions": [
                    {
                      "url": "https://api.freeagent.com/v2/bank_transactions/8",
                      "amount": "-730.0",
                      "dated_on": "2020-07-06",
                      "description": "First"
                    },
                    {
                      "url": "https://api.freeagent.com/v2/bank_transactions/15",
                      "description": "Second"
                    }
                  ]
                }
                """)
            };
            response.Headers.TryAddWithoutValidation("X-Total-Count", "5");
            return response;
        });

        var service = CreateService(handler);
        var page = await service.ListAsync(
            page: 1,
            perPage: 2,
            bankAccountId: 1,
            view: BankTransactionView.Unexplained,
            fromDate: new DateOnly(2020, 7, 1),
            toDate: new DateOnly(2020, 7, 31),
            updatedSince: new DateTimeOffset(2020, 7, 1, 9, 0, 0, TimeSpan.Zero),
            lastUploaded: true);

        Assert.Equal(1, page.Page);
        Assert.Equal(2, page.PerPage);
        Assert.Equal(5, page.Total);
        Assert.True(page.HasNextPage);
        Assert.Equal(-730.0m, page.Items[0].Amount);
        Assert.Equal(new DateOnly(2020, 7, 6), page.Items[0].DatedOn);
        Assert.Equal("First", page.Items[0].Description);
    }

    [Fact]
    public async Task ListAsync_AcceptsBankAccountReference()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("bank_account=https%3A%2F%2Fapi.sandbox.freeagent.com%2Fv2%2Fbank_accounts%2F4", request.RequestUri!.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "bank_transactions": [] }""")
            };
        });

        var service = CreateService(handler, FreeAgentEnvironment.Sandbox);
        var reference = BankAccountReference.ForEnvironment(FreeAgentEnvironment.Sandbox, 4);

        var page = await service.ListAsync(bankAccount: reference);

        Assert.Empty(page.Items);
    }

    [Fact]
    public async Task ListAsync_MissingBankAccount_Throws()
    {
        var service = CreateService(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync());
    }

    [Fact]
    public async Task ListAsync_BothBankAccountArguments_Throws()
    {
        var service = CreateService(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));
        var reference = BankAccountReference.ForEnvironment(FreeAgentEnvironment.Production, 1);

        await Assert.ThrowsAsync<ArgumentException>(() => service.ListAsync(bankAccount: reference, bankAccountId: 1));
    }

    [Fact]
    public async Task ListAsync_PerPageAboveMaximum_Throws()
    {
        var service = CreateService(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.ListAsync(perPage: 101, bankAccountId: 1));
    }

    [Fact]
    public async Task ListAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ }""")
        });

        var service = CreateService(handler);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync(bankAccountId: 1));
        Assert.Contains("Bank transactions data missing", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListAutoPagingAsync_YieldsAllPages()
    {
        var page = 0;
        Func<HttpRequestMessage, HttpResponseMessage> respond = _ =>
        {
            page++;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(page == 1
                    ? """
                      {
                        "bank_transactions": [
                          { "url": "https://api.freeagent.com/v2/bank_transactions/1" },
                          { "url": "https://api.freeagent.com/v2/bank_transactions/2" }
                        ]
                      }
                      """
                    : """
                      {
                        "bank_transactions": [
                          { "url": "https://api.freeagent.com/v2/bank_transactions/3" }
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

        var service = CreateService(new QueueHttpMessageHandler(respond, respond));
        var ids = new List<long>();
        await foreach (var transaction in service.ListAutoPagingAsync(perPage: 2, bankAccountId: 1))
        {
            ids.Add(transaction.ResourceId);
        }

        Assert.Equal([1L, 2L, 3L], ids);
    }

    [Fact]
    public async Task ListAutoPagingAsync_CancellationRequested_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "bank_transactions": [
                { "url": "https://api.freeagent.com/v2/bank_transactions/1" }
              ]
            }
            """)
        });

        var service = CreateService(handler);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in service.ListAutoPagingAsync(bankAccountId: 1, cancellationToken: cts.Token))
            {
            }
        });
    }

    [Fact]
    public async Task GetBankTransactionAsync_ReturnsNestedExplanation()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/bank_transactions/15", request.RequestUri!.AbsolutePath);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_transaction": {
                    "url": "https://api.freeagent.com/v2/bank_transactions/15",
                    "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
                    "dated_on": "2020-07-06",
                    "bank_transaction_explanations": [
                      {
                        "url": "https://api.freeagent.com/v2/bank_transaction_explanations/8",
                        "gross_value": "-730.0",
                        "is_deletable": true
                      }
                    ]
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var transaction = await service.GetBankTransactionAsync(15);

        Assert.Equal(15, transaction.ResourceId);
        Assert.Equal(1, transaction.BankAccountId);
        Assert.Equal(-730.0m, transaction.BankTransactionExplanations?[0].GrossValue);
    }

    [Fact]
    public async Task GetBankTransactionAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "bank_transaction": null }""")
        });

        var service = CreateService(handler);

        var exception = await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetBankTransactionAsync(15));
        Assert.Contains("Bank transaction data missing", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeleteBankTransactionAsync_DeletesPluralResourcePath()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.Equal("/v2/bank_transactions/15", request.RequestUri!.AbsolutePath);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var service = CreateService(handler);

        await service.DeleteBankTransactionAsync(15);
    }

    [Fact]
    public async Task UploadBankStatementAsync_PostsStatementArrayAndOmitsUnsetFields()
    {
        string? body = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/v2/bank_transactions/statement", request.RequestUri!.AbsolutePath);
            Assert.Contains("bank_account=https%3A%2F%2Fapi.freeagent.com%2Fv2%2Fbank_accounts%2F1", request.RequestUri.Query, StringComparison.Ordinal);
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var service = CreateService(handler);
        await service.UploadBankStatementAsync(
            new UploadBankStatementRequest(
            [
                new BankStatementTransaction
                {
                    DatedOn = new DateOnly(2019, 7, 1),
                    Amount = -100m,
                    Description = "Local Council",
                    FitId = "049b807d-83ea-4d98-854c-e84b18775d31"
                },
                new BankStatementTransaction
                {
                    DatedOn = new DateOnly(2019, 7, 5),
                    Amount = 3560m,
                    Description = "Sales",
                    TransactionType = BankStatementTransactionType.Credit
                }
            ]),
            bankAccountId: 1);

        Assert.NotNull(body);
        using var document = JsonDocument.Parse(body);
        var statement = document.RootElement.GetProperty("statement");
        Assert.Equal(JsonValueKind.Array, statement.ValueKind);
        Assert.Equal(2, statement.GetArrayLength());
        Assert.False(statement[0].TryGetProperty("transaction_type", out _));
        Assert.Equal("CREDIT", statement[1].GetProperty("transaction_type").GetString());
        Assert.Equal("049b807d-83ea-4d98-854c-e84b18775d31", statement[0].GetProperty("fitid").GetString());
        Assert.False(document.RootElement.TryGetProperty("bank_account", out _));
    }

    [Fact]
    public void UploadBankStatementAsync_EmptyStatement_Throws()
    {
        Assert.Throws<ArgumentException>(() => new UploadBankStatementRequest([]));
    }

    [Fact]
    public async Task UploadBankStatementFileAsync_PostsMultipartStatement()
    {
        string? body = null;
        string? contentType = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal("/v2/bank_transactions/statement", request.RequestUri!.AbsolutePath);
            Assert.Contains("bank_account=", request.RequestUri.Query, StringComparison.Ordinal);
            contentType = request.Content!.Headers.ContentType?.MediaType;
            body = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var service = CreateService(handler);
        await using var statement = new MemoryStream("OFX"u8.ToArray());
        await service.UploadBankStatementFileAsync(
            new UploadBankStatementFileRequest(statement, "statement.ofx"),
            bankAccountId: 9);

        Assert.Equal("multipart/form-data", contentType);
        Assert.NotNull(body);
        Assert.Contains("name=statement", body.Replace("\"", string.Empty, StringComparison.Ordinal), StringComparison.Ordinal);
        Assert.Contains("filename=statement.ofx", body.Replace("\"", string.Empty, StringComparison.Ordinal), StringComparison.Ordinal);
        Assert.Contains("OFX", body, StringComparison.Ordinal);
        Assert.DoesNotContain("dated_on", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task UploadBankStatementFileAsync_EmptyFile_Throws()
    {
        var service = CreateService(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));
        await using var statement = new MemoryStream();

        await Assert.ThrowsAsync<ArgumentException>(() => service.UploadBankStatementFileAsync(
            new UploadBankStatementFileRequest(statement, "empty.ofx"),
            bankAccountId: 1));
    }

    private static BankTransactionsService CreateService(
        HttpMessageHandler handler,
        FreeAgentEnvironment environment = FreeAgentEnvironment.Production)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(environment == FreeAgentEnvironment.Sandbox
                ? "https://api.sandbox.freeagent.com/v2/"
                : "https://api.freeagent.com/v2/")
        };

        var client = new FreeAgentHttpClient(
            httpClient,
            "test-token",
            new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });

        return new BankTransactionsService(client);
    }
}
