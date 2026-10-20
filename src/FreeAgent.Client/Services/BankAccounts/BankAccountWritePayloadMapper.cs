using FreeAgent.Client.Models.BankAccounts;

namespace FreeAgent.Client.Services.BankAccounts;

internal static class BankAccountWritePayloadMapper
{
    public static BankAccountWritePayload FromCreate(CreateStandardBankAccountRequest request) =>
        new()
        {
            Type = BankAccountType.StandardBankAccount,
            Name = request.Name,
            OpeningBalance = request.OpeningBalance,
            Currency = request.Currency,
            IsPersonal = request.IsPersonal,
            BankName = request.BankName,
            BankCode = request.BankCode,
            BankGuessEnabled = request.BankGuessEnabled,
            AccountNumber = request.AccountNumber,
            SortCode = request.SortCode,
            SecondarySortCode = request.SecondarySortCode,
            Iban = request.Iban,
            Bic = request.Bic
        };

    public static BankAccountWritePayload FromCreate(CreateCreditCardBankAccountRequest request) =>
        new()
        {
            Type = BankAccountType.CreditCardAccount,
            Name = request.Name,
            OpeningBalance = request.OpeningBalance,
            AccountNumber = request.AccountNumber,
            Currency = request.Currency,
            IsPersonal = request.IsPersonal,
            BankName = request.BankName,
            BankCode = request.BankCode,
            BankGuessEnabled = request.BankGuessEnabled
        };

    public static BankAccountWritePayload FromCreate(CreateEcommerceBankAccountRequest request) =>
        new()
        {
            Type = BankAccountType.EcommerceAccount,
            Name = request.Name,
            OpeningBalance = request.OpeningBalance,
            Currency = request.Currency,
            IsPersonal = request.IsPersonal,
            BankName = request.BankName,
            BankCode = request.BankCode,
            BankGuessEnabled = request.BankGuessEnabled,
            Email = request.Email
        };

    public static BankAccountWritePayload FromUpdate(UpdateBankAccountRequest request) =>
        new()
        {
            Name = request.Name,
            OpeningBalance = request.OpeningBalance,
            Status = request.Status,
            Currency = request.Currency,
            IsPersonal = request.IsPersonal,
            BankName = request.BankName,
            BankCode = request.BankCode,
            BankGuessEnabled = request.BankGuessEnabled,
            AccountNumber = request.AccountNumber,
            SortCode = request.SortCode,
            SecondarySortCode = request.SecondarySortCode,
            Iban = request.Iban,
            Bic = request.Bic,
            Email = request.Email
        };
}
