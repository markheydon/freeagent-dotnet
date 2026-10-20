namespace FreeAgent.Client.Models.BankAccounts;

/// <summary>
/// Documented <c>view</c> query values for listing bank accounts.
/// </summary>
public static class BankAccountViews
{
    /// <summary>
    /// Standard bank accounts only.
    /// </summary>
    public const string StandardBankAccounts = "standard_bank_accounts";

    /// <summary>
    /// Credit card accounts only.
    /// </summary>
    public const string CreditCardAccounts = "credit_card_accounts";

    /// <summary>
    /// PayPal accounts only.
    /// </summary>
    public const string PaypalAccounts = "paypal_accounts";
}
