using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.BankAccounts;

/// <summary>
/// Request attributes for updating a bank account.
/// </summary>
public sealed class UpdateBankAccountRequest
{
    internal string? Name { get; private init; }

    internal CurrencyCode? Currency { get; private init; }

    internal bool? IsPersonal { get; private init; }

    internal BankAccountStatus? Status { get; private init; }

    internal string? BankName { get; private init; }

    internal decimal? OpeningBalance { get; private init; }

    internal string? BankCode { get; private init; }

    internal bool? BankGuessEnabled { get; private init; }

    internal string? AccountNumber { get; private init; }

    internal string? SortCode { get; private init; }

    internal string? SecondarySortCode { get; private init; }

    internal string? Iban { get; private init; }

    internal string? Bic { get; private init; }

    internal string? Email { get; private init; }

    /// <summary>
    /// Creates a request to update bank account attributes.
    /// </summary>
    /// <param name="name">Updated account name.</param>
    /// <param name="openingBalance">Updated opening balance.</param>
    /// <param name="status">Updated visibility status.</param>
    /// <param name="currency">Updated accounting currency.</param>
    /// <param name="isPersonal">Updated personal flag.</param>
    /// <param name="bankName">Updated bank name.</param>
    /// <param name="bankCode">Updated bank identifier constant.</param>
    /// <param name="bankGuessEnabled">Updated bank guess flag.</param>
    /// <param name="accountNumber">Updated account number.</param>
    /// <param name="sortCode">Updated sort code.</param>
    /// <param name="secondarySortCode">Updated secondary sort code.</param>
    /// <param name="iban">Updated IBAN.</param>
    /// <param name="bic">Updated BIC.</param>
    /// <param name="email">Updated PayPal login email.</param>
    /// <returns>An update-bank-account request.</returns>
    public static UpdateBankAccountRequest Create(
        string? name = null,
        decimal? openingBalance = null,
        BankAccountStatus? status = null,
        CurrencyCode? currency = null,
        bool? isPersonal = null,
        string? bankName = null,
        string? bankCode = null,
        bool? bankGuessEnabled = null,
        string? accountNumber = null,
        string? sortCode = null,
        string? secondarySortCode = null,
        string? iban = null,
        string? bic = null,
        string? email = null) =>
        new()
        {
            Name = name,
            OpeningBalance = openingBalance,
            Status = status,
            Currency = currency,
            IsPersonal = isPersonal,
            BankName = bankName,
            BankCode = bankCode,
            BankGuessEnabled = bankGuessEnabled,
            AccountNumber = accountNumber,
            SortCode = sortCode,
            SecondarySortCode = secondarySortCode,
            Iban = iban,
            Bic = bic,
            Email = email
        };
}
