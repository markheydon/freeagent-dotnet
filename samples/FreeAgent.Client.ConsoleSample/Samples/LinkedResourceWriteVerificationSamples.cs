using System.Diagnostics.CodeAnalysis;
using System.Net;
using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Live sandbox checks for linked-resource PUT semantics (GitHub issue #134).
/// </summary>
[ConsoleSamples(Category = "Linked resources")]
internal sealed class LinkedResourceWriteVerificationSamples(SampleContext context) : IConsoleSampleProvider
{
    /// <summary>Display name for menu discovery and <c>--verify-linked-resources</c>.</summary>
    internal const string VerifySampleName = "Verify linked-resource writes (create, update)";

    [ConsoleSample(
        Name = VerifySampleName,
        ExcludeFromRunAll = true,
        MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task VerifyLinkedResourceWritesAsync(CancellationToken cancellationToken)
    {
        SandboxWriteGuard.EnsureRunAllAllowed(context.Client.Environment);

        var results = new List<(string Criterion, bool Passed, string? Detail)>();
        var environment = context.Client.Environment.ToString();
        var timestamp = DateTimeOffset.UtcNow;

        long? probeProjectId = null;
        long? probeInvoiceId = null;

        try
        {
            var contact = await context.Data.GetFirstContactAsync(cancellationToken);
            var expectedContactId = contact.ResourceId;

            var createdProject = await CreateProbeProjectAsync(expectedContactId, cancellationToken);
            probeProjectId = createdProject.ResourceId;

            if (createdProject.ContactId != expectedContactId)
            {
                throw new InvalidOperationException(
                    $"Project create: expected ContactId {expectedContactId}, got {createdProject.ContactId}.");
            }

            results.Add(("Project create with ContactId", true, $"ContactId={expectedContactId}"));

            var fromGet = await context.Client.Projects.GetProjectAsync(createdProject.ResourceId, cancellationToken);
            fromGet.Name = $"{fromGet.Name} (scalar update)";

            await context.Client.Projects.UpdateProjectAsync(
                fromGet.ResourceId,
                fromGet,
                cancellationToken: cancellationToken);

            var afterScalarUpdate = await context.Client.Projects.GetProjectAsync(
                fromGet.ResourceId,
                cancellationToken);

            if (afterScalarUpdate.ContactId != expectedContactId)
            {
                throw new InvalidOperationException(
                    $"Project scalar update: ContactId should remain {expectedContactId}, got {afterScalarUpdate.ContactId}.");
            }

            results.Add(("Project scalar update preserves ContactId", true, null));

            var projectForNullContact = await context.Client.Projects.GetProjectAsync(
                fromGet.ResourceId,
                cancellationToken);
            projectForNullContact.ContactId = null;

            try
            {
                await context.Client.Projects.UpdateProjectAsync(
                    projectForNullContact.ResourceId,
                    projectForNullContact,
                    cancellationToken: cancellationToken);

                throw new InvalidOperationException(
                    "Project ContactId null update: expected API validation failure, but the update succeeded.");
            }
            catch (FreeAgentApiException ex) when (ex.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.UnprocessableEntity)
            {
                if (LinkedResourceVerificationCriteria.IsProjectContactIdBlankRejection(ex))
                {
                    results.Add((
                        "Project ContactId null is rejected by API (wire sends null)",
                        true,
                        ex.Message));
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Project ContactId null update: expected API validation rejecting a blank contact, "
                        + $"but got status {(int)ex.StatusCode} {ex.StatusCode} with message: {ex.Message}");
                }
            }

            var invoice = await CreateProbeDraftInvoiceAsync(
                expectedContactId,
                afterScalarUpdate.ResourceId,
                cancellationToken);
            probeInvoiceId = invoice.ResourceId;
            var originalLineCount = invoice.InvoiceItems?.Count ?? 0;
            var originalFirstItemId = invoice.InvoiceItems?.FirstOrDefault()?.ItemId;

            if (originalFirstItemId is not > 0)
            {
                var invoiceWithLineIds = await context.Client.Invoices.GetInvoiceAsync(
                    invoice.ResourceId,
                    cancellationToken);
                originalLineCount = invoiceWithLineIds.InvoiceItems?.Count ?? originalLineCount;
                originalFirstItemId = invoiceWithLineIds.InvoiceItems?.FirstOrDefault()?.ItemId;
            }

            if (originalLineCount > 0 && originalFirstItemId is not > 0)
            {
                throw new InvalidOperationException(
                    $"Invoice create: expected first line ItemId after GET refetch when {originalLineCount} line item(s) present, but ItemId was missing or zero.");
            }

            if (invoice.ContactId != expectedContactId)
            {
                throw new InvalidOperationException(
                    $"Invoice create: expected ContactId {expectedContactId}, got {invoice.ContactId}.");
            }

            if (invoice.ProjectId != afterScalarUpdate.ResourceId)
            {
                throw new InvalidOperationException(
                    $"Invoice create: expected ProjectId {afterScalarUpdate.ResourceId}, got {invoice.ProjectId}.");
            }

            var invoiceFromGet = await context.Client.Invoices.GetInvoiceAsync(invoice.ResourceId, cancellationToken);
            invoiceFromGet.Comments = "Linked-resource verification scalar update (#134)";

            await context.Client.Invoices.UpdateInvoiceAsync(
                invoiceFromGet.ResourceId,
                invoiceFromGet,
                new InvoiceUpdateOptions { OmitLineItems = true },
                cancellationToken);

            var afterInvoiceUpdate = await context.Client.Invoices.GetInvoiceAsync(
                invoiceFromGet.ResourceId,
                cancellationToken);

            if (afterInvoiceUpdate.ContactId != expectedContactId)
            {
                throw new InvalidOperationException(
                    $"Invoice OmitLineItems update: ContactId should remain {expectedContactId}, got {afterInvoiceUpdate.ContactId}.");
            }

            var lineCountAfter = afterInvoiceUpdate.InvoiceItems?.Count ?? 0;
            if (lineCountAfter != originalLineCount)
            {
                throw new InvalidOperationException(
                    $"Invoice OmitLineItems update: line item count should remain {originalLineCount}, got {lineCountAfter}.");
            }

            var firstItemIdAfter = afterInvoiceUpdate.InvoiceItems?.FirstOrDefault()?.ItemId;
            if (originalFirstItemId is > 0 && firstItemIdAfter != originalFirstItemId)
            {
                throw new InvalidOperationException(
                    $"Invoice OmitLineItems update: first line ItemId should remain {originalFirstItemId}, got {firstItemIdAfter}.");
            }

            results.Add((
                "Invoice scalar update with OmitLineItems preserves ContactId and line items",
                true,
                $"ContactId={expectedContactId}, line items={lineCountAfter}"));

            var company = await context.Client.Company.GetCompanyAsync(cancellationToken);
            if (company.Type == CompanyType.UkUnincorporatedLandlord)
            {
                var properties = await context.Client.Properties.ListAsync(cancellationToken: cancellationToken);
                if (properties.Count > 0)
                {
                var landlordPropertyId = properties[0].ResourceId;
                if (landlordPropertyId is > 0)
                {
                    var invoiceForProperty = await context.Client.Invoices.GetInvoiceAsync(
                        invoiceFromGet.ResourceId,
                        cancellationToken);
                    invoiceForProperty.PropertyId = landlordPropertyId;

                    await context.Client.Invoices.UpdateInvoiceAsync(
                        invoiceForProperty.ResourceId,
                        invoiceForProperty,
                        new InvoiceUpdateOptions { OmitLineItems = true },
                        cancellationToken);

                    var afterPropertyUpdate = await context.Client.Invoices.GetInvoiceAsync(
                        invoiceForProperty.ResourceId,
                        cancellationToken);

                    if (afterPropertyUpdate.PropertyId != landlordPropertyId)
                    {
                        throw new InvalidOperationException(
                            $"Invoice PropertyId update: expected {landlordPropertyId}, got {afterPropertyUpdate.PropertyId}.");
                    }

                    results.Add((
                        "Invoice PropertyId write round-trip (landlord sandbox)",
                        true,
                        $"PropertyId={landlordPropertyId}"));
                }
                }
            }

            var invoiceForProjectClear = await context.Client.Invoices.GetInvoiceAsync(
                invoiceFromGet.ResourceId,
                cancellationToken);
            invoiceForProjectClear.ProjectId = null;

            await context.Client.Invoices.UpdateInvoiceAsync(
                invoiceForProjectClear.ResourceId,
                invoiceForProjectClear,
                new InvoiceUpdateOptions { OmitLineItems = true },
                cancellationToken);

            var afterProjectClear = await context.Client.Invoices.GetInvoiceAsync(
                invoiceForProjectClear.ResourceId,
                cancellationToken);

            if (afterProjectClear.ProjectId is not null)
            {
                throw new InvalidOperationException(
                    $"Invoice clear link: expected ProjectId null after explicit null PUT, got {afterProjectClear.ProjectId}.");
            }

            if (afterProjectClear.ContactId != expectedContactId)
            {
                throw new InvalidOperationException(
                    $"Invoice clear project link: ContactId should remain {expectedContactId}, got {afterProjectClear.ContactId}.");
            }

            results.Add((
                "Explicit ProjectId null clears optional invoice project link",
                true,
                "Sandbox rejects blank project billing contact (contact_id can't be blank); optional links verified on invoice"));
        }
        catch (Exception ex)
        {
            results.Add(("Verification run", false, ex.Message));
            WriteIssueSummary(results, environment, timestamp, overallPass: false);
            throw;
        }
        finally
        {
            if (probeInvoiceId is > 0)
            {
                try
                {
                    await context.Client.Invoices.DeleteInvoiceAsync(probeInvoiceId.Value, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Cleanup: failed to delete probe invoice {probeInvoiceId}: {ex.Message}");
                }
            }

            if (probeProjectId is > 0)
            {
                try
                {
                    await context.Client.Projects.DeleteProjectAsync(probeProjectId.Value, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Cleanup: failed to delete probe project {probeProjectId}: {ex.Message}");
                }
            }
        }

        SampleOutput.WriteHeader("Linked-resource write verification");
        foreach (var (criterion, passed, detail) in results)
        {
            SampleOutput.WriteField(criterion, passed ? "PASS" : "FAIL");
            if (!string.IsNullOrWhiteSpace(detail))
            {
                SampleOutput.WriteField("  detail", detail);
            }
        }

        WriteIssueSummary(results, environment, timestamp, overallPass: true);
    }

    private static void WriteIssueSummary(
        IReadOnlyList<(string Criterion, bool Passed, string? Detail)> results,
        string environment,
        DateTimeOffset timestamp,
        bool overallPass)
    {
        SampleOutput.WriteHeader("Copy to GitHub issue #134");
        Console.WriteLine("## Sandbox linked-resource PUT verification");
        Console.WriteLine();
        Console.WriteLine($"- **Environment:** {environment}");
        Console.WriteLine($"- **UTC:** {timestamp:yyyy-MM-dd HH:mm:ss}Z");
        Console.WriteLine($"- **Overall:** {(overallPass ? "PASS" : "FAIL")}");
        Console.WriteLine();
        foreach (var (criterion, passed, detail) in results)
        {
            var line = $"- [{(passed ? "x" : " ")}] {criterion}";
            if (!string.IsNullOrWhiteSpace(detail))
            {
                line += $" — {detail}";
            }

            Console.WriteLine(line);
        }

        Console.WriteLine();
        Console.WriteLine("Closes verification for [#134](https://github.com/markheydon/freeagent-dotnet/issues/134).");
    }

    private async Task<Project> CreateProbeProjectAsync(long contactId, CancellationToken cancellationToken)
    {
        return await context.Client.Projects.CreateProjectAsync(
            new Project
            {
                Name = $"Linked-resource verify project {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}",
                ContactId = contactId,
                Status = ProjectStatus.Active,
                Currency = CurrencyCode.GBP,
                Budget = 0m,
                BudgetUnits = ProjectBudgetUnits.Hours,
                HoursPerDay = 8m,
                NormalBillingRate = 0m,
                BillingPeriod = ProjectBillingPeriod.Hour,
                UsesProjectInvoiceSequence = false,
                IncludeUnbilledTimeInProfitability = true,
                IsIr35 = false
            },
            cancellationToken);
    }

    private async Task<Invoice> CreateProbeDraftInvoiceAsync(
        long contactId,
        long projectId,
        CancellationToken cancellationToken)
    {
        var nominalCode = await context.Data.GetFirstIncomeCategoryNominalCodeAsync(cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await context.Client.Invoices.CreateInvoiceAsync(
            new Invoice
            {
                ContactId = contactId,
                ProjectId = projectId,
                DatedOn = today,
                DueOn = today.AddDays(14),
                PaymentTermsInDays = 14,
                Reference = $"Linked-resource verify {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}",
                InvoiceItems =
                [
                    new InvoiceItem
                    {
                        Description = "Linked-resource verification line",
                        ItemType = InvoiceItemType.Services,
                        Quantity = 1,
                        Price = 100,
                        CategoryNominalCode = nominalCode,
                    }
                ]
            },
            cancellationToken);
    }
}
