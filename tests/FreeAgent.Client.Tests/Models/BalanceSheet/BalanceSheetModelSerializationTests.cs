using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using BalanceSheetModel = global::FreeAgent.Client.BalanceSheet;

namespace FreeAgent.Client.Tests.Models.BalanceSheet;

public class BalanceSheetModelSerializationTests
{
    [Fact]
    public void Deserialize_MapsDocumentedBalanceSheet()
    {
        const string json = """
            {
              "accounting_period_start_date": "2023-03-01",
              "as_at_date": "2023-09-30",
              "currency": "GBP",
              "capital_assets": {
                "accounts": [
                  {
                    "name": "Other Capital Asset Brought Forward",
                    "nominal_code": "601-4",
                    "total_debit_value": 800
                  }
                ],
                "net_book_value": 1099
              },
              "current_assets": {
                "accounts": [
                  {
                    "name": "Trade Debtors",
                    "nominal_code": "681",
                    "total_debit_value": 2040
                  }
                ]
              },
              "current_liabilities": {
                "accounts": [
                  {
                    "name": "Trade Creditors",
                    "nominal_code": "796",
                    "total_debit_value": -865
                  }
                ]
              },
              "net_current_assets": 10806,
              "total_assets": 11905,
              "owners_equity": {
                "accounts": [
                  {
                    "name": "Share Premium",
                    "nominal_code": "670",
                    "total_debit_value": -30
                  }
                ],
                "retained_profit": -11875
              },
              "total_owners_equity": -11905
            }
            """;

        var sheet = JsonSerializer.Deserialize<BalanceSheetModel>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(sheet);
        Assert.Equal(new DateOnly(2023, 3, 1), sheet.AccountingPeriodStartDate);
        Assert.Equal(new DateOnly(2023, 9, 30), sheet.AsAtDate);
        Assert.Equal(CurrencyCode.GBP, sheet.Currency);
        Assert.Equal(1099, sheet.CapitalAssets?.NetBookValue);
        Assert.Equal("681", sheet.CurrentAssets?.Accounts?[0].NominalCode);
        Assert.Equal(-11905, sheet.TotalOwnersEquity);
    }
}
