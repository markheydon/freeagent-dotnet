using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Models.EcMoss;

namespace FreeAgent.Client.Models.BankTransactionExplanations;

/// <summary>
/// Shared bank transaction explanation create fields.
/// </summary>
public abstract class BankTransactionExplanationCreateRequestBase
{
    internal long? BankAccountId { get; init; }

    internal long? BankTransactionId { get; init; }

    internal DateOnly DatedOn { get; init; }

    internal decimal GrossValue { get; init; }

    internal string? Description { get; init; }

    internal BankTransactionExplanationEcStatus? EcStatus { get; init; }

    internal EcMossPlaceOfSupply? PlaceOfSupply { get; init; }

    internal decimal? SalesTaxRate { get; init; }

    internal decimal? SecondSalesTaxRate { get; init; }

    internal decimal? SalesTaxValue { get; init; }

    internal decimal? SecondSalesTaxValue { get; init; }

    internal BillSalesTaxStatus? SalesTaxStatus { get; init; }

    internal string? SecondSalesTaxStatus { get; init; }

    internal string? ChequeNumber { get; init; }

    internal BankTransactionExplanationAttachmentWritePayload? Attachment { get; init; }

    internal static void ValidateBankTarget(long? bankAccountId, long? bankTransactionId)
    {
        if (bankAccountId is not null && bankTransactionId is not null)
        {
            throw new ArgumentException("Specify either bankAccountId or bankTransactionId, not both.");
        }

        if (bankAccountId is null && bankTransactionId is null)
        {
            throw new ArgumentException("bankAccountId or bankTransactionId is required.");
        }

        if (bankAccountId is long accountId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(accountId);
        }

        if (bankTransactionId is long transactionId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(transactionId);
        }
    }
}
