using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Fail-fast checks for sales document writes using constraints stated in the FreeAgent API docs.
/// </summary>
internal static class SalesDocumentWriteValidator
{
    internal static void ValidateInvoiceForCreate(Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        ValidateSalesDocumentHeader(invoice.ContactId, invoice.DatedOn, invoice.PaymentTermsInDays, nameof(invoice));
        ValidateInvoiceLineItems(invoice.InvoiceItems, nameof(invoice));
    }

    internal static void ValidateInvoiceForUpdate(Invoice invoice, bool omitLineItems)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        ValidateContactNotExplicitlyCleared(invoice.ContactIdBacking, invoice.ContactId, nameof(invoice));
        if (omitLineItems)
        {
            ValidateSalesDocumentHeader(invoice.ContactId, invoice.DatedOn, invoice.PaymentTermsInDays, nameof(invoice));
        }

        if (!omitLineItems)
        {
            ValidateInvoiceLineItemsForUpdate(invoice.InvoiceItems, nameof(invoice));
        }
    }

    internal static void ValidateEstimateForCreate(Estimate estimate)
    {
        ArgumentNullException.ThrowIfNull(estimate);
        ValidateDocumentHeader(estimate.ContactId, estimate.DatedOn, nameof(estimate));
        ValidateEstimateLineItems(estimate.EstimateItems, nameof(estimate));
    }

    internal static void ValidateEstimateForUpdate(Estimate estimate, bool omitLineItems)
    {
        ArgumentNullException.ThrowIfNull(estimate);
        ValidateContactNotExplicitlyCleared(estimate.ContactIdBacking, estimate.ContactId, nameof(estimate));
        if (omitLineItems)
        {
            ValidateDocumentHeader(estimate.ContactId, estimate.DatedOn, nameof(estimate));
        }

        if (!omitLineItems)
        {
            ValidateEstimateLineItemsForUpdate(estimate.EstimateItems, nameof(estimate));
        }
    }

    internal static void ValidateCreditNoteForCreate(CreditNote creditNote)
    {
        ArgumentNullException.ThrowIfNull(creditNote);
        ValidateSalesDocumentHeader(creditNote.ContactId, creditNote.DatedOn, creditNote.PaymentTermsInDays, nameof(creditNote));
        ValidateCreditNoteLineItems(creditNote.CreditNoteItems, nameof(creditNote));
    }

    internal static void ValidateCreditNoteForUpdate(CreditNote creditNote, bool omitLineItems)
    {
        ArgumentNullException.ThrowIfNull(creditNote);
        ValidateContactNotExplicitlyCleared(creditNote.ContactIdBacking, creditNote.ContactId, nameof(creditNote));
        if (omitLineItems)
        {
            ValidateSalesDocumentHeader(
                creditNote.ContactId,
                creditNote.DatedOn,
                creditNote.PaymentTermsInDays,
                nameof(creditNote));
        }

        if (!omitLineItems)
        {
            ValidateCreditNoteLineItemsForUpdate(creditNote.CreditNoteItems, nameof(creditNote));
        }
    }

    internal static void ValidateBillForCreate(Bill bill)
    {
        ArgumentNullException.ThrowIfNull(bill);
        ValidateBillHeader(bill.ContactId, bill.DatedOn, bill.DueOn, nameof(bill));
        ValidateBillLineItems(bill.BillItems, nameof(bill));
    }

    internal static void ValidateBillForUpdate(Bill bill, bool omitLineItems)
    {
        ArgumentNullException.ThrowIfNull(bill);
        ValidateContactNotExplicitlyCleared(bill.ContactIdBacking, bill.ContactId, nameof(bill));
        if (omitLineItems)
        {
            ValidateBillHeader(bill.ContactId, bill.DatedOn, bill.DueOn, nameof(bill));
        }

        if (!omitLineItems)
        {
            ValidateBillLineItemsForUpdate(bill.BillItems, nameof(bill));
        }
    }

    private static void ValidateContactNotExplicitlyCleared(
        SettableLinkId contactBacking,
        long? contactId,
        string paramName)
    {
        if (contactBacking.IsExplicitlySet && contactId is null)
        {
            throw new ArgumentException("Contact cannot be cleared.", paramName);
        }
    }

    private static void ValidateDocumentHeader(long? contactId, DateOnly? datedOn, string paramName)
    {
        if (contactId is null)
        {
            throw new ArgumentException("Contact is required.", paramName);
        }

        if (datedOn is null)
        {
            throw new ArgumentException("Dated on is required.", paramName);
        }
    }

    private static void ValidateSalesDocumentHeader(
        long? contactId,
        DateOnly? datedOn,
        int? paymentTermsInDays,
        string paramName)
    {
        ValidateDocumentHeader(contactId, datedOn, paramName);

        if (paymentTermsInDays is null)
        {
            throw new ArgumentException("Payment terms in days is required.", paramName);
        }
    }

    private static void ValidateBillHeader(
        long? contactId,
        DateOnly? datedOn,
        DateOnly? dueOn,
        string paramName)
    {
        if (contactId is null)
        {
            throw new ArgumentException("Contact is required.", paramName);
        }

        if (datedOn is null)
        {
            throw new ArgumentException("Dated on is required.", paramName);
        }

        if (dueOn is null)
        {
            throw new ArgumentException("Due on is required.", paramName);
        }
    }

    private static void ValidateInvoiceLineItems(List<InvoiceItem>? items, string paramName)
    {
        if (items is null || items.Count == 0)
        {
            throw new ArgumentException("At least one invoice line item is required.", paramName);
        }

        foreach (var item in items)
        {
            ValidateInvoiceLineItemForCreate(item, paramName);
        }
    }

    private static void ValidateInvoiceLineItemsForUpdate(List<InvoiceItem>? items, string paramName)
    {
        if (items is null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item.Destroy == 1)
            {
                RequireLineItemId(item.ItemId, paramName);
                continue;
            }

            if (item.ItemId is not null)
            {
                continue;
            }

            ValidateInvoiceLineItemForCreate(item, paramName);
        }
    }

    private static void ValidateInvoiceLineItemForCreate(InvoiceItem item, string paramName)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (string.IsNullOrWhiteSpace(item.Description))
        {
            throw new ArgumentException("Invoice line description is required.", paramName);
        }

        if (item.ItemType == InvoiceItemType.Stock && item.StockItemId is null)
        {
            throw new ArgumentException("Stock item is required when the line item type is Stock.", paramName);
        }

        if (RequiresUnitPrice(item.ItemType) && item.Price is null)
        {
            throw new ArgumentException("Price is required for this invoice line item type.", paramName);
        }
    }

    private static void ValidateEstimateLineItems(List<EstimateItem>? items, string paramName)
    {
        if (items is null || items.Count == 0)
        {
            throw new ArgumentException("At least one estimate line item is required.", paramName);
        }

        foreach (var item in items)
        {
            ValidateEstimateLineItemForCreate(item, paramName);
        }
    }

    private static void ValidateEstimateLineItemsForUpdate(List<EstimateItem>? items, string paramName)
    {
        if (items is null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item.Destroy == 1)
            {
                RequireLineItemId(item.ItemId, paramName);
                continue;
            }

            if (item.ItemId is not null)
            {
                continue;
            }

            ValidateEstimateLineItemForCreate(item, paramName);
        }
    }

    private static void ValidateEstimateLineItemForCreate(EstimateItem item, string paramName)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (string.IsNullOrWhiteSpace(item.Description))
        {
            throw new ArgumentException("Estimate line description is required.", paramName);
        }

        if (RequiresEstimateUnitPrice(item.ItemType) && item.Price is null)
        {
            throw new ArgumentException("Price is required for this estimate line item type.", paramName);
        }
    }

    private static void ValidateCreditNoteLineItems(List<CreditNoteItem>? items, string paramName)
    {
        if (items is null || items.Count == 0)
        {
            throw new ArgumentException("At least one credit note line item is required.", paramName);
        }

        foreach (var item in items)
        {
            ValidateCreditNoteLineItemForCreate(item, paramName);
        }
    }

    private static void ValidateCreditNoteLineItemsForUpdate(List<CreditNoteItem>? items, string paramName)
    {
        if (items is null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item.Destroy == 1)
            {
                RequireLineItemId(item.ItemId, paramName);
                continue;
            }

            if (item.ItemId is not null)
            {
                continue;
            }

            ValidateCreditNoteLineItemForCreate(item, paramName);
        }
    }

    private static void ValidateCreditNoteLineItemForCreate(CreditNoteItem item, string paramName)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (string.IsNullOrWhiteSpace(item.Description))
        {
            throw new ArgumentException("Credit note line description is required.", paramName);
        }

        if (item.ItemType == InvoiceItemType.Stock && item.StockItemId is null)
        {
            throw new ArgumentException("Stock item is required when the line item type is Stock.", paramName);
        }

        if (RequiresUnitPrice(item.ItemType) && item.Price is null)
        {
            throw new ArgumentException("Price is required for this credit note line item type.", paramName);
        }
    }

    private static void ValidateBillLineItems(List<BillItem>? items, string paramName)
    {
        if (items is null || items.Count == 0)
        {
            throw new ArgumentException("At least one bill line item is required.", paramName);
        }

        foreach (var item in items)
        {
            ValidateBillLineItemForCreate(item, paramName);
        }
    }

    private static void ValidateBillLineItemsForUpdate(List<BillItem>? items, string paramName)
    {
        if (items is null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item.Destroy == 1)
            {
                RequireLineItemId(item.ItemId, paramName);
                continue;
            }

            if (item.ItemId is not null)
            {
                continue;
            }

            ValidateBillLineItemForCreate(item, paramName);
        }
    }

    private static void ValidateBillLineItemForCreate(BillItem item, string paramName)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (string.IsNullOrWhiteSpace(item.Description))
        {
            throw new ArgumentException("Bill line description is required.", paramName);
        }

        if (item.StockItemId is not null && item.StockAlteringQuantity is null)
        {
            throw new ArgumentException("Stock altering quantity is required when a stock item is set.", paramName);
        }
    }

    private static bool RequiresUnitPrice(InvoiceItemType? itemType)
    {
        if (itemType is null)
        {
            return true;
        }

        return itemType switch
        {
            InvoiceItemType.Hours => false,
            InvoiceItemType.Days => false,
            InvoiceItemType.Weeks => false,
            InvoiceItemType.Months => false,
            InvoiceItemType.Years => false,
            InvoiceItemType.Comment => false,
            _ => true
        };
    }

    private static bool RequiresEstimateUnitPrice(EstimateItemType? itemType)
    {
        if (itemType is null)
        {
            return true;
        }

        return itemType switch
        {
            EstimateItemType.Hours => false,
            EstimateItemType.Days => false,
            EstimateItemType.Weeks => false,
            EstimateItemType.Months => false,
            EstimateItemType.Years => false,
            EstimateItemType.Comments => false,
            _ => true
        };
    }

    private static void RequireLineItemId(long? itemId, string paramName)
    {
        if (itemId is null)
        {
            throw new ArgumentException("Line item id is required when updating or deleting a line item.", paramName);
        }
    }
}
