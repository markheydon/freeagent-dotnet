using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.BankTransactionExplanations;
using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Services.BankTransactionExplanations;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.BankTransactionExplanations;

public class BankTransactionExplanationsServiceTests
{
    [Fact]
    public async System.Threading.Tasks.Task ListAsync_IncludesBankAccountAndDateFilters()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("bank_account=", request.RequestUri!.Query, StringComparison.Ordinal);
            Assert.Contains("from_date=2012-01-01", request.RequestUri.Query, StringComparison.Ordinal);
            Assert.Contains("to_date=2012-03-31", request.RequestUri.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "bank_transaction_explanations": [] }""")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BankTransactionExplanationsService(client);

        await service.ListAsync(
            bankAccountId: 1,
            fromDate: new DateOnly(2012, 1, 1),
            toDate: new DateOnly(2012, 3, 31));
    }

    [Fact]
    public async System.Threading.Tasks.Task ListAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "bank_transaction_explanations": null }""")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BankTransactionExplanationsService(client);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync(bankAccountId: 1));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetBankTransactionExplanationAsync_ReturnsExplanation()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/bank_transaction_explanations/20", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_transaction_explanation": {
                    "url": "https://api.freeagent.com/v2/bank_transaction_explanations/20",
                    "gross_value": "-90.0",
                    "bank_account": "https://api.freeagent.com/v2/bank_accounts/1"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BankTransactionExplanationsService(client);

        var explanation = await service.GetBankTransactionExplanationAsync(20);

        Assert.Equal(20, explanation.ResourceId);
        Assert.Equal(-90m, explanation.GrossValue);
        Assert.Equal(1, explanation.BankAccountId);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateCategoryBankTransactionExplanationAsync_SerialisesExpectedPayload()
    {
        string? body = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "bank_transaction_explanation": {
                    "url": "https://api.freeagent.com/v2/bank_transaction_explanations/12",
                    "gross_value": "-10.0"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BankTransactionExplanationsService(client);

        await service.CreateCategoryBankTransactionExplanationAsync(
            CreateCategoryBankTransactionExplanationRequest.ForExistingTransaction(
                bankTransactionId: 8,
                categoryNominalCode: "285",
                datedOn: new DateOnly(2019, 5, 1),
                grossValue: -10m,
                description: "test"));

        Assert.NotNull(body);
        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/bank_transactions/8", explanation.GetProperty("bank_transaction").GetString());
        Assert.Equal("https://api.freeagent.com/v2/categories/285", explanation.GetProperty("category").GetString());
        Assert.False(explanation.TryGetProperty("paid_invoice", out _));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateInvoiceReceiptBankTransactionExplanationAsync_SerialisesPaidInvoiceOnly()
    {
        string? body = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "bank_transaction_explanation": {
                    "url": "https://api.freeagent.com/v2/bank_transaction_explanations/12"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BankTransactionExplanationsService(client);

        await service.CreateInvoiceReceiptBankTransactionExplanationAsync(
            CreateInvoiceReceiptBankTransactionExplanationRequest.Create(
                bankTransactionId: 8,
                paidInvoiceId: 3,
                datedOn: new DateOnly(2019, 5, 1),
                grossValue: 100m));

        Assert.NotNull(body);
        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/invoices/3", explanation.GetProperty("paid_invoice").GetString());
        Assert.False(explanation.TryGetProperty("category", out _));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTransferBankTransactionExplanationAsync_SerialisesTransferAccount()
    {
        string? body = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "bank_transaction_explanation": {
                    "url": "https://api.freeagent.com/v2/bank_transaction_explanations/12"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BankTransactionExplanationsService(client);

        await service.CreateTransferBankTransactionExplanationAsync(
            CreateTransferBankTransactionExplanationRequest.Create(
                bankTransactionId: 8,
                transferBankAccountId: 2,
                datedOn: new DateOnly(2019, 5, 1),
                grossValue: -170m));

        Assert.NotNull(body);
        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/bank_accounts/2", explanation.GetProperty("transfer_bank_account").GetString());
    }

    [Fact]
    public async System.Threading.Tasks.Task CreatePaymentOrRefundBankTransactionExplanationAsync_SerialisesRebillFields()
    {
        string? body = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "bank_transaction_explanation": {
                    "url": "https://api.freeagent.com/v2/bank_transaction_explanations/12"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BankTransactionExplanationsService(client);

        await service.CreatePaymentOrRefundBankTransactionExplanationAsync(
            CreatePaymentOrRefundBankTransactionExplanationRequest.Create(
                bankTransactionId: 8,
                projectId: 1,
                rebillType: BillRebillType.Markup,
                datedOn: new DateOnly(2019, 5, 1),
                grossValue: -90m,
                rebillFactor: 0.25m,
                categoryNominalCode: "366"));

        Assert.NotNull(body);
        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/projects/1", explanation.GetProperty("project").GetString());
        Assert.Equal("markup", explanation.GetProperty("rebill_type").GetString());
        Assert.Equal(0.25m, explanation.GetProperty("rebill_factor").GetDecimal());
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteBankTransactionExplanationAsync_CallsDeleteEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/bank_transaction_explanations/99", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BankTransactionExplanationsService(client);

        await service.DeleteBankTransactionExplanationAsync(99);
    }
}
