using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.BankAccounts;

/// <summary>
/// Bank account type returned by the FreeAgent API.
/// </summary>
/// <remarks>
/// API apps may create <see cref="StandardBankAccount"/>, <see cref="CreditCardAccount"/>, and
/// <see cref="EcommerceAccount"/> only. Other values may be returned for integrated accounts.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BankAccountType>))]
public enum BankAccountType
{
    /// <summary>
    /// Standard business or personal bank account (default when no type is supplied on create).
    /// </summary>
    [JsonStringEnumMemberName("StandardBankAccount")]
    StandardBankAccount,

    /// <summary>
    /// Credit card account.
    /// </summary>
    [JsonStringEnumMemberName("CreditCardAccount")]
    CreditCardAccount,

    /// <summary>
    /// E-commerce payment account.
    /// </summary>
    [JsonStringEnumMemberName("EcommerceAccount")]
    EcommerceAccount,

    /// <summary>
    /// Rental statement account (read-only integration type).
    /// </summary>
    [JsonStringEnumMemberName("RentalStatementAccount")]
    RentalStatementAccount,

    /// <summary>
    /// Amazon Seller bank account (read-only integration type).
    /// </summary>
    [JsonStringEnumMemberName("AmazonSellerBankAccount")]
    AmazonSellerBankAccount,

    /// <summary>
    /// GoCardless bank account (read-only integration type).
    /// </summary>
    [JsonStringEnumMemberName("GocardlessBankAccount")]
    GocardlessBankAccount,

    /// <summary>
    /// PayPal Classic account (read-only integration type).
    /// </summary>
    [JsonStringEnumMemberName("PaypalClassicAccount")]
    PaypalClassicAccount,

    /// <summary>
    /// PayPal currency account (read-only integration type).
    /// </summary>
    [JsonStringEnumMemberName("Paypal::CurrencyAccount")]
    PaypalCurrencyAccount,

    /// <summary>
    /// Stripe bank account (read-only integration type).
    /// </summary>
    [JsonStringEnumMemberName("StripeBankAccount")]
    StripeBankAccount
}
