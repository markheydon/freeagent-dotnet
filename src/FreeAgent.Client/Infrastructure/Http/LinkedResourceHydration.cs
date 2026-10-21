using FreeAgent.Client.Services.Categories;

namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Optional hydration of linked resources after single-resource GET responses.
/// </summary>
internal static class LinkedResourceHydration
{
    public static async System.Threading.Tasks.Task HydrateContactAsync(
        Project project,
        IFreeAgentRequestClient requestClient,
        bool includeContact,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeContact || project.Contact is not null || project.ContactId is not long contactId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ContactResponse>($"contacts/{contactId}", cancellationToken);

        if (response.Contact is null)
        {
            throw new FreeAgentApiException("Contact data missing from API response");
        }

        project.AttachContact(response.Contact);
    }

    public static async System.Threading.Tasks.Task HydrateBillingContactAsync(
        Invoice invoice,
        IFreeAgentRequestClient requestClient,
        bool includeContact,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeContact || invoice.Contact is not null || invoice.ContactId is not long contactId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ContactResponse>($"contacts/{contactId}", cancellationToken);

        if (response.Contact is null)
        {
            throw new FreeAgentApiException("Contact data missing from API response");
        }

        invoice.AttachContact(response.Contact);
    }

    public static async System.Threading.Tasks.Task HydrateProjectAsync(
        Invoice invoice,
        IFreeAgentRequestClient requestClient,
        bool includeProject,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeProject || invoice.Project is not null || invoice.ProjectId is not long projectId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        invoice.AttachProject(response.Project);
    }

    public static async System.Threading.Tasks.Task HydrateBillingContactAsync(
        Estimate estimate,
        IFreeAgentRequestClient requestClient,
        bool includeContact,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(estimate);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeContact || estimate.Contact is not null || estimate.ContactId is not long contactId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ContactResponse>($"contacts/{contactId}", cancellationToken);

        if (response.Contact is null)
        {
            throw new FreeAgentApiException("Contact data missing from API response");
        }

        estimate.AttachContact(response.Contact);
    }

    public static async System.Threading.Tasks.Task HydrateProjectAsync(
        Estimate estimate,
        IFreeAgentRequestClient requestClient,
        bool includeProject,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(estimate);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeProject || estimate.Project is not null || estimate.ProjectId is not long projectId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        estimate.AttachProject(response.Project);
    }

    public static async System.Threading.Tasks.Task HydrateBillingContactAsync(
        Bill bill,
        IFreeAgentRequestClient requestClient,
        bool includeContact,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(bill);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeContact || bill.Contact is not null || bill.ContactId is not long contactId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ContactResponse>($"contacts/{contactId}", cancellationToken);

        if (response.Contact is null)
        {
            throw new FreeAgentApiException("Contact data missing from API response");
        }

        bill.AttachContact(response.Contact);
    }

    public static async System.Threading.Tasks.Task HydrateProjectAsync(
        Bill bill,
        IFreeAgentRequestClient requestClient,
        bool includeProject,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(bill);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeProject || bill.Project is not null || bill.ProjectId is not long projectId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        bill.AttachProject(response.Project);
    }

    public static async System.Threading.Tasks.Task HydrateBillingContactAsync(
        CreditNote creditNote,
        IFreeAgentRequestClient requestClient,
        bool includeContact,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(creditNote);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeContact || creditNote.Contact is not null || creditNote.ContactId is not long contactId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ContactResponse>($"contacts/{contactId}", cancellationToken);

        if (response.Contact is null)
        {
            throw new FreeAgentApiException("Contact data missing from API response");
        }

        creditNote.AttachContact(response.Contact);
    }

    public static async System.Threading.Tasks.Task HydrateProjectAsync(
        CreditNote creditNote,
        IFreeAgentRequestClient requestClient,
        bool includeProject,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(creditNote);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeProject || creditNote.Project is not null || creditNote.ProjectId is not long projectId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        creditNote.AttachProject(response.Project);
    }

    public static async System.Threading.Tasks.Task HydrateBillingContactAsync(
        RecurringInvoice recurringInvoice,
        IFreeAgentRequestClient requestClient,
        bool includeContact,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(recurringInvoice);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeContact || recurringInvoice.Contact is not null || recurringInvoice.ContactId is not long contactId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ContactResponse>($"contacts/{contactId}", cancellationToken);

        if (response.Contact is null)
        {
            throw new FreeAgentApiException("Contact data missing from API response");
        }

        recurringInvoice.AttachContact(response.Contact);
    }

    public static async System.Threading.Tasks.Task HydrateProjectAsync(
        RecurringInvoice recurringInvoice,
        IFreeAgentRequestClient requestClient,
        bool includeProject,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(recurringInvoice);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeProject || recurringInvoice.Project is not null || recurringInvoice.ProjectId is not long projectId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        recurringInvoice.AttachProject(response.Project);
    }

    public static async System.Threading.Tasks.Task HydrateProjectAsync(
        ProjectTask task,
        IFreeAgentRequestClient requestClient,
        bool includeProject,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeProject || task.Project is not null || task.ProjectId is not long projectId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        task.AttachProject(response.Project);
    }

    public static async System.Threading.Tasks.Task HydrateProjectTaskAsync(
        Timeslip timeslip,
        IFreeAgentRequestClient requestClient,
        bool includeProjectTask,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(timeslip);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeProjectTask || timeslip.ProjectTask is not null || timeslip.ProjectTaskId is not long projectTaskId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ProjectTaskResponse>($"tasks/{projectTaskId}", cancellationToken);

        if (response.ProjectTask is null)
        {
            throw new FreeAgentApiException("Task data missing from API response");
        }

        timeslip.AttachProjectTask(response.ProjectTask);
    }

    public static async System.Threading.Tasks.Task HydrateProjectAsync(
        Timeslip timeslip,
        IFreeAgentRequestClient requestClient,
        bool includeProject,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(timeslip);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeProject || timeslip.Project is not null || timeslip.ProjectId is not long projectId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        timeslip.AttachProject(response.Project);
    }

    public static async System.Threading.Tasks.Task HydrateUserAsync(
        Timeslip timeslip,
        IFreeAgentRequestClient requestClient,
        bool includeUser,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(timeslip);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeUser || timeslip.User is not null || timeslip.UserId is not long userId)
        {
            return;
        }

        var response = await requestClient.GetAsync<UserResponse>($"users/{userId}", cancellationToken);

        if (response.User is null)
        {
            throw new FreeAgentApiException("User data missing from API response");
        }

        timeslip.AttachUser(response.User);
    }

    public static async System.Threading.Tasks.Task HydrateInvoiceAsync(
        CreditNoteReconciliation reconciliation,
        IFreeAgentRequestClient requestClient,
        bool includeInvoice,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(reconciliation);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeInvoice || reconciliation.Invoice is not null || reconciliation.InvoiceId is not long invoiceId)
        {
            return;
        }

        var response = await requestClient.GetAsync<InvoiceResponse>($"invoices/{invoiceId}", cancellationToken);

        if (response.Invoice is null)
        {
            throw new FreeAgentApiException("Invoice data missing from API response");
        }

        reconciliation.AttachInvoice(response.Invoice);
    }

    public static async System.Threading.Tasks.Task HydrateCreditNoteAsync(
        CreditNoteReconciliation reconciliation,
        IFreeAgentRequestClient requestClient,
        bool includeCreditNote,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(reconciliation);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeCreditNote || reconciliation.CreditNote is not null || reconciliation.CreditNoteId is not long creditNoteId)
        {
            return;
        }

        var response = await requestClient.GetAsync<CreditNoteResponse>($"credit_notes/{creditNoteId}", cancellationToken);

        if (response.CreditNote is null)
        {
            throw new FreeAgentApiException("Credit note data missing from API response");
        }

        reconciliation.AttachCreditNote(response.CreditNote);
    }

    public static async System.Threading.Tasks.Task HydrateNoteParentAsync(
        Note note,
        IFreeAgentRequestClient requestClient,
        bool includeParentContact,
        bool includeParentProject,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (includeParentContact
            && note.Contact is null
            && note.TryParseParentContactId(out var contactId))
        {
            var response = await requestClient.GetAsync<ContactResponse>($"contacts/{contactId}", cancellationToken);

            if (response.Contact is null)
            {
                throw new FreeAgentApiException("Contact data missing from API response");
            }

            note.AttachContact(response.Contact);
        }

        if (includeParentProject
            && note.Project is null
            && note.TryParseParentProjectId(out var projectId))
        {
            var response = await requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);

            if (response.Project is null)
            {
                throw new FreeAgentApiException("Project data missing from API response");
            }

            note.AttachProject(response.Project);
        }
    }

    public static async System.Threading.Tasks.Task HydrateInvoiceAsync(
        Estimate estimate,
        IFreeAgentRequestClient requestClient,
        bool includeInvoice,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(estimate);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeInvoice || estimate.Invoice is not null || estimate.InvoiceId is not long invoiceId)
        {
            return;
        }

        var response = await requestClient.GetAsync<InvoiceResponse>($"invoices/{invoiceId}", cancellationToken);

        if (response.Invoice is null)
        {
            throw new FreeAgentApiException("Invoice data missing from API response");
        }

        estimate.AttachInvoice(response.Invoice);
    }

    public static async System.Threading.Tasks.Task HydrateBilledOnInvoiceAsync(
        Timeslip timeslip,
        IFreeAgentRequestClient requestClient,
        bool includeBilledOnInvoice,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(timeslip);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeBilledOnInvoice
            || timeslip.BilledOnInvoice is not null
            || timeslip.BilledOnInvoiceId is not long invoiceId)
        {
            return;
        }

        var response = await requestClient.GetAsync<InvoiceResponse>($"invoices/{invoiceId}", cancellationToken);

        if (response.Invoice is null)
        {
            throw new FreeAgentApiException("Invoice data missing from API response");
        }

        timeslip.AttachBilledOnInvoice(response.Invoice);
    }

    public static async System.Threading.Tasks.Task HydrateRebillToProjectAsync(
        Bill bill,
        IFreeAgentRequestClient requestClient,
        bool includeRebillToProject,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(bill);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeRebillToProject
            || bill.RebillToProject is not null
            || bill.RebillToProjectId is not long projectId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        bill.AttachRebillToProject(response.Project);
    }

    public static async System.Threading.Tasks.Task HydrateRebilledOnInvoiceAsync(
        Bill bill,
        IFreeAgentRequestClient requestClient,
        bool includeRebilledOnInvoice,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(bill);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeRebilledOnInvoice
            || bill.RebilledOnInvoice is not null
            || bill.RebilledOnInvoiceId is not long invoiceId)
        {
            return;
        }

        var response = await requestClient.GetAsync<InvoiceResponse>($"invoices/{invoiceId}", cancellationToken);

        if (response.Invoice is null)
        {
            throw new FreeAgentApiException("Invoice data missing from API response");
        }

        bill.AttachRebilledOnInvoice(response.Invoice);
    }

    public static async System.Threading.Tasks.Task HydrateBankAccountAsync(
        Invoice invoice,
        IFreeAgentRequestClient requestClient,
        bool includeBankAccount,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeBankAccount || invoice.BankAccount is not null || invoice.BankAccountId is not long bankAccountId)
        {
            return;
        }

        var response = await requestClient.GetAsync<BankAccountResponse>($"bank_accounts/{bankAccountId}", cancellationToken);

        if (response.BankAccount is null)
        {
            throw new FreeAgentApiException("Bank account data missing from API response");
        }

        invoice.AttachBankAccount(response.BankAccount);
    }

    public static async System.Threading.Tasks.Task HydrateBankAccountAsync(
        CreditNote creditNote,
        IFreeAgentRequestClient requestClient,
        bool includeBankAccount,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(creditNote);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeBankAccount || creditNote.BankAccount is not null || creditNote.BankAccountId is not long bankAccountId)
        {
            return;
        }

        var response = await requestClient.GetAsync<BankAccountResponse>($"bank_accounts/{bankAccountId}", cancellationToken);

        if (response.BankAccount is null)
        {
            throw new FreeAgentApiException("Bank account data missing from API response");
        }

        creditNote.AttachBankAccount(response.BankAccount);
    }

    public static async System.Threading.Tasks.Task HydrateBankAccountAsync(
        RecurringInvoice recurringInvoice,
        IFreeAgentRequestClient requestClient,
        bool includeBankAccount,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(recurringInvoice);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeBankAccount
            || recurringInvoice.BankAccount is not null
            || recurringInvoice.BankAccountId is not long bankAccountId)
        {
            return;
        }

        var response = await requestClient.GetAsync<BankAccountResponse>($"bank_accounts/{bankAccountId}", cancellationToken);

        if (response.BankAccount is null)
        {
            throw new FreeAgentApiException("Bank account data missing from API response");
        }

        recurringInvoice.AttachBankAccount(response.BankAccount);
    }

    public static async System.Threading.Tasks.Task HydrateBankAccountAsync(
        BankFeed bankFeed,
        IFreeAgentRequestClient requestClient,
        bool includeBankAccount,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(bankFeed);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeBankAccount || bankFeed.BankAccount is not null || bankFeed.BankAccountId is not long bankAccountId)
        {
            return;
        }

        var response = await requestClient.GetAsync<BankAccountResponse>($"bank_accounts/{bankAccountId}", cancellationToken);

        if (response.BankAccount is null)
        {
            throw new FreeAgentApiException("Bank account data missing from API response");
        }

        bankFeed.AttachBankAccount(response.BankAccount);
    }

    public static async System.Threading.Tasks.Task HydrateBillAsync(
        HirePurchase hirePurchase,
        IFreeAgentRequestClient requestClient,
        bool includeBill,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(hirePurchase);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeBill || hirePurchase.Bill is not null || hirePurchase.BillId is not long billId)
        {
            return;
        }

        var response = await requestClient.GetAsync<BillResponse>($"bills/{billId}", cancellationToken);

        if (response.Bill is null)
        {
            throw new FreeAgentApiException("Bill data missing from API response");
        }

        hirePurchase.AttachBill(response.Bill);
    }

    public static async System.Threading.Tasks.Task HydrateLiabilitiesOverOneYearCategoryAsync(
        HirePurchase hirePurchase,
        IFreeAgentRequestClient requestClient,
        bool includeCategory,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(hirePurchase);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeCategory
            || hirePurchase.LiabilitiesOverOneYearCategory is not null
            || hirePurchase.LiabilitiesOverOneYearCategoryNominalCode is not string nominalCode)
        {
            return;
        }

        var response = await requestClient.GetAsync<CategorySingleResponse>(
            $"categories/{nominalCode}",
            cancellationToken);
        var category = CategoryResponseMapper.ToCategory(response);
        hirePurchase.AttachLiabilitiesOverOneYearCategory(category);
    }

    public static async System.Threading.Tasks.Task HydrateLiabilitiesUnderOneYearCategoryAsync(
        HirePurchase hirePurchase,
        IFreeAgentRequestClient requestClient,
        bool includeCategory,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(hirePurchase);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeCategory
            || hirePurchase.LiabilitiesUnderOneYearCategory is not null
            || hirePurchase.LiabilitiesUnderOneYearCategoryNominalCode is not string nominalCode)
        {
            return;
        }

        var response = await requestClient.GetAsync<CategorySingleResponse>(
            $"categories/{nominalCode}",
            cancellationToken);
        var category = CategoryResponseMapper.ToCategory(response);
        hirePurchase.AttachLiabilitiesUnderOneYearCategory(category);
    }

    public static async System.Threading.Tasks.Task HydrateBankTransactionExplanationAsync(
        BankTransactionExplanation explanation,
        IFreeAgentRequestClient requestClient,
        BankTransactionExplanationGetOptions? options,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(explanation);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (options is null)
        {
            return;
        }

        if (options.IncludeCategory
            && explanation.Category is null
            && explanation.CategoryNominalCode is string categoryCode)
        {
            var categoryResponse = await requestClient.GetAsync<CategorySingleResponse>(
                $"categories/{categoryCode}",
                cancellationToken);
            explanation.AttachCategory(CategoryResponseMapper.ToCategory(categoryResponse));
        }

        if (options.IncludeProject && explanation.Project is null && explanation.ProjectId is long projectId)
        {
            var projectResponse = await requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);
            if (projectResponse.Project is null)
            {
                throw new FreeAgentApiException("Project data missing from API response");
            }

            explanation.AttachProject(projectResponse.Project);
        }

        if (options.IncludeBankAccount
            && explanation.BankAccount is null
            && explanation.BankAccountId is long bankAccountId)
        {
            var bankAccountResponse = await requestClient.GetAsync<BankAccountResponse>(
                $"bank_accounts/{bankAccountId}",
                cancellationToken);
            if (bankAccountResponse.BankAccount is null)
            {
                throw new FreeAgentApiException("Bank account data missing from API response");
            }

            explanation.AttachBankAccount(bankAccountResponse.BankAccount);
        }

        if (options.IncludePaidInvoice
            && explanation.PaidInvoice is null
            && explanation.PaidInvoiceId is long paidInvoiceId)
        {
            var invoiceResponse = await requestClient.GetAsync<InvoiceResponse>(
                $"invoices/{paidInvoiceId}",
                cancellationToken);
            if (invoiceResponse.Invoice is null)
            {
                throw new FreeAgentApiException("Invoice data missing from API response");
            }

            explanation.AttachPaidInvoice(invoiceResponse.Invoice);
        }

        if (options.IncludePaidCreditNote
            && explanation.PaidCreditNote is null
            && explanation.PaidCreditNoteId is long paidCreditNoteId)
        {
            var creditNoteResponse = await requestClient.GetAsync<CreditNoteResponse>(
                $"credit_notes/{paidCreditNoteId}",
                cancellationToken);
            if (creditNoteResponse.CreditNote is null)
            {
                throw new FreeAgentApiException("Credit note data missing from API response");
            }

            explanation.AttachPaidCreditNote(creditNoteResponse.CreditNote);
        }

        if (options.IncludePaidBill && explanation.PaidBill is null && explanation.PaidBillId is long paidBillId)
        {
            var billResponse = await requestClient.GetAsync<BillResponse>($"bills/{paidBillId}", cancellationToken);
            if (billResponse.Bill is null)
            {
                throw new FreeAgentApiException("Bill data missing from API response");
            }

            explanation.AttachPaidBill(billResponse.Bill);
        }

        if (options.IncludePaidUser && explanation.PaidUser is null && explanation.PaidUserId is long paidUserId)
        {
            var userResponse = await requestClient.GetAsync<UserResponse>($"users/{paidUserId}", cancellationToken);
            if (userResponse.User is null)
            {
                throw new FreeAgentApiException("User data missing from API response");
            }

            explanation.AttachPaidUser(userResponse.User);
        }

        if (options.IncludeDirectContact
            && explanation.DirectContact is null
            && explanation.DirectContactId is long directContactId)
        {
            var contactResponse = await requestClient.GetAsync<ContactResponse>(
                $"contacts/{directContactId}",
                cancellationToken);
            if (contactResponse.Contact is null)
            {
                throw new FreeAgentApiException("Contact data missing from API response");
            }

            explanation.AttachDirectContact(contactResponse.Contact);
        }

        if (options.IncludeTransferBankAccount
            && explanation.TransferBankAccount is null
            && explanation.TransferBankAccountId is long transferBankAccountId)
        {
            var transferResponse = await requestClient.GetAsync<BankAccountResponse>(
                $"bank_accounts/{transferBankAccountId}",
                cancellationToken);
            if (transferResponse.BankAccount is null)
            {
                throw new FreeAgentApiException("Bank account data missing from API response");
            }

            explanation.AttachTransferBankAccount(transferResponse.BankAccount);
        }

        if (options.IncludeStockItem && explanation.StockItem is null && explanation.StockItemId is long stockItemId)
        {
            var stockResponse = await requestClient.GetAsync<StockItemResponse>(
                $"stock_items/{stockItemId}",
                cancellationToken);
            if (stockResponse.StockItem is null)
            {
                throw new FreeAgentApiException("Stock item data missing from API response");
            }

            explanation.AttachStockItem(stockResponse.StockItem);
        }
    }
}
