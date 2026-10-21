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
        var probeContext = await TryBuildWriteProbeContextAsync(
            BankTransactionExplanationWriteProbeVariant.CreateCategory,
            cancellationToken);
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
        var probeContext = await TryBuildWriteProbeContextAsync(variant, cancellationToken);
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
            case BankTransactionExplanationWriteProbeVariant.CreateInvoiceReceipt
                when probeContext.InvoiceId <= 0:
                Console.WriteLine("  Skipped: no invoice available for invoice receipt probe.");
                return false;
            case BankTransactionExplanationWriteProbeVariant.CreateBillPayment
                when probeContext.BillId <= 0:
                Console.WriteLine("  Skipped: no bill available for bill payment probe.");
                return false;
            case BankTransactionExplanationWriteProbeVariant.CreateStock
                when probeContext.StockItemId <= 0:
                Console.WriteLine("  Skipped: no stock item available for stock probe.");
                return false;
            case BankTransactionExplanationWriteProbeVariant.CreateSmartUserPayment
                or BankTransactionExplanationWriteProbeVariant.CreateCategorisedUserPayment
                when probeContext.UserId <= 0:
                Console.WriteLine("  Skipped: no user available for user payment probe.");
                return false;
            case BankTransactionExplanationWriteProbeVariant.CreateOpeningBalance
                when probeContext.ContactId <= 0:
                Console.WriteLine("  Skipped: no contact available for opening balance probe.");
                return false;
            case BankTransactionExplanationWriteProbeVariant.CreatePaymentOrRefund
                when probeContext.ProjectId <= 0:
                Console.WriteLine("  Skipped: no project available for payment or refund probe.");
                return false;
            default:
                return true;
        }
    }

    private async Task<BankTransactionExplanationWriteProbeContext?> TryBuildWriteProbeContextAsync(
        BankTransactionExplanationWriteProbeVariant variant,
        CancellationToken cancellationToken)
    {
        var bankAccount = await context.Data.GetFirstBankAccountAsync(cancellationToken);
        var bankTransactionId = await UploadProbeBankTransactionAsync(bankAccount.ResourceId, cancellationToken);
        if (bankTransactionId is null)
        {
            Console.WriteLine("  Could not create a probe bank transaction.");
            return null;
        }

        long projectId = 0;
        long invoiceId = 0;
        long billId = 0;
        long userId = 0;
        long transferBankAccountId = 0;
        long stockItemId = 0;
        long contactId = 0;
        long disposedAssetId = 0;
        var categoryNominalCode = "285";
        var propertyUri = string.Empty;

        if (VariantUsesCategoryNominalCode(variant))
        {
            categoryNominalCode = await context.Data.GetFirstAdminExpensesCategoryNominalCodeAsync(cancellationToken);
        }

        switch (variant)
        {
            case BankTransactionExplanationWriteProbeVariant.CreatePaymentOrRefund:
            {
                var project = await context.Data.GetFirstProjectAsync(cancellationToken);
                projectId = project.ResourceId;
                break;
            }

            case BankTransactionExplanationWriteProbeVariant.CreateInvoiceReceipt:
            {
                var invoice = await context.Data.GetFirstInvoiceAsync(cancellationToken);
                invoiceId = invoice.ResourceId;
                break;
            }

            case BankTransactionExplanationWriteProbeVariant.CreateBillPayment:
            {
                var bill = await context.Data.GetFirstBillAsync(cancellationToken);
                billId = bill.ResourceId;
                break;
            }

            case BankTransactionExplanationWriteProbeVariant.CreateSmartUserPayment
                or BankTransactionExplanationWriteProbeVariant.CreateCategorisedUserPayment:
            {
                var user = await context.Data.GetFirstUserAsync(cancellationToken);
                userId = user.ResourceId;
                break;
            }

            case BankTransactionExplanationWriteProbeVariant.CreateTransfer:
            {
                var bankAccounts = await context.Client.BankAccounts.ListAsync(cancellationToken: cancellationToken);
                transferBankAccountId = bankAccounts.Count >= 2 ? bankAccounts[1].ResourceId : 0L;
                break;
            }

            case BankTransactionExplanationWriteProbeVariant.CreateStock:
            {
                var stockItem = await context.Data.GetFirstStockItemAsync(cancellationToken);
                stockItemId = stockItem.ResourceId;
                break;
            }

            case BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetDisposal:
            {
                var capitalAssets = await context.Client.CapitalAssets.ListAsync(cancellationToken: cancellationToken);
                disposedAssetId = capitalAssets.Count > 0 ? capitalAssets[0].ResourceId : 0L;
                break;
            }

            case BankTransactionExplanationWriteProbeVariant.CreateLandlordProperty:
                propertyUri = await TryResolvePropertyUriForLandlordProbeAsync(cancellationToken);
                break;

            case BankTransactionExplanationWriteProbeVariant.CreateOpeningBalance:
            {
                var contact = await context.Data.GetFirstContactAsync(cancellationToken);
                contactId = contact.ResourceId;
                break;
            }
        }

        return new BankTransactionExplanationWriteProbeContext
        {
            BankAccountId = bankAccount.ResourceId,
            BankTransactionId = bankTransactionId.Value,
            ProjectId = projectId,
            InvoiceId = invoiceId,
            BillId = billId,
            UserId = userId,
            TransferBankAccountId = transferBankAccountId,
            StockItemId = stockItemId,
            ContactId = contactId,
            DisposedAssetId = disposedAssetId,
            CategoryNominalCode = categoryNominalCode,
            PropertyUri = propertyUri,
        };
    }

    private static bool VariantUsesCategoryNominalCode(BankTransactionExplanationWriteProbeVariant variant) =>
        variant is BankTransactionExplanationWriteProbeVariant.CreateCategory
            or BankTransactionExplanationWriteProbeVariant.CreatePaymentOrRefund
            or BankTransactionExplanationWriteProbeVariant.CreateCategorisedUserPayment
            or BankTransactionExplanationWriteProbeVariant.CreateStock
            or BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchase
            or BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchaseWithDepreciationProfile
            or BankTransactionExplanationWriteProbeVariant.CreateLandlordProperty
            or BankTransactionExplanationWriteProbeVariant.CreateOpeningBalance;

    private async Task<string> TryResolvePropertyUriForLandlordProbeAsync(CancellationToken cancellationToken)
    {
        try
        {
            var company = await context.Client.Company.GetCompanyAsync(cancellationToken);
            if (company.Type != CompanyType.UkUnincorporatedLandlord)
            {
                return string.Empty;
            }

            var properties = await context.Client.Properties.ListAsync(cancellationToken: cancellationToken);
            if (properties.Count == 0)
            {
                return string.Empty;
            }

            return context.Client.Urls.Property(properties[0].ResourceId).Uri;
        }
        catch (FreeAgentApiException)
        {
            return string.Empty;
        }
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

        if (created is not null)
        {
            return created.ResourceId;
        }

        foreach (var orphan in uploaded.Items.Where(
                     transaction =>
                         string.Equals(transaction.TransactionId, fitId, StringComparison.Ordinal)
                         || string.Equals(transaction.Description, fitId, StringComparison.Ordinal)))
        {
            await TryDeleteProbeBankTransactionAsync(orphan.ResourceId, cancellationToken);
        }

        return null;
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
