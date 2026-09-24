using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Seeds FreeAgent invoices from upstream Turpinverse invoice canon.
/// </summary>
public sealed class TurpinverseInvoiceSeeder
{
    private readonly TurpinverseInvoiceCatalog _invoiceCatalog;
    private readonly TurpinverseContactCatalog _contactCatalog;
    private readonly TurpinverseProjectCatalog _projectCatalog;

    public TurpinverseInvoiceSeeder(
        TurpinverseInvoiceCatalog invoiceCatalog,
        TurpinverseContactCatalog contactCatalog,
        TurpinverseProjectCatalog projectCatalog)
    {
        _invoiceCatalog = invoiceCatalog ?? throw new ArgumentNullException(nameof(invoiceCatalog));
        _contactCatalog = contactCatalog ?? throw new ArgumentNullException(nameof(contactCatalog));
        _projectCatalog = projectCatalog ?? throw new ArgumentNullException(nameof(projectCatalog));
    }

    public async Task<TurpinverseInvoiceSeedResult> CreateHighwayCommissionDraftAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await EnsureCanonLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var invoice = _invoiceCatalog.HighwayCommissionDraft;
        return await UpsertInvoiceAsync(client, invoice, cancellationToken);
    }

    public async Task<TurpinverseInvoiceBulkSeedResult> CreateAllInvoicesAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await EnsureCanonLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var existingInvoices = await LoadExistingInvoicesByReferenceAsync(client, cancellationToken);
        var created = new List<Invoice>();
        var updated = new List<Invoice>();
        var failures = new List<TurpinverseInvoiceSeedFailure>();

        foreach (var invoice in _invoiceCatalog.Invoices)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var result = await UpsertInvoiceAsync(
                    client,
                    invoice,
                    cancellationToken,
                    existingInvoices);

                if (result.Action == InvoiceSeedAction.Created)
                {
                    created.Add(result.Invoice);
                }
                else
                {
                    updated.Add(result.Invoice);
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or FreeAgentApiException)
            {
                failures.Add(new TurpinverseInvoiceSeedFailure(
                    invoice.InvoiceId,
                    invoice.InvoiceNumber,
                    ex.Message));
            }
        }

        return new TurpinverseInvoiceBulkSeedResult(created, updated, failures);
    }

    private async Task EnsureCanonLoadedAsync(bool forceRefresh, CancellationToken cancellationToken)
    {
        await _invoiceCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
        await _contactCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
        await _projectCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TurpinverseInvoiceSeedResult> UpsertInvoiceAsync(
        FreeAgentClient client,
        TurpinverseInvoice invoice,
        CancellationToken cancellationToken,
        Dictionary<string, Invoice>? existingInvoices = null)
    {
        var contact = await ResolveOrganisationContactReferenceAsync(client, invoice.AccountId, cancellationToken);
        var project = await ResolveProjectReferenceAsync(client, invoice, cancellationToken);
        var desired = TurpinverseInvoiceMapper.ToFreeAgentInvoice(invoice, contact, project);
        var reference = TurpinverseInvoiceMapper.BuildReference(invoice.InvoiceId);

        existingInvoices ??= await LoadExistingInvoicesByReferenceAsync(client, cancellationToken);
        if (!existingInvoices.TryGetValue(reference, out var existingMatch))
        {
            var created = await client.Invoices.CreateInvoiceAsync(desired, cancellationToken);
            created = await ApplyCanonStatusAsync(client, created, invoice, cancellationToken);
            existingInvoices[reference] = created;
            return new TurpinverseInvoiceSeedResult(created, InvoiceSeedAction.Created);
        }

        var invoiceId = existingMatch.GetResourceId();
        var current = await client.Invoices.GetInvoiceAsync(invoiceId, cancellationToken);
        MergeWritableFields(current, desired);
        var updated = await client.Invoices.UpdateInvoiceAsync(invoiceId, current, cancellationToken);
        updated = await ApplyCanonStatusAsync(client, updated, invoice, cancellationToken);
        existingInvoices[reference] = updated;
        return new TurpinverseInvoiceSeedResult(updated, InvoiceSeedAction.Updated);
    }

    private async Task<ContactReference> ResolveOrganisationContactReferenceAsync(
        FreeAgentClient client,
        string organisationId,
        CancellationToken cancellationToken)
    {
        var organisation = _contactCatalog.Organisations
            .FirstOrDefault(org => org.Id == organisationId)
            ?? throw new InvalidOperationException($"Turpinverse organisation '{organisationId}' was not found in canon.");

        var upsertEmail = TurpinverseContactMapper.ResolveUpsertEmail(organisation, _contactCatalog.PersonasById);
        var contactsByEmail = await ContactSeederSupport.LoadExistingContactsByEmailAsync(client, cancellationToken);
        if (!contactsByEmail.TryGetValue(upsertEmail, out var contact))
        {
            throw new InvalidOperationException(
                $"No FreeAgent contact exists for organisation '{organisation.TradingName}'. Seed contacts first from Contact CRUD.");
        }

        return ContactReference.Parse(contact.Url);
    }

    private static async Task<ProjectReference?> ResolveProjectReferenceAsync(
        FreeAgentClient client,
        TurpinverseInvoice invoice,
        CancellationToken cancellationToken)
    {
        var projectId = invoice.Lines
            .Select(static line => line.ProjectId)
            .FirstOrDefault(static id => !string.IsNullOrWhiteSpace(id));

        if (string.IsNullOrWhiteSpace(projectId))
        {
            return null;
        }

        var contractReference = TurpinverseProjectMapper.BuildContractReference(projectId);
        var projectsByReference = await LoadExistingProjectsByContractReferenceAsync(client, cancellationToken);
        if (!projectsByReference.TryGetValue(contractReference, out var project))
        {
            throw new InvalidOperationException(
                $"No FreeAgent project exists for Turpinverse project '{projectId}'. Seed projects first from Project CRUD.");
        }

        return ProjectReference.Parse(project.Url);
    }

    private static async Task<Dictionary<string, Invoice>> LoadExistingInvoicesByReferenceAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken)
    {
        var invoicesByReference = new Dictionary<string, Invoice>(StringComparer.Ordinal);

        await foreach (var invoice in client.Invoices.ListAutoPagingAsync(
                           view: InvoiceViews.All,
                           cancellationToken: cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(invoice.Reference)
                || !invoice.Reference.StartsWith(TurpinverseInvoiceMapper.ReferencePrefix, StringComparison.Ordinal))
            {
                continue;
            }

            invoicesByReference[invoice.Reference] = invoice;
        }

        return invoicesByReference;
    }

    private static async Task<Dictionary<string, Project>> LoadExistingProjectsByContractReferenceAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken)
    {
        var projectsByReference = new Dictionary<string, Project>(StringComparer.Ordinal);

        await foreach (var project in client.Projects.ListAutoPagingAsync(cancellationToken: cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(project.ContractPoReference)
                || !project.ContractPoReference.StartsWith(TurpinverseProjectMapper.ContractReferencePrefix, StringComparison.Ordinal))
            {
                continue;
            }

            projectsByReference[project.ContractPoReference] = project;
        }

        return projectsByReference;
    }

    private static async Task<Invoice> ApplyCanonStatusAsync(
        FreeAgentClient client,
        Invoice invoice,
        TurpinverseInvoice canon,
        CancellationToken cancellationToken)
    {
        var invoiceId = invoice.GetResourceId();
        var shouldMarkAsSent = TurpinverseInvoiceMapper.ShouldMarkAsSent(canon.Status);

        if (shouldMarkAsSent && invoice.Status == InvoiceStatus.Draft)
        {
            return await client.Invoices.MarkInvoiceAsSentAsync(invoiceId, cancellationToken);
        }

        if (!shouldMarkAsSent && invoice.Status is InvoiceStatus.Open or InvoiceStatus.Overdue)
        {
            return await client.Invoices.MarkInvoiceAsDraftAsync(invoiceId, cancellationToken);
        }

        return invoice;
    }

    private static void MergeWritableFields(Invoice current, Invoice desired)
    {
        current.BillingContact = desired.BillingContact;
        current.LinkedProject = desired.LinkedProject;
        current.OmitBillingContactFromWrite = false;
        current.OmitProjectFromWrite = desired.LinkedProject is null;
        current.Reference = desired.Reference;
        current.PoReference = desired.PoReference;
        current.DatedOn = desired.DatedOn;
        current.DueOn = desired.DueOn;
        current.PaymentTermsInDays = desired.PaymentTermsInDays;
        current.Currency = desired.Currency;
        current.Comments = desired.Comments;
        current.PaymentTerms = desired.PaymentTerms;
        current.SendNewInvoiceEmails = desired.SendNewInvoiceEmails;
        current.SendReminderEmails = desired.SendReminderEmails;
        current.SendThankYouEmails = desired.SendThankYouEmails;
        current.InvoiceItems = desired.InvoiceItems;
    }
}

public enum InvoiceSeedAction
{
    Created,
    Updated
}

public sealed record TurpinverseInvoiceSeedResult(Invoice Invoice, InvoiceSeedAction Action);

public sealed record TurpinverseInvoiceSeedFailure(string InvoiceId, string InvoiceNumber, string Message);

public sealed record TurpinverseInvoiceBulkSeedResult(
    IReadOnlyList<Invoice> Created,
    IReadOnlyList<Invoice> Updated,
    IReadOnlyList<TurpinverseInvoiceSeedFailure> Failures);
