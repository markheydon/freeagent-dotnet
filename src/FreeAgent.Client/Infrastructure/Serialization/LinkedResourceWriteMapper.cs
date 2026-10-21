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

    public static ProjectTaskReference? ToProjectTaskReference(FreeAgentEnvironment environment, long? projectTaskId) =>
        projectTaskId is long id ? ProjectTaskReference.ForEnvironment(environment, id) : null;

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

    public static string? ToPaidInvoiceWireUri(
        FreeAgentEnvironment environment,
        long? invoiceId,
        long? creditNoteId)
    {
        if (creditNoteId is long creditNote)
        {
            return CreditNoteReference.ForEnvironment(environment, creditNote).Uri;
        }

        return invoiceId is long invoice
            ? InvoiceReference.ForEnvironment(environment, invoice).Uri
            : null;
    }

    public static BillReference? ToBillReference(FreeAgentEnvironment environment, long? billId) =>
        billId is long id ? BillReference.ForEnvironment(environment, id) : null;

    public static BankTransactionReference? ToBankTransactionReference(FreeAgentEnvironment environment, long? bankTransactionId) =>
        bankTransactionId is long id ? BankTransactionReference.ForEnvironment(environment, id) : null;

    public static CapitalAssetReference? ToCapitalAssetReference(FreeAgentEnvironment environment, long? capitalAssetId) =>
        capitalAssetId is long id ? CapitalAssetReference.ForEnvironment(environment, id) : null;

    public static CapitalAssetTypeReference? ToCapitalAssetTypeReference(FreeAgentEnvironment environment, long? capitalAssetTypeId) =>
        capitalAssetTypeId is long id ? CapitalAssetTypeReference.ForEnvironment(environment, id) : null;

    public static PropertyReference? ToPropertyReference(FreeAgentEnvironment environment, long? propertyId) =>
        propertyId is long id ? PropertyReference.ForEnvironment(environment, id) : null;

    public static WriteLink<CapitalAssetTypeReference>? ResolveCapitalAssetTypeReference(
        FreeAgentEnvironment environment,
        in SettableLinkId backing,
        long? linkId,
        bool omit = false)
    {
        if (omit)
        {
            return null;
        }

        if (backing.IsExplicitlySet)
        {
            return backing.Get(fromLink: null) is long id
                ? new WriteLink<CapitalAssetTypeReference> { Value = CapitalAssetTypeReference.ForEnvironment(environment, id) }
                : new WriteLink<CapitalAssetTypeReference> { IsCleared = true };
        }

        return linkId is long roundTripped
            ? new WriteLink<CapitalAssetTypeReference> { Value = CapitalAssetTypeReference.ForEnvironment(environment, roundTripped) }
            : null;
    }

    public static WriteLink<PropertyReference>? ResolvePropertyReference(
        FreeAgentEnvironment environment,
        in SettableLinkId backing,
        long? linkId,
        bool omit = false)
    {
        if (omit)
        {
            return null;
        }

        if (backing.IsExplicitlySet)
        {
            return backing.Get(fromLink: null) is long id
                ? new WriteLink<PropertyReference> { Value = PropertyReference.ForEnvironment(environment, id) }
                : new WriteLink<PropertyReference> { IsCleared = true };
        }

        return linkId is long roundTripped
            ? new WriteLink<PropertyReference> { Value = PropertyReference.ForEnvironment(environment, roundTripped) }
            : null;
    }

    public static WriteLink<ContactReference>? ResolveContactReference(
        FreeAgentEnvironment environment,
        in SettableLinkId backing,
        long? linkId,
        bool omit = false)
    {
        if (omit)
        {
            return null;
        }

        if (backing.IsExplicitlySet)
        {
            return backing.Get(fromLink: null) is long id
                ? new WriteLink<ContactReference> { Value = ContactReference.ForEnvironment(environment, id) }
                : new WriteLink<ContactReference> { IsCleared = true };
        }

        return linkId is long roundTripped
            ? new WriteLink<ContactReference> { Value = ContactReference.ForEnvironment(environment, roundTripped) }
            : null;
    }

    public static WriteLink<ProjectReference>? ResolveProjectReference(
        FreeAgentEnvironment environment,
        in SettableLinkId backing,
        long? linkId,
        bool omit = false)
    {
        if (omit)
        {
            return null;
        }

        if (backing.IsExplicitlySet)
        {
            return backing.Get(fromLink: null) is long id
                ? new WriteLink<ProjectReference> { Value = ProjectReference.ForEnvironment(environment, id) }
                : new WriteLink<ProjectReference> { IsCleared = true };
        }

        return linkId is long roundTripped
            ? new WriteLink<ProjectReference> { Value = ProjectReference.ForEnvironment(environment, roundTripped) }
            : null;
    }

    public static WriteLink<ProjectTaskReference>? ResolveProjectTaskReference(
        FreeAgentEnvironment environment,
        in SettableLinkId backing,
        long? linkId,
        bool omit = false)
    {
        if (omit)
        {
            return null;
        }

        if (backing.IsExplicitlySet)
        {
            return backing.Get(fromLink: null) is long id
                ? new WriteLink<ProjectTaskReference> { Value = ProjectTaskReference.ForEnvironment(environment, id) }
                : new WriteLink<ProjectTaskReference> { IsCleared = true };
        }

        return linkId is long roundTripped
            ? new WriteLink<ProjectTaskReference> { Value = ProjectTaskReference.ForEnvironment(environment, roundTripped) }
            : null;
    }

    public static WriteLink<UserReference>? ResolveUserReference(
        FreeAgentEnvironment environment,
        in SettableLinkId backing,
        long? linkId,
        bool omit = false)
    {
        if (omit)
        {
            return null;
        }

        if (backing.IsExplicitlySet)
        {
            return backing.Get(fromLink: null) is long id
                ? new WriteLink<UserReference> { Value = UserReference.ForEnvironment(environment, id) }
                : new WriteLink<UserReference> { IsCleared = true };
        }

        return linkId is long roundTripped
            ? new WriteLink<UserReference> { Value = UserReference.ForEnvironment(environment, roundTripped) }
            : null;
    }

    public static WriteLink<BankAccountReference>? ResolveBankAccountReference(
        FreeAgentEnvironment environment,
        in SettableLinkId backing,
        long? linkId,
        bool omit = false)
    {
        if (omit)
        {
            return null;
        }

        if (backing.IsExplicitlySet)
        {
            return backing.Get(fromLink: null) is long id
                ? new WriteLink<BankAccountReference> { Value = BankAccountReference.ForEnvironment(environment, id) }
                : new WriteLink<BankAccountReference> { IsCleared = true };
        }

        return linkId is long roundTripped
            ? new WriteLink<BankAccountReference> { Value = BankAccountReference.ForEnvironment(environment, roundTripped) }
            : null;
    }

    public static WriteLink<StockItemReference>? ResolveStockItemReference(
        FreeAgentEnvironment environment,
        in SettableLinkId backing,
        long? linkId,
        bool omit = false)
    {
        if (omit)
        {
            return null;
        }

        if (backing.IsExplicitlySet)
        {
            return backing.Get(fromLink: null) is long id
                ? new WriteLink<StockItemReference> { Value = StockItemReference.ForEnvironment(environment, id) }
                : new WriteLink<StockItemReference> { IsCleared = true };
        }

        return linkId is long roundTripped
            ? new WriteLink<StockItemReference> { Value = StockItemReference.ForEnvironment(environment, roundTripped) }
            : null;
    }

    public static WriteLink<CategoryReference>? ResolveCategoryReference(
        FreeAgentEnvironment environment,
        in SettableLinkValue backing,
        string? linkNominalCode,
        bool omit = false)
    {
        if (omit)
        {
            return null;
        }

        if (backing.IsExplicitlySet)
        {
            var code = backing.Get(fromLink: null);
            return string.IsNullOrWhiteSpace(code)
                ? new WriteLink<CategoryReference> { IsCleared = true }
                : new WriteLink<CategoryReference> { Value = CategoryReference.ForEnvironment(environment, code) };
        }

        return string.IsNullOrWhiteSpace(linkNominalCode)
            ? null
            : new WriteLink<CategoryReference> { Value = CategoryReference.ForEnvironment(environment, linkNominalCode) };
    }

    public static ExpenseCategoryWriteValue? ResolveExpenseCategoryWrite(
        FreeAgentEnvironment environment,
        bool? isMileageExplicit,
        bool isMileageFromLink,
        in SettableLinkValue categoryBacking,
        string? linkNominalCode)
    {
        if (isMileageExplicit == true)
        {
            return ExpenseCategoryWriteValue.Mileage();
        }

        if (categoryBacking.IsExplicitlySet)
        {
            var code = categoryBacking.Get(fromLink: null);
            if (!string.IsNullOrWhiteSpace(code))
            {
                return ExpenseCategoryWriteValue.ForCategory(CategoryReference.ForEnvironment(environment, code));
            }

            if (isMileageExplicit != false && isMileageFromLink)
            {
                return ExpenseCategoryWriteValue.Mileage();
            }

            return null;
        }

        if (isMileageExplicit == false)
        {
            return string.IsNullOrWhiteSpace(linkNominalCode)
                ? null
                : ExpenseCategoryWriteValue.ForCategory(CategoryReference.ForEnvironment(environment, linkNominalCode));
        }

        if (isMileageFromLink)
        {
            return ExpenseCategoryWriteValue.Mileage();
        }

        return string.IsNullOrWhiteSpace(linkNominalCode)
            ? null
            : ExpenseCategoryWriteValue.ForCategory(CategoryReference.ForEnvironment(environment, linkNominalCode));
    }
}
