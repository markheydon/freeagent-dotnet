using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.BankAccounts;

/// <summary>
/// Request attributes for creating a standard bank account.
/// </summary>
public sealed class CreateStandardBankAccountRequest
{
    internal string Name { get; private init; } = string.Empty;

    internal decimal OpeningBalance { get; private init; }

    internal CurrencyCode? Currency { get; private init; }

    internal bool? IsPersonal { get; private init; }

    internal string? BankName { get; private init; }

    internal string? BankCode { get; private init; }

    internal bool? BankGuessEnabled { get; private init; }

    internal string? AccountNumber { get; private init; }

    internal string? SortCode { get; private init; }

    internal string? SecondarySortCode { get; private init; }

    internal string? Iban { get; private init; }

    internal string? Bic { get; private init; }

    /// <summary>
    /// Creates a request for a new standard bank account.
    /// </summary>
    /// <param name="name">Account name.</param>
    /// <param name="openingBalance">Balance at the FreeAgent start date (use zero for accounts opened after that date).</param>
    /// <param name="currency">Accounting currency; defaults to the company native currency when omitted.</param>
    /// <param name="isPersonal">Whether the account is personal.</param>
    /// <param name="bankName">Bank name.</param>
    /// <param name="bankCode">Bank identifier constant (for example <c>barclays</c>).</param>
    /// <param name="bankGuessEnabled">Whether bank guess is enabled.</param>
    /// <param name="accountNumber">Bank account number.</param>
    /// <param name="sortCode">Sort code or routing number.</param>
    /// <param name="secondarySortCode">Secondary sort code when applicable.</param>
    /// <param name="iban">International Bank Account Number.</param>
    /// <param name="bic">Bank Identifier Code.</param>
    /// <returns>A create-standard-bank-account request.</returns>
    public static CreateStandardBankAccountRequest Create(
        string name,
        decimal openingBalance,
        CurrencyCode? currency = null,
        bool? isPersonal = null,
        string? bankName = null,
        string? bankCode = null,
        bool? bankGuessEnabled = null,
        string? accountNumber = null,
        string? sortCode = null,
        string? secondarySortCode = null,
        string? iban = null,
        string? bic = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new CreateStandardBankAccountRequest
        {
            Name = name,
            OpeningBalance = openingBalance,
            Currency = currency,
            IsPersonal = isPersonal,
            BankName = bankName,
            BankCode = bankCode,
            BankGuessEnabled = bankGuessEnabled,
            AccountNumber = accountNumber,
            SortCode = sortCode,
            SecondarySortCode = secondarySortCode,
            Iban = iban,
            Bic = bic
        };
    }
}

/// <summary>
/// Request attributes for creating a credit card account.
/// </summary>
public sealed class CreateCreditCardBankAccountRequest
{
    internal string Name { get; private init; } = string.Empty;

    internal decimal OpeningBalance { get; private init; }

    internal string AccountNumber { get; private init; } = string.Empty;

    internal CurrencyCode? Currency { get; private init; }

    internal bool? IsPersonal { get; private init; }

    internal string? BankName { get; private init; }

    internal string? BankCode { get; private init; }

    internal bool? BankGuessEnabled { get; private init; }

    /// <summary>
    /// Creates a request for a new credit card account.
    /// </summary>
    /// <param name="name">Account name.</param>
    /// <param name="openingBalance">Balance at the FreeAgent start date.</param>
    /// <param name="accountNumber">Last four digits of the card number.</param>
    /// <param name="currency">Accounting currency.</param>
    /// <param name="isPersonal">Whether the account is personal.</param>
    /// <param name="bankName">Bank name.</param>
    /// <param name="bankCode">Bank identifier constant.</param>
    /// <param name="bankGuessEnabled">Whether bank guess is enabled.</param>
    /// <returns>A create-credit-card-bank-account request.</returns>
    public static CreateCreditCardBankAccountRequest Create(
        string name,
        decimal openingBalance,
        string accountNumber,
        CurrencyCode? currency = null,
        bool? isPersonal = null,
        string? bankName = null,
        string? bankCode = null,
        bool? bankGuessEnabled = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(accountNumber);

        return new CreateCreditCardBankAccountRequest
        {
            Name = name,
            OpeningBalance = openingBalance,
            AccountNumber = accountNumber,
            Currency = currency,
            IsPersonal = isPersonal,
            BankName = bankName,
            BankCode = bankCode,
            BankGuessEnabled = bankGuessEnabled
        };
    }
}

/// <summary>
/// Request attributes for creating an e-commerce bank account.
/// </summary>
public sealed class CreateEcommerceBankAccountRequest
{
    internal string Name { get; private init; } = string.Empty;

    internal decimal OpeningBalance { get; private init; }

    internal CurrencyCode? Currency { get; private init; }

    internal bool? IsPersonal { get; private init; }

    internal string? BankName { get; private init; }

    internal string? BankCode { get; private init; }

    internal bool? BankGuessEnabled { get; private init; }

    internal string? Email { get; private init; }

    /// <summary>
    /// Creates a request for a new e-commerce bank account.
    /// </summary>
    /// <param name="name">Account name.</param>
    /// <param name="openingBalance">Balance at the FreeAgent start date.</param>
    /// <param name="currency">Accounting currency.</param>
    /// <param name="isPersonal">Whether the account is personal.</param>
    /// <param name="bankName">Bank name.</param>
    /// <param name="bankCode">Bank identifier constant.</param>
    /// <param name="bankGuessEnabled">Whether bank guess is enabled.</param>
    /// <param name="email">PayPal login email (required when creating a PayPal account).</param>
    /// <returns>A create-e-commerce-bank-account request.</returns>
    public static CreateEcommerceBankAccountRequest Create(
        string name,
        decimal openingBalance,
        CurrencyCode? currency = null,
        bool? isPersonal = null,
        string? bankName = null,
        string? bankCode = null,
        bool? bankGuessEnabled = null,
        string? email = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new CreateEcommerceBankAccountRequest
        {
            Name = name,
            OpeningBalance = openingBalance,
            Currency = currency,
            IsPersonal = isPersonal,
            BankName = bankName,
            BankCode = bankCode,
            BankGuessEnabled = bankGuessEnabled,
            Email = email
        };
    }
}
