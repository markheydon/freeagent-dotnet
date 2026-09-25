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
using TaskModel = FreeAgent.Client.Models.Tasks.Task;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Resolves sandbox entities for examples without prompting the user.
/// </summary>
internal sealed class SampleData
{
    private readonly SampleContext _context;

    /// <summary>
    /// Initialises sample data helpers.
    /// </summary>
    /// <param name="context">Shared sample context.</param>
    public SampleData(SampleContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Returns the first contact from the active contacts list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A contact.</returns>
    public async Task<Contact> GetFirstContactAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Contacts.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no contacts found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns the first project from the projects list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A project.</returns>
    public async Task<Project> GetFirstProjectAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Projects.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no projects found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns a project picked from the first page of results.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A project.</returns>
    public async Task<Project> GetRandomProjectAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Projects.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no projects found in sandbox account");
        }

        var index = Random.Shared.Next(page.Items.Count);
        return page.Items[index];
    }

    /// <summary>
    /// Returns the first task from the tasks list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task.</returns>
    public async Task<TaskModel> GetFirstTaskAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Tasks.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no tasks found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns the first timeslip from the timeslips list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A timeslip.</returns>
    public async Task<Timeslip> GetFirstTimeslipAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Timeslips.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no timeslips found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns the first note from a contact or project in the sandbox account.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A note.</returns>
    public async Task<Note> GetFirstNoteAsync(CancellationToken cancellationToken = default)
    {
        var contact = await GetFirstContactAsync(cancellationToken);
        var contactNotes = await _context.Client.Notes.ListContactNotesAsync(
            contactId: contact.ResourceId,
            cancellationToken: cancellationToken);
        if (contactNotes.Count > 0)
        {
            return contactNotes[0];
        }

        var project = await GetFirstProjectAsync(cancellationToken);
        var projectNotes = await _context.Client.Notes.ListProjectNotesAsync(
            projectId: project.ResourceId,
            cancellationToken: cancellationToken);
        if (projectNotes.Count > 0)
        {
            return projectNotes[0];
        }

        SampleContext.Skip("no notes found in sandbox account");
        return default!;
    }

    /// <summary>
    /// Returns the nominal code of the first income category.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An income category nominal code.</returns>
    public async Task<string> GetFirstIncomeCategoryNominalCodeAsync(CancellationToken cancellationToken = default)
    {
        var sets = await _context.Client.Categories.ListAsync(cancellationToken: cancellationToken);
        if (sets.IncomeCategories.Count == 0)
        {
            SampleContext.Skip("no income categories found in sandbox account");
        }

        var nominalCode = sets.IncomeCategories[0].NominalCode;
        if (string.IsNullOrWhiteSpace(nominalCode))
        {
            SampleContext.Skip("first income category has no nominal code");
        }

        return nominalCode;
    }

    /// <summary>
    /// Returns the first invoice from the invoices list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An invoice.</returns>
    public async Task<Invoice> GetFirstInvoiceAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Invoices.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no invoices found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns the first estimate from the estimates list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An estimate.</returns>
    public async Task<Estimate> GetFirstEstimateAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Estimates.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no estimates found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns the first credit note from the credit notes list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A credit note.</returns>
    public async Task<CreditNote> GetFirstCreditNoteAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.CreditNotes.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no credit notes found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns the first credit note reconciliation from the reconciliations list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A credit note reconciliation.</returns>
    public async Task<CreditNoteReconciliation> GetFirstCreditNoteReconciliationAsync(CancellationToken cancellationToken = default)
    {
        var reconciliations = await _context.Client.CreditNoteReconciliations.ListAsync(cancellationToken: cancellationToken);
        if (reconciliations.Count == 0)
        {
            SampleContext.Skip("no credit note reconciliations found in sandbox account");
        }

        return reconciliations[0];
    }

    /// <summary>
    /// Returns the first recurring invoice from the recurring invoices list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A recurring invoice.</returns>
    public async Task<RecurringInvoice> GetFirstRecurringInvoiceAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.RecurringInvoices.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no recurring invoices found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns the first user from the users list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A user.</returns>
    public async Task<User> GetFirstUserAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Client.Users.ListAsync(cancellationToken: cancellationToken);
        if (users.Count == 0)
        {
            SampleContext.Skip("no users found in sandbox account");
        }

        return users[0];
    }
}
