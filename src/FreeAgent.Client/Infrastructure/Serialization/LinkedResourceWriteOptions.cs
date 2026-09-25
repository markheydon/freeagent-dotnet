namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Per-update flags for omitting linked resources from write payloads.
/// </summary>
internal readonly record struct LinkedResourceWriteOptions(
    bool OmitContact = false,
    bool OmitProject = false,
    bool OmitBankAccount = false)
{
    public static LinkedResourceWriteOptions FromInvoiceUpdate(InvoiceUpdateOptions? options) =>
        options is null
            ? default
            : new LinkedResourceWriteOptions(
                OmitContact: options.OmitContact,
                OmitProject: options.OmitProject,
                OmitBankAccount: options.OmitBankAccount);

    public static LinkedResourceWriteOptions FromEstimateUpdate(EstimateUpdateOptions? options) =>
        options is null
            ? default
            : new LinkedResourceWriteOptions(
                OmitContact: options.OmitContact,
                OmitProject: options.OmitProject);

    public static LinkedResourceWriteOptions FromCreditNoteUpdate(CreditNoteUpdateOptions? options) =>
        options is null
            ? default
            : new LinkedResourceWriteOptions(
                OmitContact: options.OmitContact,
                OmitProject: options.OmitProject,
                OmitBankAccount: options.OmitBankAccount);

    public static LinkedResourceWriteOptions FromProjectUpdate(ProjectUpdateOptions? options) =>
        options is null
            ? default
            : new LinkedResourceWriteOptions(OmitContact: options.OmitContact);
}
