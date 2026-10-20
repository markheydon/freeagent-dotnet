using FreeAgent.Client.Models.BankAccounts;

namespace FreeAgent.Client.BlazorSample.Services;

/// <summary>
/// Documented bank account write operations exposed by the SDK for sample probing.
/// </summary>
public enum BankAccountWriteProbeVariant
{
    CreateStandard,
    CreateCreditCard,
    CreateEcommerce,
    Update
}

/// <summary>
/// Executes bank account write probes against the live API using typed SDK requests.
/// </summary>
public static class BankAccountWriteProbeSupport
{
    public static async Task<BankAccount> ExecuteAsync(
        FreeAgentClient client,
        BankAccountWriteProbeVariant variant,
        long? bankAccountId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        var probeName = $"SDK probe bank account {DateTime.UtcNow:yyyyMMddHHmmss}";

        return variant switch
        {
            BankAccountWriteProbeVariant.CreateStandard => await client.BankAccounts.CreateStandardBankAccountAsync(
                CreateStandardBankAccountRequest.Create(probeName, openingBalance: 0m),
                cancellationToken),
            BankAccountWriteProbeVariant.CreateCreditCard => await client.BankAccounts.CreateCreditCardBankAccountAsync(
                CreateCreditCardBankAccountRequest.Create(probeName, openingBalance: 0m, accountNumber: "1234"),
                cancellationToken),
            BankAccountWriteProbeVariant.CreateEcommerce => await client.BankAccounts.CreateEcommerceBankAccountAsync(
                CreateEcommerceBankAccountRequest.Create(probeName, openingBalance: 0m),
                cancellationToken),
            BankAccountWriteProbeVariant.Update => await client.BankAccounts.UpdateBankAccountAsync(
                bankAccountId ?? throw new InvalidOperationException("Bank account ID is required for update probes."),
                UpdateBankAccountRequest.Create(name: $"{probeName} (updated)"),
                cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, "Unsupported bank account write probe variant.")
        };
    }

    public static string GetSdkMethodName(BankAccountWriteProbeVariant variant) => variant switch
    {
        BankAccountWriteProbeVariant.CreateStandard => "CreateStandardBankAccountAsync",
        BankAccountWriteProbeVariant.CreateCreditCard => "CreateCreditCardBankAccountAsync",
        BankAccountWriteProbeVariant.CreateEcommerce => "CreateEcommerceBankAccountAsync",
        BankAccountWriteProbeVariant.Update => "UpdateBankAccountAsync",
        _ => "BankAccountsService"
    };
}
