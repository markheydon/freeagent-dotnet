using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.BankTransactions;

/// <summary>
/// Wire envelope for a JSON statement upload. The API expects <c>{ "statement": [ ... ] }</c>.
/// </summary>
internal sealed class BankStatementUploadPayload
{
    /// <summary>
    /// Statement transaction lines.
    /// </summary>
    [JsonPropertyName("statement")]
    public IReadOnlyList<BankStatementTransaction> Statement { get; set; } = [];
}
