using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Maps public link identifiers on models to typed references for write payloads.
/// </summary>
internal static class LinkedResourceWriteMapper
{
    public static ContactReference? ToContactReference(FreeAgentEnvironment environment, long? contactId) =>
        contactId is long id ? ContactReference.ForEnvironment(environment, id) : null;

    public static ProjectReference? ToProjectReference(FreeAgentEnvironment environment, long? projectId) =>
        projectId is long id ? ProjectReference.ForEnvironment(environment, id) : null;

    public static TaskReference? ToTaskReference(FreeAgentEnvironment environment, long? taskId) =>
        taskId is long id ? TaskReference.ForEnvironment(environment, id) : null;

    public static UserReference? ToUserReference(FreeAgentEnvironment environment, long? userId) =>
        userId is long id ? UserReference.ForEnvironment(environment, id) : null;

    public static BankAccountReference? ToBankAccountReference(FreeAgentEnvironment environment, long? bankAccountId) =>
        bankAccountId is long id ? BankAccountReference.ForEnvironment(environment, id) : null;

    public static StockItemReference? ToStockItemReference(FreeAgentEnvironment environment, long? stockItemId) =>
        stockItemId is long id ? StockItemReference.ForEnvironment(environment, id) : null;

    public static CategoryReference? ToCategoryReference(FreeAgentEnvironment environment, string? nominalCode) =>
        string.IsNullOrWhiteSpace(nominalCode)
            ? null
            : CategoryReference.ForEnvironment(environment, nominalCode);

    public static InvoiceReference? ToInvoiceReference(FreeAgentEnvironment environment, long? invoiceId) =>
        invoiceId is long id ? InvoiceReference.ForEnvironment(environment, id) : null;

    public static CreditNoteReference? ToCreditNoteReference(FreeAgentEnvironment environment, long? creditNoteId) =>
        creditNoteId is long id ? CreditNoteReference.ForEnvironment(environment, id) : null;
}
