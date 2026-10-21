using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Services.BankAccounts;

namespace FreeAgent.Client.Tests.Models.BankAccounts;

public class BankAccountModelSerializationTests
{
    [Fact]
    public void Deserialise_ListEnvelope_MapsEnumsAndDates()
    {
        const string json = """
            {
              "bank_accounts": [
                {
                  "url": "https://api.freeagent.com/v2/bank_accounts/2",
                  "type": "Paypal::CurrencyAccount",
                  "name": "PayPal GBP",
                  "status": "active",
                  "currency": "GBP",
                  "latest_activity_date": "2024-01-15",
                  "created_at": "2024-01-01T10:00:00Z",
                  "updated_at": "2024-01-02T11:00:00Z"
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<BankAccountListResponse>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(response?.BankAccounts);
        var account = response.BankAccounts![0];
        Assert.Equal(BankAccountType.PaypalCurrencyAccount, account.Type);
        Assert.Equal(BankAccountStatus.Active, account.Status);
        Assert.Equal(CurrencyCode.GBP, account.Currency);
        Assert.Equal(new DateOnly(2024, 1, 15), account.LatestActivityDate);
        Assert.Equal(
            DateTimeOffset.Parse("2024-01-01T10:00:00Z", CultureInfo.InvariantCulture),
            account.CreatedAt);
    }

    [Fact]
    public void Serialise_CreateStandardPayload_IncludesTypeAndSortCode()
    {
        var payload = BankAccountWritePayloadMapper.FromCreate(
            CreateStandardBankAccountRequest.Create("Probe", 0m, sortCode: "12-34-56"));

        var json = JsonSerializer.Serialize(
            new BankAccountRequestEnvelope { BankAccount = payload },
            FreeAgentJsonSerializer.Options);

        using var document = JsonDocument.Parse(json);
        var bankAccount = document.RootElement.GetProperty("bank_account");
        Assert.Equal("StandardBankAccount", bankAccount.GetProperty("type").GetString());
        Assert.Equal("12-34-56", bankAccount.GetProperty("sort_code").GetString());
    }

    private sealed class BankAccountRequestEnvelope
    {
        [JsonPropertyName("bank_account")]
        public BankAccountWritePayload? BankAccount { get; set; }
    }
}
