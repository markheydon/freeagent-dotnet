using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for <c>GET /v2/bank_accounts</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BankAccountView>))]
public enum BankAccountView
{
    /// <summary>Standard bank accounts only.</summary>
    [JsonStringEnumMemberName("standard_bank_accounts")]
    StandardBankAccounts,

    /// <summary>Credit card accounts only.</summary>
    [JsonStringEnumMemberName("credit_card_accounts")]
    CreditCardAccounts,

    /// <summary>PayPal accounts only.</summary>
    [JsonStringEnumMemberName("paypal_accounts")]
    PaypalAccounts,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
