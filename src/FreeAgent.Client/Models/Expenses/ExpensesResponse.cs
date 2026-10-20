using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Expenses;

/// <summary>
/// Response envelope for a list of expenses.
/// </summary>
internal sealed class ExpensesResponse
{
    [JsonPropertyName("expenses")]
    public List<Expense>? Expenses { get; set; }
}
