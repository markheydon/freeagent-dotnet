using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Read-only bank transaction explanation type returned by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BankTransactionExplanationType>))]
public enum BankTransactionExplanationType
{
    /// <summary>Payment explanation.</summary>
    [JsonStringEnumMemberName("Payment")]
    Payment,

    /// <summary>Refund explanation.</summary>
    [JsonStringEnumMemberName("Refund")]
    Refund,

    /// <summary>Invoice receipt explanation.</summary>
    [JsonStringEnumMemberName("Invoice Receipt")]
    InvoiceReceipt,

    /// <summary>Credit note refund explanation.</summary>
    [JsonStringEnumMemberName("Credit Note Refund")]
    CreditNoteRefund,

    /// <summary>Bill payment explanation.</summary>
    [JsonStringEnumMemberName("Bill Payment")]
    BillPayment,

    /// <summary>Bill refund explanation.</summary>
    [JsonStringEnumMemberName("Bill Refund")]
    BillRefund,

    /// <summary>Money paid to user explanation.</summary>
    [JsonStringEnumMemberName("Money Paid to User")]
    MoneyPaidToUser,

    /// <summary>Money received from user explanation.</summary>
    [JsonStringEnumMemberName("Money Received from User")]
    MoneyReceivedFromUser,

    /// <summary>Transfer to another account explanation.</summary>
    [JsonStringEnumMemberName("Transfer to Another Account")]
    TransferToAnotherAccount,

    /// <summary>Transfer from another account explanation.</summary>
    [JsonStringEnumMemberName("Transfer from Another Account")]
    TransferFromAnotherAccount,

    /// <summary>Purchase of stock explanation.</summary>
    [JsonStringEnumMemberName("Purchase of Stock")]
    PurchaseOfStock,

    /// <summary>Sale of stock explanation.</summary>
    [JsonStringEnumMemberName("Sale of Stock")]
    SaleOfStock,

    /// <summary>Purchase of capital asset explanation.</summary>
    [JsonStringEnumMemberName("Purchase of Capital Asset")]
    PurchaseOfCapitalAsset,

    /// <summary>Disposal of capital asset explanation.</summary>
    [JsonStringEnumMemberName("Disposal of Capital Asset")]
    DisposalOfCapitalAsset,

    /// <summary>Receipt from initial debtor explanation.</summary>
    [JsonStringEnumMemberName("Receipt from Initial Debtor")]
    ReceiptFromInitialDebtor,

    /// <summary>Payment to initial creditor explanation.</summary>
    [JsonStringEnumMemberName("Payment to Initial Creditor")]
    PaymentToInitialCreditor,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
