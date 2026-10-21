using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for a single expense.
/// </summary>
internal sealed class ExpenseResponse
{
    [JsonPropertyName("expense")]
    public Expense? Expense { get; set; }
}
