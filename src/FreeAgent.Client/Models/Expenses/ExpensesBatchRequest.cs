using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Expenses;

/// <summary>
/// Request envelope for batch expense create.
/// </summary>
internal sealed class ExpensesBatchRequest
{
    [JsonPropertyName("expenses")]
    public List<ExpenseWritePayload>? Expenses { get; set; }
}
