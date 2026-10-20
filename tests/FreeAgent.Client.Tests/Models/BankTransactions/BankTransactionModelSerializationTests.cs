using System.Globalization;
using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankTransactionExplanations;
using FreeAgent.Client.Models.BankTransactions;

namespace FreeAgent.Client.Tests.Models.BankTransactions;

public class BankTransactionModelSerializationTests
{
    [Fact]
    public void Deserialise_ListEnvelope_MapsDatesDecimalsAndBankAccountId()
    {
        const string json = """
            {
              "bank_transactions": [
                {
                  "url": "https://api.freeagent.com/v2/bank_transactions/8",
                  "amount": "-730.0",
                  "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
                  "dated_on": "2020-07-06",
                  "description": "Local Council",
                  "full_description": "Local Council/OTHER/£730.0",
                  "uploaded_at": "2020-07-06T15:26:21.000Z",
                  "unexplained_amount": "0.0",
                  "is_manual": false,
                  "transaction_id": "049b807d-83ea-4d98-854c-e84b18775d31",
                  "created_at": "2020-07-06T15:26:21.000Z",
                  "updated_at": "2020-07-06T15:26:21.000Z",
                  "matching_transactions_count": 0,
                  "bank_transaction_explanations": []
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<BankTransactionListResponse>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(response?.BankTransactions);
        var transaction = response.BankTransactions[0];
        Assert.Equal(8, transaction.ResourceId);
        Assert.Equal(-730.0m, transaction.Amount);
        Assert.Equal(1, transaction.BankAccountId);
        Assert.Equal(new DateOnly(2020, 7, 6), transaction.DatedOn);
        Assert.Equal(0m, transaction.UnexplainedAmount);
        Assert.False(transaction.IsManual);
        Assert.Equal(
            DateTimeOffset.Parse("2020-07-06T15:26:21.000Z", CultureInfo.InvariantCulture),
            transaction.UploadedAt);
        Assert.Equal(
            DateTimeOffset.Parse("2020-07-06T15:26:21.000Z", CultureInfo.InvariantCulture),
            transaction.CreatedAt);
        Assert.NotNull(transaction.BankTransactionExplanations);
        Assert.Empty(transaction.BankTransactionExplanations);
    }

    [Fact]
    public void Deserialise_SingleEnvelope_MapsNestedExplanation()
    {
        const string json = """
            {
              "bank_transaction": {
                "url": "https://api.freeagent.com/v2/bank_transactions/15",
                "amount": "-730.0",
                "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
                "dated_on": "2020-07-06",
                "bank_transaction_explanations": [
                  {
                    "url": "https://api.freeagent.com/v2/bank_transaction_explanations/8",
                    "bank_transaction": "https://api.freeagent.com/v2/bank_transactions/8",
                    "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
                    "dated_on": "2020-07-06",
                    "description": "harness end-to-end e-business",
                    "entry_type": "Business Entertaining",
                    "gross_value": "-730.0",
                    "is_deletable": true
                  }
                ]
              }
            }
            """;

        var response = JsonSerializer.Deserialize<BankTransactionResponse>(json, FreeAgentJsonSerializer.Options);

        var explanation = response?.BankTransaction?.BankTransactionExplanations?[0];
        Assert.NotNull(explanation);
        Assert.Equal(8, explanation.ResourceId);
        Assert.Equal(8, explanation.BankTransactionId);
        Assert.Equal(1, explanation.BankAccountId);
        Assert.Equal(new DateOnly(2020, 7, 6), explanation.DatedOn);
        Assert.Equal("Business Entertaining", explanation.EntryType);
        Assert.Equal(-730.0m, explanation.GrossValue);
        Assert.True(explanation.IsDeletable);
    }

    [Fact]
    public void Serialise_StatementLine_UsesFitIdAndOmitsUnsetType()
    {
        var line = new BankStatementTransaction
        {
            DatedOn = new DateOnly(2019, 7, 1),
            Amount = -100m,
            Description = "Local Council",
            FitId = "049b807d-83ea-4d98-854c-e84b18775d31"
        };

        var json = JsonSerializer.Serialize(line, FreeAgentJsonSerializer.Options);

        using var document = JsonDocument.Parse(json);
        Assert.Equal("2019-07-01", document.RootElement.GetProperty("dated_on").GetString());
        Assert.Equal(-100m, document.RootElement.GetProperty("amount").GetDecimal());
        Assert.Equal("049b807d-83ea-4d98-854c-e84b18775d31", document.RootElement.GetProperty("fitid").GetString());
        Assert.False(document.RootElement.TryGetProperty("transaction_type", out _));
    }

    [Fact]
    public void Serialise_StatementTransactionType_UsesExactWireValue()
    {
        var line = new BankStatementTransaction
        {
            DatedOn = new DateOnly(2019, 7, 5),
            TransactionType = BankStatementTransactionType.DirectDebit
        };

        var json = JsonSerializer.Serialize(line, FreeAgentJsonSerializer.Options);

        using var document = JsonDocument.Parse(json);
        Assert.Equal("DIRECTDEBIT", document.RootElement.GetProperty("transaction_type").GetString());
    }

    [Fact]
    public void Deserialise_StatementTransactionType_ReadsExactWireValue()
    {
        const string json = """{ "dated_on": "2019-07-05", "transaction_type": "SRVCHG" }""";

        var line = JsonSerializer.Deserialize<BankStatementTransaction>(json, FreeAgentJsonSerializer.Options);

        Assert.Equal(BankStatementTransactionType.ServiceCharge, line?.TransactionType);
    }
}
