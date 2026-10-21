using System.Diagnostics.CodeAnalysis;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Bank accounts endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Bank accounts")]
internal sealed class BankAccountSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List bank accounts")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListBankAccountsAsync(CancellationToken cancellationToken)
    {
        var bankAccounts = await context.Client.BankAccounts.ListAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Bank accounts ({bankAccounts.Count})");
        SampleOutput.WriteRows(
            bankAccounts,
            account => $"{account.ResourceId,8}  {account.Name}  ({account.Type}, {account.Status})");
    }

    [ConsoleSample(Name = "List standard bank accounts")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListStandardBankAccountsAsync(CancellationToken cancellationToken)
    {
        var bankAccounts = await context.Client.BankAccounts.ListAsync(
            BankAccountViews.StandardBankAccounts,
            cancellationToken);

        SampleOutput.WriteHeader($"Standard bank accounts ({bankAccounts.Count})");
        SampleOutput.WriteRows(bankAccounts, account => $"{account.ResourceId,8}  {account.Name}");
    }

    [ConsoleSample(Name = "Get bank account detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetBankAccountDetailAsync(CancellationToken cancellationToken)
    {
        var bankAccounts = await context.Client.BankAccounts.ListAsync(cancellationToken: cancellationToken);
        if (bankAccounts.Count == 0)
        {
            SampleOutput.WriteHeader("Bank account detail");
            SampleOutput.WriteField("Result", "No bank accounts returned.");
            return;
        }

        var bankAccountId = bankAccounts[0].ResourceId;
        var detail = await context.Client.BankAccounts.GetBankAccountAsync(bankAccountId, cancellationToken);

        SampleOutput.WriteHeader("Bank account detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Name", detail.Name);
        SampleOutput.WriteField("Type", detail.Type?.ToString());
        SampleOutput.WriteField("Status", detail.Status?.ToString());
        SampleOutput.WriteField("Currency", detail.Currency?.ToString());
        SampleOutput.WriteField("Current balance", detail.CurrentBalance);
    }

    [ConsoleSample(Name = "Create standard bank account and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateStandardBankAccountAndDeleteAsync(CancellationToken cancellationToken)
    {
        var name = $"SDK console bank account {DateTime.UtcNow:yyyyMMddHHmmss}";
        var created = await context.Client.BankAccounts.CreateStandardBankAccountAsync(
            CreateStandardBankAccountRequest.Create(name, openingBalance: 0m),
            cancellationToken);

        SampleOutput.WriteHeader("Created standard bank account");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Name", created.Name);

        await context.Client.BankAccounts.DeleteBankAccountAsync(created.ResourceId, cancellationToken);
        SampleOutput.WriteField("Deleted", created.ResourceId);
    }

    [ConsoleSample(Name = "Create credit card account and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateCreditCardBankAccountAndDeleteAsync(CancellationToken cancellationToken)
    {
        var name = $"SDK console card {DateTime.UtcNow:yyyyMMddHHmmss}";
        var created = await context.Client.BankAccounts.CreateCreditCardBankAccountAsync(
            CreateCreditCardBankAccountRequest.Create(name, openingBalance: 0m, accountNumber: "1234"),
            cancellationToken);

        SampleOutput.WriteHeader("Created credit card account");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Type", created.Type?.ToString());

        await context.Client.BankAccounts.DeleteBankAccountAsync(created.ResourceId, cancellationToken);
        SampleOutput.WriteField("Deleted", created.ResourceId);
    }

    [ConsoleSample(Name = "Update bank account and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateBankAccountAndDeleteAsync(CancellationToken cancellationToken)
    {
        var name = $"SDK console bank account {DateTime.UtcNow:yyyyMMddHHmmss}";
        var created = await context.Client.BankAccounts.CreateStandardBankAccountAsync(
            CreateStandardBankAccountRequest.Create(name, openingBalance: 0m),
            cancellationToken);

        var updated = await context.Client.BankAccounts.UpdateBankAccountAsync(
            created.ResourceId,
            UpdateBankAccountRequest.Create(name: $"{name} (updated)"),
            cancellationToken);

        SampleOutput.WriteHeader("Updated bank account");
        SampleOutput.WriteField("Id", updated.ResourceId);
        SampleOutput.WriteField("Name", updated.Name);

        await context.Client.BankAccounts.DeleteBankAccountAsync(created.ResourceId, cancellationToken);
        SampleOutput.WriteField("Deleted", created.ResourceId);
    }

    [ConsoleSample(Name = "Create e-commerce account and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateEcommerceBankAccountAndDeleteAsync(CancellationToken cancellationToken)
    {
        var name = $"SDK console ecommerce {DateTime.UtcNow:yyyyMMddHHmmss}";
        var created = await context.Client.BankAccounts.CreateEcommerceBankAccountAsync(
            CreateEcommerceBankAccountRequest.Create(name, openingBalance: 0m),
            cancellationToken);

        SampleOutput.WriteHeader("Created e-commerce bank account");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Type", created.Type?.ToString());

        await context.Client.BankAccounts.DeleteBankAccountAsync(created.ResourceId, cancellationToken);
        SampleOutput.WriteField("Deleted", created.ResourceId);
    }
}
