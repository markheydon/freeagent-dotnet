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

    [ConsoleSample(Name = "Create payment or refund explanation and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreatePaymentOrRefundExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreatePaymentOrRefund, cancellationToken);

    [ConsoleSample(Name = "Create invoice receipt explanation and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateInvoiceReceiptExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateInvoiceReceipt, cancellationToken);

    [ConsoleSample(Name = "Create bill payment explanation and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateBillPaymentExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateBillPayment, cancellationToken);

    [ConsoleSample(Name = "Create smart user payment explanation and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateSmartUserPaymentExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateSmartUserPayment, cancellationToken);

    [ConsoleSample(Name = "Create categorised user payment explanation and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateCategorisedUserPaymentExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateCategorisedUserPayment, cancellationToken);

    [ConsoleSample(Name = "Create transfer explanation and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateTransferExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateTransfer, cancellationToken);

    [ConsoleSample(Name = "Create stock explanation and delete (adjusts quantity)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateStockExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateStock, cancellationToken);

    [ConsoleSample(Name = "Create capital asset purchase explanation and delete (may leave asset)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateCapitalAssetPurchaseExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchase, cancellationToken);

    [ConsoleSample(Name = "Create capital asset purchase with depreciation and delete", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task CreateCapitalAssetPurchaseWithDepreciationExplanationAndDeleteAsync(CancellationToken cancellationToken) =>
        RunCreateVariantProbeAsync(
            BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchaseWithDepreciationProfile,
            cancellationToken);

    [ConsoleSample(Name = "Create capital asset disposal explanation and delete (probe asset)", MutatesData = true, ExcludeFromRunAll = true)]
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

    [ConsoleSample(Name = "Update bank transaction explanation and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateBankTransactionExplanationAndDeleteAsync(CancellationToken cancellationToken)
    {
        var (probeContext, probeSkipReason) = await TryBuildWriteProbeContextAsync(
            BankTransactionExplanationWriteProbeVariant.CreateCategory,
            cancellationToken);
        if (probeContext is null)
        {
            SampleContext.Skip(probeSkipReason ?? "could not create a probe bank transaction for explanation update");
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
        if (variant == BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetDisposal)
        {
            await RunCapitalAssetDisposalProbeAsync(cancellationToken);
            return;
        }

        var (probeContext, probeSkipReason) = await TryBuildWriteProbeContextAsync(variant, cancellationToken);
        if (probeContext is null)
        {
            SampleContext.Skip(probeSkipReason ?? "could not create a probe bank transaction for explanation create");
        }

        EnsureVariantPrerequisitesOrSkip(variant, probeContext);

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

    private static void EnsureVariantPrerequisitesOrSkip(
        BankTransactionExplanationWriteProbeVariant variant,
        BankTransactionExplanationWriteProbeContext probeContext)
    {
        if (BankTransactionExplanationWriteProbeSupport.TryValidateCreatePrerequisites(
                variant,
                probeContext,
                out var skipReason))
        {
            return;
        }

        SampleContext.Skip(skipReason ?? "prerequisites for explanation probe are not met");
    }

    private async Task RunCapitalAssetDisposalProbeAsync(CancellationToken cancellationToken)
    {
        var (purchaseContext, purchaseSkipReason) = await TryBuildWriteProbeContextAsync(
            BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchase,
            cancellationToken);
        if (purchaseContext is null)
        {
            SampleContext.Skip(
                purchaseSkipReason ?? "could not create a probe bank transaction for capital asset disposal");
        }

        var purchase = await BankTransactionExplanationWriteProbeSupport.ExecuteCreateAsync(
            context.Client,
            BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetPurchase,
            purchaseContext,
            cancellationToken);

        if (purchase.CapitalAssetId is not long capitalAssetId)
        {
            Console.WriteLine("  Skipped: capital asset purchase did not return a capital asset id.");
            await context.Client.BankTransactionExplanations.DeleteBankTransactionExplanationAsync(
                purchase.ResourceId,
                cancellationToken);
            await TryDeleteProbeBankTransactionAsync(purchaseContext.BankTransactionId, cancellationToken);
            return;
        }

        var disposedAssetId = capitalAssetId;
        var (disposalBankTransactionId, disposalUploadSkipReason) = await UploadProbeBankTransactionAsync(
            purchaseContext.BankAccountId,
            cancellationToken,
            BankTransactionExplanationWriteProbeSupport.ProbeDisposalStatementAmount);
        if (disposalBankTransactionId is null)
        {
            await context.Client.BankTransactionExplanations.DeleteBankTransactionExplanationAsync(
                purchase.ResourceId,
                cancellationToken);
            await TryDeleteProbeBankTransactionAsync(purchaseContext.BankTransactionId, cancellationToken);
            SampleContext.Skip(
                disposalUploadSkipReason ?? "could not upload or resolve a probe bank transaction for disposal");
        }

        var disposalContext = new BankTransactionExplanationWriteProbeContext
        {
            BankAccountId = purchaseContext.BankAccountId,
            BankTransactionId = disposalBankTransactionId.Value,
            DisposedAssetId = disposedAssetId,
            CategoryNominalCode = purchaseContext.CategoryNominalCode,
        };

        try
        {
            var disposal = await BankTransactionExplanationWriteProbeSupport.ExecuteCreateAsync(
                context.Client,
                BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetDisposal,
                disposalContext,
                cancellationToken);

            SampleOutput.WriteHeader("Created explanation (CreateCapitalAssetDisposal)");
            SampleOutput.WriteField(
                "Call",
                BankTransactionExplanationWriteProbeSupport.GetCallUnderTest(
                    BankTransactionExplanationWriteProbeVariant.CreateCapitalAssetDisposal));
            SampleOutput.WriteField("Id", disposal.ResourceId);
            SampleOutput.WriteField("Type", disposal.Type);
            SampleOutput.WriteField("Gross value", disposal.GrossValue);
            SampleOutput.WriteField("Disposed asset ID", disposedAssetId);

            await context.Client.BankTransactionExplanations.DeleteBankTransactionExplanationAsync(
                disposal.ResourceId,
                cancellationToken);
            Console.WriteLine($"  Deleted explanation {disposal.ResourceId}.");
        }
        finally
        {
            await context.Client.BankTransactionExplanations.DeleteBankTransactionExplanationAsync(
                purchase.ResourceId,
                cancellationToken);
            await TryDeleteProbeBankTransactionAsync(disposalBankTransactionId.Value, cancellationToken);
            await TryDeleteProbeBankTransactionAsync(purchaseContext.BankTransactionId, cancellationToken);
            Console.WriteLine($"  Deleted probe purchase explanation {purchase.ResourceId}.");
        }
    }

    private async Task<(BankTransactionExplanationWriteProbeContext? Context, string? SkipReason)> TryBuildWriteProbeContextAsync(
        BankTransactionExplanationWriteProbeVariant variant,
        CancellationToken cancellationToken)
    {
        var bankAccount = await context.Data.GetFirstBankAccountAsync(cancellationToken);
        var uploadAmount = BankTransactionExplanationWriteProbeSupport.GetProbeStatementAmount(variant);
        var (bankTransactionId, uploadSkipReason) = await UploadProbeBankTransactionAsync(
            bankAccount.ResourceId,
            cancellationToken,
            uploadAmount);
        if (bankTransactionId is null)
        {
            return (null, uploadSkipReason ?? "could not upload or resolve a probe bank transaction");
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
        long propertyId = 0;

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
                    var invoice = await context.Data.GetFirstReceiptableInvoiceAsync(cancellationToken);
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

            case BankTransactionExplanationWriteProbeVariant.CreateLandlordProperty:
                propertyId = await TryResolvePropertyIdForLandlordProbeAsync(cancellationToken);
                break;

            case BankTransactionExplanationWriteProbeVariant.CreateOpeningBalance:
                {
                    var contact = await context.Data.GetFirstContactAsync(cancellationToken);
                    contactId = contact.ResourceId;
                    break;
                }
        }

        return (new BankTransactionExplanationWriteProbeContext
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
            PropertyId = propertyId,
        }, null);
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

    private async Task<long> TryResolvePropertyIdForLandlordProbeAsync(CancellationToken cancellationToken)
    {
        try
        {
            var company = await context.Client.Company.GetCompanyAsync(cancellationToken);
            if (company.Type != CompanyType.UkUnincorporatedLandlord)
            {
                return 0;
            }

            var properties = await context.Client.Properties.ListAsync(cancellationToken: cancellationToken);
            if (properties.Count == 0)
            {
                return 0;
            }

            return properties[0].ResourceId;
        }
        catch (FreeAgentApiException)
        {
            return 0;
        }
    }

    private async Task<(long? TransactionId, string? SkipReason)> UploadProbeBankTransactionAsync(
        long bankAccountId,
        CancellationToken cancellationToken,
        decimal amount = BankTransactionExplanationWriteProbeSupport.ProbeMoneyOutStatementAmount)
    {
        var fitId = ProbeBankTransactionLifecycleSupport.CreateProbeFitId("sdk-console-bte");
        await context.Client.BankTransactions.UploadBankStatementAsync(
            new UploadBankStatementRequest(
            [
                new BankStatementTransaction
                {
                    DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                    Amount = amount,
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

        var createdId = BankTransactionExplanationWriteProbeSupport.TryResolveUploadedBankTransactionId(
            uploaded.Items,
            fitId);
        if (createdId is null)
        {
            foreach (var orphan in BankTransactionExplanationWriteProbeSupport.FindUploadOrphans(uploaded.Items, fitId))
            {
                await TryDeleteProbeBankTransactionAsync(orphan.ResourceId, cancellationToken);
            }

            return (null, "bank statement upload did not return a matching transaction id");
        }

        var detail = await context.Client.BankTransactions.GetBankTransactionAsync(createdId.Value, cancellationToken);
        if (ProbeBankTransactionLifecycleSupport.IsUnexplainedForProbe(detail, amount))
        {
            return (createdId, null);
        }

        await TryDeleteProbeBankTransactionAsync(createdId.Value, cancellationToken);
        return (
            null,
            $"uploaded bank transaction {createdId.Value} is not unexplained for probe amount {amount.ToString(CultureInfo.InvariantCulture)} (unexplained {detail.UnexplainedAmount?.ToString(CultureInfo.InvariantCulture) ?? "null"})");
    }

    private Task TryDeleteProbeBankTransactionAsync(long bankTransactionId, CancellationToken cancellationToken) =>
        ProbeBankTransactionLifecycleSupport.DeleteExplanationsAndBankTransactionAsync(
            context.Client,
            bankTransactionId,
            cancellationToken);

    private static string FormatExplanationRow(BankTransactionExplanation explanation) =>
        $"{explanation.ResourceId,8}  gross={explanation.GrossValue,10}  type={explanation.Type?.ToString() ?? "—",-20}  dated={explanation.DatedOn?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "—"}";
}
