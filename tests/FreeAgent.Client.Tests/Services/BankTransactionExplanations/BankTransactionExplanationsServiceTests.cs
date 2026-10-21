using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.BankTransactionExplanations;
using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Models.DepreciationProfiles;
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
    public async System.Threading.Tasks.Task GetBankTransactionExplanationAsync_WithHydration_FetchesPaidInvoice()
    {
        var handler = new QueueHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_transaction_explanation": {
                    "url": "https://api.freeagent.com/v2/bank_transaction_explanations/20",
                    "paid_invoice": "https://api.freeagent.com/v2/invoices/55"
                  }
                }
                """)
            },
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "invoice": {
                    "url": "https://api.freeagent.com/v2/invoices/55",
                    "reference": "PAID"
                  }
                }
                """)
            });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BankTransactionExplanationsService(client);

        var explanation = await service.GetBankTransactionExplanationAsync(
            20,
            new BankTransactionExplanationGetOptions { IncludePaidInvoice = true });

        Assert.Equal("PAID", explanation.PaidInvoice?.Reference);
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
    public async System.Threading.Tasks.Task CreateInvoiceReceiptBankTransactionExplanationAsync_SerialisesCreditNoteRefundUri()
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
            CreateInvoiceReceiptBankTransactionExplanationRequest.CreateCreditNoteRefund(
                bankTransactionId: 8,
                paidCreditNoteId: 5,
                datedOn: new DateOnly(2019, 5, 1),
                grossValue: 100m));

        Assert.NotNull(body);
        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/credit_notes/5", explanation.GetProperty("paid_invoice").GetString());
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
    public async System.Threading.Tasks.Task CreateBillPaymentBankTransactionExplanationAsync_SerialisesPaidBillOnly()
    {
        var body = await CaptureCreatePayloadAsync(service => service.CreateBillPaymentBankTransactionExplanationAsync(
            CreateBillPaymentBankTransactionExplanationRequest.Create(8, 5, new DateOnly(2019, 5, 1), 50m)));

        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/bills/5", explanation.GetProperty("paid_bill").GetString());
        Assert.False(explanation.TryGetProperty("category", out _));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateSmartUserPaymentBankTransactionExplanationAsync_SerialisesPaidUserOnly()
    {
        var body = await CaptureCreatePayloadAsync(service => service.CreateSmartUserPaymentBankTransactionExplanationAsync(
            CreateSmartUserPaymentBankTransactionExplanationRequest.Create(8, 2, new DateOnly(2019, 5, 1), -20m)));

        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/users/2", explanation.GetProperty("paid_user").GetString());
        Assert.False(explanation.TryGetProperty("category", out _));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateCategorisedUserPaymentBankTransactionExplanationAsync_SerialisesPaidUserAndCategory()
    {
        var body = await CaptureCreatePayloadAsync(service => service.CreateCategorisedUserPaymentBankTransactionExplanationAsync(
            CreateCategorisedUserPaymentBankTransactionExplanationRequest.Create(8, 2, "285", new DateOnly(2019, 5, 1), -20m)));

        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/users/2", explanation.GetProperty("paid_user").GetString());
        Assert.Equal("https://api.freeagent.com/v2/categories/285", explanation.GetProperty("category").GetString());
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateStockBankTransactionExplanationAsync_SerialisesStockFields()
    {
        var body = await CaptureCreatePayloadAsync(service => service.CreateStockBankTransactionExplanationAsync(
            CreateStockBankTransactionExplanationRequest.Create(8, 4, stockAlteringQuantity: 3, new DateOnly(2019, 5, 1), -15m, categoryNominalCode: "366")));

        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/stock_items/4", explanation.GetProperty("stock_item").GetString());
        Assert.Equal(3, explanation.GetProperty("stock_altering_quantity").GetInt32());
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateCapitalAssetPurchaseBankTransactionExplanationAsync_SerialisesCategory()
    {
        var body = await CaptureCreatePayloadAsync(service => service.CreateCapitalAssetPurchaseBankTransactionExplanationAsync(
            CreateCapitalAssetPurchaseBankTransactionExplanationRequest.Create(8, "720", new DateOnly(2019, 5, 1), -500m, assetLifeYears: "5")));

        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/categories/720", explanation.GetProperty("category").GetString());
        Assert.Equal("5", explanation.GetProperty("asset_life_years").GetString());
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateCapitalAssetPurchaseBankTransactionExplanationAsync_SerialisesDepreciationProfile()
    {
        var body = await CaptureCreatePayloadAsync(service => service.CreateCapitalAssetPurchaseBankTransactionExplanationAsync(
            CreateCapitalAssetPurchaseBankTransactionExplanationRequest.CreateWithDepreciationProfile(
                8,
                "720",
                new DateOnly(2019, 5, 1),
                -500m,
                CapitalAssetPurchaseWrite.StraightLine(10, DepreciationFrequency.Annually))));

        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        var capitalAsset = explanation.GetProperty("capital_asset");
        var profile = capitalAsset.GetProperty("depreciation_profile");
        Assert.Equal("straight_line", profile.GetProperty("method").GetString());
        Assert.Equal(10, profile.GetProperty("asset_life_years").GetInt32());
        Assert.Equal("annually", profile.GetProperty("frequency").GetString());
        Assert.False(explanation.TryGetProperty("asset_life_years", out _));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateCapitalAssetDisposalBankTransactionExplanationAsync_SerialisesDisposedAsset()
    {
        var body = await CaptureCreatePayloadAsync(service => service.CreateCapitalAssetDisposalBankTransactionExplanationAsync(
            CreateCapitalAssetDisposalBankTransactionExplanationRequest.Create(8, 9, new DateOnly(2019, 5, 1), 200m)));

        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/capital_assets/9", explanation.GetProperty("disposed_asset").GetString());
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateLandlordPropertyBankTransactionExplanationAsync_SerialisesPropertyUri()
    {
        const string propertyUri = "https://api.freeagent.com/v2/properties/1";
        var body = await CaptureCreatePayloadAsync(service => service.CreateLandlordPropertyBankTransactionExplanationAsync(
            CreateLandlordPropertyBankTransactionExplanationRequest.Create(8, "285", propertyUri, new DateOnly(2019, 5, 1), -30m)));

        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal(propertyUri, explanation.GetProperty("property").GetString());
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateLandlordPropertyBankTransactionExplanationAsync_SerialisesPropertyFromId()
    {
        var body = await CaptureCreatePayloadAsync(service => service.CreateLandlordPropertyBankTransactionExplanationAsync(
            CreateLandlordPropertyBankTransactionExplanationRequest.Create(
                8,
                "285",
                1,
                FreeAgentEnvironment.Production,
                new DateOnly(2019, 5, 1),
                -30m)));

        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/properties/1", explanation.GetProperty("property").GetString());
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateOpeningBalanceBankTransactionExplanationAsync_SerialisesDirectContactAndCategory()
    {
        var body = await CaptureCreatePayloadAsync(service => service.CreateOpeningBalanceBankTransactionExplanationAsync(
            CreateOpeningBalanceBankTransactionExplanationRequest.Create(8, 7, "681", new DateOnly(2019, 5, 1), 100m)));

        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("https://api.freeagent.com/v2/contacts/7", explanation.GetProperty("direct_contact").GetString());
        Assert.Equal("https://api.freeagent.com/v2/categories/681", explanation.GetProperty("category").GetString());
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateBankTransactionExplanationAsync_SerialisesUpdatePayload()
    {
        string? body = null;
        var handler = new QueueHttpMessageHandler(request =>
        {
            body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_transaction_explanation": {
                    "url": "https://api.freeagent.com/v2/bank_transaction_explanations/12",
                    "description": "updated"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new BankTransactionExplanationsService(client);

        await service.UpdateBankTransactionExplanationAsync(
            12,
            new UpdateBankTransactionExplanationRequest { Description = "updated", CategoryNominalCode = "285" });

        Assert.NotNull(body);
        using var document = JsonDocument.Parse(body);
        var explanation = document.RootElement.GetProperty("bank_transaction_explanation");
        Assert.Equal("updated", explanation.GetProperty("description").GetString());
        Assert.Equal("https://api.freeagent.com/v2/categories/285", explanation.GetProperty("category").GetString());
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

    private static async Task<string> CaptureCreatePayloadAsync(
        Func<BankTransactionExplanationsService, Task<BankTransactionExplanation>> act)
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

        await act(service);

        return body ?? throw new InvalidOperationException("Create request body was not captured.");
    }
}
