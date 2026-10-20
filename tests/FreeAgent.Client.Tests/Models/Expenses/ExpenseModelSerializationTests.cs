using System.Text.Json;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Models.Expenses;

namespace FreeAgent.Client.Tests.Models.Expenses;

public class ExpenseModelSerializationTests
{
    [Fact]
    public void DeserializeExpense_MapsCategoryUserDatesAndEnums()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/expenses/3",
              "user": "https://api.freeagent.com/v2/users/1",
              "category": "https://api.freeagent.com/v2/categories/285",
              "dated_on": "2011-08-24",
              "gross_value": "-20.0",
              "sales_tax_status": "TAXABLE",
              "ec_status": "UK/Non-EC",
              "reclaim_mileage": 1,
              "created_at": "2011-08-24T08:10:40Z",
              "updated_at": "2011-08-24T08:10:40Z"
            }
            """;

        var expense = JsonSerializer.Deserialize<Expense>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(expense);
        Assert.Equal(3, expense!.ResourceId);
        Assert.Equal(1, expense.UserId);
        Assert.Equal("285", expense.CategoryNominalCode);
        Assert.False(expense.IsMileageCategory);
        Assert.Equal(new DateOnly(2011, 8, 24), expense.DatedOn);
        Assert.Equal(-20.0m, expense.GrossValue);
        Assert.Equal(BillSalesTaxStatus.Taxable, expense.SalesTaxStatus);
        Assert.Equal(BillEcStatus.UkNonEc, expense.EcStatus);
        Assert.Equal(ExpenseReclaimMileage.AtAmapRate, expense.ReclaimMileage);
    }

    [Fact]
    public void DeserializeExpense_MapsMileageCategoryLiteral()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/expenses/4",
              "user": "https://api.freeagent.com/v2/users/1",
              "category": "Mileage",
              "dated_on": "2024-03-01",
              "mileage": "12.5",
              "vehicle_type": "Car",
              "description": "Client visit"
            }
            """;

        var expense = JsonSerializer.Deserialize<Expense>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(expense);
        Assert.True(expense!.IsMileageCategory);
        Assert.Null(expense.CategoryNominalCode);
        Assert.Equal(12.5m, expense.Mileage);
        Assert.Equal(ExpenseVehicleType.Car, expense.VehicleType);
    }

    [Fact]
    public void SerializeExpenseWritePayload_MapsMileageCategory()
    {
        var payload = ExpenseWritePayload.FromExpense(
            new Expense
            {
                UserId = 1,
                IsMileageCategory = true,
                DatedOn = new DateOnly(2024, 3, 1),
                Mileage = 10m,
                VehicleType = ExpenseVehicleType.Car,
                Description = "Trip"
            },
            FreeAgentEnvironment.Sandbox);

        var json = JsonSerializer.Serialize(payload, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"category\":\"Mileage\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("categories/", json, StringComparison.Ordinal);
    }

    [Fact]
    public void SerializeExpenseWritePayload_MapsMileageAfterCategoryClear()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/expenses/4",
              "user": "https://api.freeagent.com/v2/users/1",
              "category": "Mileage",
              "dated_on": "2024-03-01",
              "mileage": "10",
              "vehicle_type": "Car",
              "description": "Trip"
            }
            """;

        var expense = JsonSerializer.Deserialize<Expense>(json, FreeAgentJsonSerializer.Options);
        Assert.NotNull(expense);
        expense!.CategoryNominalCode = null;

        var payload = ExpenseWritePayload.FromExpense(expense, FreeAgentEnvironment.Sandbox);
        var payloadJson = JsonSerializer.Serialize(payload, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"category\":\"Mileage\"", payloadJson, StringComparison.Ordinal);
    }

    [Fact]
    public void SerializeExpenseWritePayload_OmitsReadOnlyCapitalAssetFields()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/expenses/5",
              "user": "https://api.freeagent.com/v2/users/1",
              "category": "https://api.freeagent.com/v2/categories/601",
              "dated_on": "2024-03-01",
              "gross_value": "-500.0",
              "capital_asset": "https://api.freeagent.com/v2/capital_assets/9",
              "depreciation_schedule": "reducing_balance"
            }
            """;

        var expense = JsonSerializer.Deserialize<Expense>(json, FreeAgentJsonSerializer.Options);
        Assert.NotNull(expense);

        var payload = ExpenseWritePayload.FromExpense(expense!, FreeAgentEnvironment.Sandbox);
        var payloadJson = JsonSerializer.Serialize(payload, FreeAgentJsonSerializer.Options);

        Assert.DoesNotContain("capital_asset", payloadJson, StringComparison.Ordinal);
        Assert.DoesNotContain("depreciation_schedule", payloadJson, StringComparison.Ordinal);
    }

    [Fact]
    public void SerializeExpenseWritePayload_MapsCategoryNominalCode()
    {
        var payload = ExpenseWritePayload.FromExpense(
            new Expense
            {
                UserId = 2,
                CategoryNominalCode = "285",
                DatedOn = new DateOnly(2024, 3, 1),
                GrossValue = -15m,
                Description = "Lunch"
            },
            FreeAgentEnvironment.Sandbox);

        var json = JsonSerializer.Serialize(payload, FreeAgentJsonSerializer.Options);

        Assert.Contains("categories/285", json, StringComparison.Ordinal);
        Assert.Contains("\"user\":", json, StringComparison.Ordinal);
        Assert.Contains("\"gross_value\":-15", json, StringComparison.Ordinal);
    }
}
