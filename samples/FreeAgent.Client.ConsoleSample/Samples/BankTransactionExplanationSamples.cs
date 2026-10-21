using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FreeAgent.Client;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Bank transaction explanation endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Bank transaction explanations")]
internal sealed class BankTransactionExplanationSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List bank transaction explanations")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListBankTransactionExplanationsAsync(CancellationToken cancellationToken)
    {
        var bankAccount = await context.Data.GetFirstBankAccountAsync(cancellationToken);
        var explanations = await context.Client.BankTransactionExplanations.ListAsync(
            bankAccountId: bankAccount.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader(
            $"Bank transaction explanations for account {bankAccount.ResourceId} ({explanations.Count})");
        SampleOutput.WriteRows(explanations, FormatExplanationRow);
    }

    [ConsoleSample(Name = "Get bank transaction explanation by id")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetBankTransactionExplanationByIdAsync(CancellationToken cancellationToken)
    {
        var explanation = await context.Data.GetFirstBankTransactionExplanationAsync(cancellationToken);
        var detail = await context.Client.BankTransactionExplanations.GetBankTransactionExplanationAsync(
            explanation.ResourceId,
            new BankTransactionExplanationGetOptions
            {
                IncludeCategory = true,
                IncludeProject = true,
                IncludeBankAccount = true,
                IncludePaidInvoice = true,
                IncludePaidBill = true
            },
            cancellationToken);

        SampleOutput.WriteHeader("Bank transaction explanation detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Type", detail.Type);
        SampleOutput.WriteField("Dated on", detail.DatedOn);
        SampleOutput.WriteField("Gross value", detail.GrossValue);
        SampleOutput.WriteField("Description", detail.Description);
        SampleOutput.WriteField("Category", detail.CategoryNominalCode);
        SampleOutput.WriteField("Bank transaction ID", detail.BankTransactionId);
        SampleOutput.WriteField("Bank account ID", detail.BankAccountId);
    }

    [ConsoleSample(Name = "Create category explanation and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateCategoryExplanationAndDeleteAsync(CancellationToken cancellationToken)
    {
        var bankAccount = await context.Data.GetFirstBankAccountAsync(cancellationToken);
        var categoryNominalCode = await context.Data.GetFirstAdminExpensesCategoryNominalCodeAsync(cancellationToken);
        var datedOn = DateOnly.FromDateTime(DateTime.UtcNow);
        var description = $"sdk-console-explanation-{DateTime.UtcNow:yyyyMMddHHmmss}";

        var created = await context.Client.BankTransactionExplanations.CreateCategoryBankTransactionExplanationAsync(
            CreateCategoryBankTransactionExplanationRequest.WithNewTransaction(
                bankAccountId: bankAccount.ResourceId,
                categoryNominalCode: categoryNominalCode,
                datedOn: datedOn,
                grossValue: -0.01m,
                description: description),
            cancellationToken);

        SampleOutput.WriteHeader("Created bank transaction explanation");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Gross value", created.GrossValue);
        SampleOutput.WriteField("Category", created.CategoryNominalCode);
        SampleOutput.WriteField("Bank transaction ID", created.BankTransactionId);

        await context.Client.BankTransactionExplanations.DeleteBankTransactionExplanationAsync(
            created.ResourceId,
            cancellationToken);

        Console.WriteLine();
        Console.WriteLine($"  Deleted explanation {created.ResourceId}.");
    }

    private static string FormatExplanationRow(BankTransactionExplanation explanation) =>
        $"{explanation.ResourceId,8}  gross={explanation.GrossValue,10}  type={explanation.Type?.ToString() ?? "—",-20}  dated={explanation.DatedOn?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "—"}";
}
