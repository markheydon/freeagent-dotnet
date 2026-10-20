using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Expenses;

/// <summary>
/// Request envelope for a single expense create or update.
/// </summary>
internal sealed class ExpenseRequest
{
    [JsonPropertyName("expense")]
    public ExpenseWritePayload? Expense { get; set; }
}
