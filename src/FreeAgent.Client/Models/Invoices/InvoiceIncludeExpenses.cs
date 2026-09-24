using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Expense grouping options when creating or updating an invoice.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<InvoiceIncludeExpenses>))]
public enum InvoiceIncludeExpenses
{
    /// <summary>One line grouped by a single expense.</summary>
    [JsonStringEnumMemberName("billed_grouped_by_single_expense")]
    BilledGroupedBySingleExpense,

    /// <summary>Separate lines grouped by expense.</summary>
    [JsonStringEnumMemberName("billed_grouped_by_expense")]
    BilledGroupedByExpense
}
