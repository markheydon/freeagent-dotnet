using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.CreditNoteReconciliations;
using FreeAgent.Client.Models.CreditNotes;
using FreeAgent.Client.Models.Estimates;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Notes;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.RecurringInvoices;
using FreeAgent.Client.Models.Tasks;
using FreeAgent.Client.Models.Timeslips;
using FreeAgent.Client.Models.Users;
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

    public static async System.Threading.Tasks.Task HydrateTaskAsync(
        Timeslip timeslip,
        IFreeAgentRequestClient requestClient,
        bool includeTask,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(timeslip);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeTask || timeslip.Task is not null || timeslip.TaskId is not long taskId)
        {
            return;
        }

        var response = await requestClient.GetAsync<TaskResponse>($"tasks/{taskId}", cancellationToken);

        if (response.Task is null)
        {
            throw new FreeAgentApiException("Task data missing from API response");
        }

        timeslip.AttachTask(response.Task);
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

        var response = await requestClient.GetAsync<Models.CreditNotes.CreditNoteResponse>($"credit_notes/{creditNoteId}", cancellationToken);

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
}
