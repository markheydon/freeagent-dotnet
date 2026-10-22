using System.Net;
using System.Net.Http;
using System.Text.Json;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.BankAccounts;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.BankAccounts;

public class BankAccountsServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsBankAccounts()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/bank_accounts", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_accounts": [
                    {
                      "url": "https://api.freeagent.com/v2/bank_accounts/1",
                      "opening_balance": "0.0",
                      "type": "StandardBankAccount",
                      "name": "Default bank account",
                      "is_personal": false,
                      "status": "active",
                      "currency": "GBP",
                      "current_balance": "0.0",
                      "latest_activity_date": "2011-07-28",
                      "updated_at": "2011-07-28T11:25:20Z",
                      "created_at": "2011-07-28T11:25:11Z"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var bankAccounts = await service.ListAsync();

        Assert.Single(bankAccounts);
        Assert.Equal("Default bank account", bankAccounts[0].Name);
        Assert.Equal(BankAccountType.StandardBankAccount, bankAccounts[0].Type);
        Assert.Equal(BankAccountStatus.Active, bankAccounts[0].Status);
        Assert.Equal(CurrencyCode.GBP, bankAccounts[0].Currency);
        Assert.Equal(new DateOnly(2011, 7, 28), bankAccounts[0].LatestActivityDate);
        Assert.Equal(1, bankAccounts[0].ResourceId);
    }

    [Fact]
    public async Task ListAsync_WithView_IncludesQueryParameter()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("view=standard_bank_accounts", request.RequestUri!.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{ "bank_accounts": [] }""")
            };
        });

        var service = CreateService(handler);
        await service.ListAsync(view: BankAccountView.StandardBankAccounts);
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
    public async Task GetBankAccountAsync_ReturnsBankAccount()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/bank_accounts/42", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_account": {
                    "url": "https://api.freeagent.com/v2/bank_accounts/42",
                    "opening_balance": "10.0",
                    "type": "CreditCardAccount",
                    "name": "Company card",
                    "bank_guess_enabled": false,
                    "updated_at": "2011-07-28T11:25:20Z",
                    "created_at": "2011-07-28T11:25:11Z"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var bankAccount = await service.GetBankAccountAsync(42);

        Assert.Equal("Company card", bankAccount.Name);
        Assert.Equal(BankAccountType.CreditCardAccount, bankAccount.Type);
        Assert.False(bankAccount.BankGuessEnabled);
        Assert.Equal(42, bankAccount.ResourceId);
    }

    [Fact]
    public async Task GetBankAccountAsync_MissingPayload_Throws()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.GetBankAccountAsync(1));
    }

    [Fact]
    public async Task CreateStandardBankAccountAsync_SerialisesExpectedPayload()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/bank_accounts", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            var json = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            using var document = JsonDocument.Parse(json);
            var bankAccount = document.RootElement.GetProperty("bank_account");

            Assert.Equal("StandardBankAccount", bankAccount.GetProperty("type").GetString());
            Assert.Equal("SDK test account", bankAccount.GetProperty("name").GetString());
            Assert.Equal(0m, bankAccount.GetProperty("opening_balance").GetDecimal());
            Assert.False(bankAccount.TryGetProperty("account_number", out _));

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "bank_account": {
                    "url": "https://api.freeagent.com/v2/bank_accounts/61",
                    "type": "StandardBankAccount",
                    "name": "SDK test account",
                    "opening_balance": "0.0"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var created = await service.CreateStandardBankAccountAsync(
            CreateStandardBankAccountRequest.Create("SDK test account", openingBalance: 0m));

        Assert.Equal(61, created.ResourceId);
    }

    [Fact]
    public async Task CreateCreditCardBankAccountAsync_SerialisesAccountNumber()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            var json = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            using var document = JsonDocument.Parse(json);
            var bankAccount = document.RootElement.GetProperty("bank_account");

            Assert.Equal("CreditCardAccount", bankAccount.GetProperty("type").GetString());
            Assert.Equal("1234", bankAccount.GetProperty("account_number").GetString());
            Assert.False(bankAccount.TryGetProperty("sort_code", out _));

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "bank_account": {
                    "url": "https://api.freeagent.com/v2/bank_accounts/62",
                    "type": "CreditCardAccount",
                    "name": "Card",
                    "account_number": "1234"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        await service.CreateCreditCardBankAccountAsync(
            CreateCreditCardBankAccountRequest.Create("Card", openingBalance: 0m, accountNumber: "1234"));
    }

    [Fact]
    public async Task CreateEcommerceBankAccountAsync_SerialisesType()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            var json = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            using var document = JsonDocument.Parse(json);
            var bankAccount = document.RootElement.GetProperty("bank_account");

            Assert.Equal("EcommerceAccount", bankAccount.GetProperty("type").GetString());

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "bank_account": {
                    "url": "https://api.freeagent.com/v2/bank_accounts/63",
                    "type": "EcommerceAccount",
                    "name": "Shop"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        await service.CreateEcommerceBankAccountAsync(
            CreateEcommerceBankAccountRequest.Create("Shop", openingBalance: 0m));
    }

    [Fact]
    public async Task CreateEcommerceBankAccountAsync_SerialisesEmail()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            var json = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            using var document = JsonDocument.Parse(json);
            var bankAccount = document.RootElement.GetProperty("bank_account");

            Assert.Equal("seller@example.com", bankAccount.GetProperty("email").GetString());

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent("""
                {
                  "bank_account": {
                    "url": "https://api.freeagent.com/v2/bank_accounts/64",
                    "type": "EcommerceAccount",
                    "name": "PayPal",
                    "email": "seller@example.com"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        await service.CreateEcommerceBankAccountAsync(
            CreateEcommerceBankAccountRequest.Create(
                "PayPal",
                openingBalance: 0m,
                email: "seller@example.com"));
    }

    [Fact]
    public async Task UpdateBankAccountAsync_PutsEnvelope()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/bank_accounts/9", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);

            var json = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            using var document = JsonDocument.Parse(json);
            var bankAccount = document.RootElement.GetProperty("bank_account");

            Assert.Equal("Renamed account", bankAccount.GetProperty("name").GetString());
            Assert.Equal("hidden", bankAccount.GetProperty("status").GetString());

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "bank_account": {
                    "url": "https://api.freeagent.com/v2/bank_accounts/9",
                    "name": "Renamed account",
                    "status": "hidden"
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var updated = await service.UpdateBankAccountAsync(
            9,
            UpdateBankAccountRequest.Create(name: "Renamed account", status: BankAccountStatus.Hidden));

        Assert.Equal("Renamed account", updated.Name);
    }

    [Fact]
    public async Task DeleteBankAccountAsync_DeletesResource()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/bank_accounts/5", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var service = CreateService(handler);
        await service.DeleteBankAccountAsync(5);
    }

    private static BankAccountsService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new BankAccountsService(client);
    }
}
