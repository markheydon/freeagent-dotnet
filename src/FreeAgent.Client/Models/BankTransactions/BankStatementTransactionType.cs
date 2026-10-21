using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Statement upload <c>transaction_type</c> values.
/// </summary>
/// <remarks>
/// FreeAgent applies sign rules from the type. When omitted, the API defaults to <c>OTHER</c>.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BankStatementTransactionType>))]
public enum BankStatementTransactionType
{
    /// <summary>Generic credit. Treated as positive regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("CREDIT")]
    Credit,

    /// <summary>Generic debit. Treated as negative regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("DEBIT")]
    Debit,

    /// <summary>Interest earned or paid. Sign follows the amount.</summary>
    [JsonStringEnumMemberName("INT")]
    Interest,

    /// <summary>Dividend. Treated as positive regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("DIV")]
    Dividend,

    /// <summary>Financial-institution fee. Treated as negative regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("FEE")]
    Fee,

    /// <summary>Service charge. Treated as negative regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("SRVCHG")]
    ServiceCharge,

    /// <summary>Deposit. Treated as positive regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("DEP")]
    Deposit,

    /// <summary>ATM debit or credit. Sign follows the amount.</summary>
    [JsonStringEnumMemberName("ATM")]
    Atm,

    /// <summary>Point-of-sale debit or credit. Sign follows the amount.</summary>
    [JsonStringEnumMemberName("POS")]
    PointOfSale,

    /// <summary>Transfer. Treated as negative regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("XFER")]
    Transfer,

    /// <summary>Cheque. Treated as negative regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("CHECK")]
    Cheque,

    /// <summary>Electronic payment. Treated as negative regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("PAYMENT")]
    Payment,

    /// <summary>Cash withdrawal. Treated as negative regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("CASH")]
    Cash,

    /// <summary>Direct deposit. Treated as positive regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("DIRECTDEP")]
    DirectDeposit,

    /// <summary>Merchant-initiated debit. Treated as negative regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("DIRECTDEBIT")]
    DirectDebit,

    /// <summary>Repeating payment or standing order. Treated as negative regardless of the amount sign.</summary>
    [JsonStringEnumMemberName("REPEATPMT")]
    RepeatingPayment,

    /// <summary>Other debit or credit. Sign follows the amount. This is the API default when the field is omitted.</summary>
    [JsonStringEnumMemberName("OTHER")]
    Other,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
