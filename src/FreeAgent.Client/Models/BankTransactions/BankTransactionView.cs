using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.BankTransactions;

/// <summary>
/// Documented <c>view</c> query values for listing bank transactions.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BankTransactionView>))]
public enum BankTransactionView
{
    /// <summary>
    /// All transactions for the bank account. This is the API default.
    /// </summary>
    [JsonStringEnumMemberName("all")]
    All,

    /// <summary>
    /// Transactions that still have an unexplained amount.
    /// </summary>
    [JsonStringEnumMemberName("unexplained")]
    Unexplained,

    /// <summary>
    /// Transactions that are fully explained.
    /// </summary>
    [JsonStringEnumMemberName("explained")]
    Explained,

    /// <summary>
    /// Transactions added manually.
    /// </summary>
    [JsonStringEnumMemberName("manual")]
    Manual,

    /// <summary>
    /// Transactions imported from a statement or bank feed.
    /// </summary>
    [JsonStringEnumMemberName("imported")]
    Imported,

    /// <summary>
    /// Transactions marked for review.
    /// </summary>
    [JsonStringEnumMemberName("marked_for_review")]
    MarkedForReview
}
