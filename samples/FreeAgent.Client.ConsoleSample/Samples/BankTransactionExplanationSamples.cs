using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Samples.Shared.Probes;

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
    private Task CreateCategoryExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateCategory, cancellationToken);

    [ConsoleSample(Name = "Create payment or refund explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreatePaymentOrRefundExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreatePaymentOrRefund, cancellationToken);

    [ConsoleSample(Name = "Create invoice receipt explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateInvoiceReceiptExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateInvoiceReceipt, cancellationToken);

    [ConsoleSample(Name = "Create bill payment explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateBillPaymentExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateBillPayment, cancellationToken);

    [ConsoleSample(Name = "Create smart user payment explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateSmartUserPaymentExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateSmartUserPayment, cancellationToken);

    [ConsoleSample(Name = "Create categorised user payment explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateCategorisedUserPaymentExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateCategorisedUserPayment, cancellationToken);

    [ConsoleSample(Name = "Create transfer explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateTransferExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateTransfer, cancellationToken);

    [ConsoleSample(Name = "Create stock explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateStockExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateStock, cancellationToken);

    [ConsoleSample(Name = "Create capital asset purchase explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateCapitalAssetPurchaseExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchase, cancellationToken);

    [ConsoleSample(Name = "Create capital asset purchase with depreciation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateCapitalAssetPurchaseWithDepreciationExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(
            BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchaseWithDepreciationProfile,
            cancellationToken);

    [ConsoleSample(Name = "Create capital asset disposal explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateCapitalAssetDisposalExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetDisposal, cancellationToken);

    [ConsoleSample(Name = "Create landlord property explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateLandlordPropertyExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateLandlordProperty, cancellationToken);

    [ConsoleSample(Name = "Create opening balance explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateOpeningBalanceExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateOpeningBalance, cancellationToken);

    [ConsoleSample(Name = "Update bank transaction explanation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateBankTransactionExplanationAndDeleteAsync(CancellationToken cancellationToken)
    {
        var probeContext = await TryBuildWriteProbeContextAsync(cancellationToken);
        if (probeContext is null)
        {
            return;
        }

        var created = await BankTransactionExplanationWriteProbeSupport.ExecuteCreateAsync(
            context.Client,
            BankTransactionExplanationWriteProbeVariant.CreateCategory,
            probeContext,
            cancellationToken);

        var updatedDescription = $"sdk-console-updated-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var updated = await context.Client.BankTransactionExplanations.UpdateBankTransactionExplanationAsync(
            created.ResourceId,
            new UpdateBankTransactionExplanationRequest { Description = updatedDescription },
            cancellationToken);

        SampleOutput.WriteHeader("Updated bank transaction explanation");
        SampleOutput.WriteField("Id", updated.ResourceId);
        SampleOutput.WriteField("Description", updated.Description);

        await context.Client.BankTransactionExplanations.DeleteBankTransactionExplanationAsync(
            created.ResourceId,
            cancellationToken);

        await TryDeleteProbeBankTransactionAsync(probeContext.BankTransactionId, cancellationToken);
        Console.WriteLine($"  Deleted explanation {created.ResourceId}.");
    }

    private async Task RunCreateVariantProbeAsync(
        BankTransactionExplanationWriteProbeVariant variant,
        CancellationToken cancellationToken)
    {
        var probeContext = await TryBuildWriteProbeContextAsync(cancellationToken);
        if (probeContext is null)
        {
            return;
        }

        if (!ValidateVariantPrerequisites(variant, probeContext))
        {
            return;
        }

        var created = await BankTransactionExplanationWriteProbeSupport.ExecuteCreateAsync(
            context.Client,
            variant,
            probeContext,
            cancellationToken);

        SampleOutput.WriteHeader($"Created explanation ({variant})");
        SampleOutput.WriteField("Call", BankTransactionExplanationWriteProbeSupport.GetCallUnderTest(variant));
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Type", created.Type);
        SampleOutput.WriteField("Gross value", created.GrossValue);

        await context.Client.BankTransactionExplanations.DeleteBankTransactionExplanationAsync(
            created.ResourceId,
            cancellationToken);

        await TryDeleteProbeBankTransactionAsync(probeContext.BankTransactionId, cancellationToken);
        Console.WriteLine($"  Deleted explanation {created.ResourceId}.");
    }

    private static bool ValidateVariantPrerequisites(
        BankTransactionExplanationWriteProbeVariant variant,
        BankTransactionExplanationWriteProbeContext probeContext)
    {
        switch (variant)
        {
            case BankTransactionExplanationWriteProbeVariant.CreateTransfer
                when probeContext.TransferBankAccountId <= 0:
                Console.WriteLine("  Skipped: need at least two bank accounts for a transfer probe.");
                return false;
            case BankTransactionExplanationWriteProbeVariant.CreateLandlordProperty
                when string.IsNullOrWhiteSpace(probeContext.PropertyUri):
                Console.WriteLine("  Skipped: no property URI available (landlord company type required).");
                return false;
            case BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetDisposal
                when probeContext.DisposedAssetId <= 0:
                Console.WriteLine("  Skipped: no capital asset available for disposal probe.");
                return false;
            default:
                return true;
        }
    }

    private async Task<BankTransactionExplanationWriteProbeContext?> TryBuildWriteProbeContextAsync(
        CancellationToken cancellationToken)
    {
        var bankAccount = await context.Data.GetFirstBankAccountAsync(cancellationToken);
        var bankTransactionId = await UploadProbeBankTransactionAsync(bankAccount.ResourceId, cancellationToken);
        if (bankTransactionId is null)
        {
            Console.WriteLine("  Could not create a probe bank transaction.");
            return null;
        }

        var project = await context.Data.GetFirstProjectAsync(cancellationToken);
        var invoice = await context.Data.GetFirstInvoiceAsync(cancellationToken);
        var bill = await context.Data.GetFirstBillAsync(cancellationToken);
        var user = await context.Data.GetFirstUserAsync(cancellationToken);
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var stockItem = await context.Data.GetFirstStockItemAsync(cancellationToken);
        var categoryNominalCode = await context.Data.GetFirstAdminExpensesCategoryNominalCodeAsync(cancellationToken);

        var bankAccounts = await context.Client.BankAccounts.ListAsync(cancellationToken: cancellationToken);
        var transferBankAccountId = bankAccounts.Count >= 2
            ? bankAccounts[1].ResourceId
            : 0L;

        var capitalAssets = await context.Client.CapitalAssets.ListAsync(cancellationToken: cancellationToken);
        var disposedAssetId = capitalAssets.Count > 0 ? capitalAssets[0].ResourceId : 0L;

        string propertyUri = string.Empty;
        var properties = await context.Client.Properties.ListAsync(cancellationToken: cancellationToken);
        if (properties.Count > 0)
        {
            propertyUri = context.Client.Urls.Property(properties[0].ResourceId).Uri;
        }

        return new BankTransactionExplanationWriteProbeContext
        {
            BankAccountId = bankAccount.ResourceId,
            BankTransactionId = bankTransactionId.Value,
            ProjectId = project.ResourceId,
            InvoiceId = invoice.ResourceId,
            BillId = bill.ResourceId,
            UserId = user.ResourceId,
            TransferBankAccountId = transferBankAccountId,
            StockItemId = stockItem.ResourceId,
            ContactId = contact.ResourceId,
            DisposedAssetId = disposedAssetId,
            CategoryNominalCode = categoryNominalCode,
            PropertyUri = propertyUri
        };
    }

    private async Task<long?> UploadProbeBankTransactionAsync(long bankAccountId, CancellationToken cancellationToken)
    {
        var fitId = $"sdk-console-bte-{DateTime.UtcNow:yyyyMMddHHmmss}";
        await context.Client.BankTransactions.UploadBankStatementAsync(
            new UploadBankStatementRequest(
            [
                new BankStatementTransaction
                {
                    DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                    Amount = -0.01m,
                    Description = fitId,
                    FitId = fitId,
                    TransactionType = BankStatementTransactionType.Other
                }
            ]),
            bankAccountId: bankAccountId,
            cancellationToken: cancellationToken);

        var uploaded = await context.Client.BankTransactions.ListAsync(
            bankAccountId: bankAccountId,
            lastUploaded: true,
            cancellationToken: cancellationToken);

        var created = uploaded.Items.FirstOrDefault(transaction => transaction.TransactionId == fitId)
            ?? uploaded.Items.FirstOrDefault(transaction => transaction.Description == fitId);

        return created?.ResourceId;
    }

    private async Task TryDeleteProbeBankTransactionAsync(long bankTransactionId, CancellationToken cancellationToken)
    {
        try
        {
            await context.Client.BankTransactions.DeleteBankTransactionAsync(bankTransactionId, cancellationToken);
        }
        catch (FreeAgentApiException)
        {
            // Explanation delete may have removed the transaction; ignore cleanup failures.
        }
    }

    private static string FormatExplanationRow(BankTransactionExplanation explanation) =>
        $"{explanation.ResourceId,8}  gross={explanation.GrossValue,10}  type={explanation.Type?.ToString() ?? "—",-20}  dated={explanation.DatedOn?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "—"}";
}
