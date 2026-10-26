using FreeAgent.Client.Infrastructure.Configuration;
namespace FreeAgent.Client;

/// <summary>
/// Builds environment-correct FreeAgent resource URIs.
/// </summary>
/// <remarks>
/// Use <see cref="ForEnvironment(FreeAgentEnvironment)"/> when you do not have a <see cref="FreeAgentClient"/> instance
/// (for example in mappers or unit tests). Individual <see cref="ContactReference.ForEnvironment(FreeAgentEnvironment, long)"/>
/// methods build the same URIs for a single resource type.
/// </remarks>
public sealed class FreeAgentResourceUrls
{
    private readonly FreeAgentEnvironment _environment;

    /// <summary>
    /// Creates URL builders for the given API environment (same URIs as <see cref="FreeAgentClient.Urls"/>).
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <returns>Resource URI builder.</returns>
    public static FreeAgentResourceUrls ForEnvironment(FreeAgentEnvironment environment) => new(environment);

    internal FreeAgentResourceUrls(FreeAgentEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Creates a contact resource reference.
    /// </summary>
    /// <param name="contactId">Contact identifier.</param>
    /// <returns>Contact reference.</returns>
    public ContactReference Contact(long contactId) => ContactReference.ForEnvironment(_environment, contactId);

    /// <summary>
    /// Creates a project resource reference.
    /// </summary>
    /// <param name="projectId">Project identifier.</param>
    /// <returns>Project reference.</returns>
    public ProjectReference Project(long projectId) => ProjectReference.ForEnvironment(_environment, projectId);

    /// <summary>
    /// Creates a task resource reference.
    /// </summary>
    /// <param name="projectTaskId">Task identifier.</param>
    /// <returns>Task reference.</returns>
    public ProjectTaskReference ProjectTask(long projectTaskId) => ProjectTaskReference.ForEnvironment(_environment, projectTaskId);

    /// <summary>
    /// Creates a user resource reference.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <returns>User reference.</returns>
    public UserReference User(long userId) => UserReference.ForEnvironment(_environment, userId);

    /// <summary>
    /// Creates a timeslip resource reference.
    /// </summary>
    /// <param name="timeslipId">Timeslip identifier.</param>
    /// <returns>Timeslip reference.</returns>
    public TimeslipReference Timeslip(long timeslipId) => TimeslipReference.ForEnvironment(_environment, timeslipId);

    /// <summary>
    /// Creates an invoice resource reference.
    /// </summary>
    /// <param name="invoiceId">Invoice identifier.</param>
    /// <returns>Invoice reference.</returns>
    public InvoiceReference Invoice(long invoiceId) => InvoiceReference.ForEnvironment(_environment, invoiceId);

    /// <summary>
    /// Creates an estimate resource reference.
    /// </summary>
    /// <param name="estimateId">Estimate identifier.</param>
    /// <returns>Estimate reference.</returns>
    public EstimateReference Estimate(long estimateId) => EstimateReference.ForEnvironment(_environment, estimateId);

    /// <summary>
    /// Creates a note resource reference.
    /// </summary>
    /// <param name="noteId">Note identifier.</param>
    /// <returns>Note reference.</returns>
    public NoteReference Note(long noteId) => NoteReference.ForEnvironment(_environment, noteId);

    /// <summary>
    /// Creates a bank account resource reference.
    /// </summary>
    /// <param name="bankAccountId">Bank account identifier.</param>
    /// <returns>Bank account reference.</returns>
    public BankAccountReference BankAccount(long bankAccountId) =>
        BankAccountReference.ForEnvironment(_environment, bankAccountId);

    /// <summary>
    /// Creates a category resource reference.
    /// </summary>
    /// <param name="nominalCode">Category nominal code.</param>
    /// <returns>Category reference.</returns>
    public CategoryReference Category(string nominalCode) =>
        CategoryReference.ForEnvironment(_environment, nominalCode);

    /// <summary>
    /// Creates a recurring invoice resource reference.
    /// </summary>
    /// <param name="recurringInvoiceId">Recurring invoice identifier.</param>
    /// <returns>Recurring invoice reference.</returns>
    public RecurringInvoiceReference RecurringInvoice(long recurringInvoiceId) =>
        RecurringInvoiceReference.ForEnvironment(_environment, recurringInvoiceId);

    /// <summary>
    /// Creates an attachment resource reference.
    /// </summary>
    /// <param name="attachmentId">Attachment identifier.</param>
    /// <returns>Attachment reference.</returns>
    public AttachmentReference Attachment(long attachmentId) =>
        AttachmentReference.ForEnvironment(_environment, attachmentId);

    /// <summary>
    /// Creates a bill resource reference.
    /// </summary>
    /// <param name="billId">Bill identifier.</param>
    /// <returns>Bill reference.</returns>
    public BillReference Bill(long billId) =>
        BillReference.ForEnvironment(_environment, billId);

    /// <summary>
    /// Creates a hire purchase resource reference.
    /// </summary>
    /// <param name="hirePurchaseId">Hire purchase identifier.</param>
    /// <returns>Hire purchase reference.</returns>
    public HirePurchaseReference HirePurchase(long hirePurchaseId) =>
        HirePurchaseReference.ForEnvironment(_environment, hirePurchaseId);

    /// <summary>
    /// Creates a credit note resource reference.
    /// </summary>
    /// <param name="creditNoteId">Credit note identifier.</param>
    /// <returns>Credit note reference.</returns>
    public CreditNoteReference CreditNote(long creditNoteId) =>
        CreditNoteReference.ForEnvironment(_environment, creditNoteId);

    /// <summary>
    /// Creates a credit note reconciliation resource reference.
    /// </summary>
    /// <param name="creditNoteReconciliationId">Credit note reconciliation identifier.</param>
    /// <returns>Credit note reconciliation reference.</returns>
    public CreditNoteReconciliationReference CreditNoteReconciliation(long creditNoteReconciliationId) =>
        CreditNoteReconciliationReference.ForEnvironment(_environment, creditNoteReconciliationId);

    /// <summary>
    /// Creates a stock item resource reference.
    /// </summary>
    /// <param name="stockItemId">Stock item identifier.</param>
    /// <returns>Stock item reference.</returns>
    public StockItemReference StockItem(long stockItemId) =>
        StockItemReference.ForEnvironment(_environment, stockItemId);

    /// <summary>
    /// Creates a capital asset resource reference.
    /// </summary>
    /// <param name="capitalAssetId">Capital asset identifier.</param>
    /// <returns>Capital asset reference.</returns>
    public CapitalAssetReference CapitalAsset(long capitalAssetId) =>
        CapitalAssetReference.ForEnvironment(_environment, capitalAssetId);

    /// <summary>
    /// Creates a capital asset type resource reference.
    /// </summary>
    /// <param name="capitalAssetTypeId">Capital asset type identifier.</param>
    /// <returns>Capital asset type reference.</returns>
    public CapitalAssetTypeReference CapitalAssetType(long capitalAssetTypeId) =>
        CapitalAssetTypeReference.ForEnvironment(_environment, capitalAssetTypeId);

    /// <summary>
    /// Creates a property resource reference.
    /// </summary>
    /// <param name="propertyId">Property identifier.</param>
    /// <returns>Property reference.</returns>
    public PropertyReference Property(long propertyId) =>
        PropertyReference.ForEnvironment(_environment, propertyId);
}
