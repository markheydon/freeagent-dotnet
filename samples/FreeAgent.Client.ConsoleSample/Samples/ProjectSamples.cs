using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Projects endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Projects")]
internal sealed class ProjectSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List projects")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListProjectsAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.Projects.ListAsync(perPage: 25, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Projects (showing {page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(page.Items, project => $"{project.ResourceId,8}  {project.Name}  ({project.Status})");
    }

    [ConsoleSample(Name = "List active projects")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListActiveProjectsAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.Projects.ListAsync(
            perPage: 25,
            view: ProjectViews.Active,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Active projects ({page.Items.Count})");
        SampleOutput.WriteRows(page.Items, project => $"{project.ResourceId,8}  {project.Name}");
    }

    [ConsoleSample(Name = "Get project detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetProjectDetailAsync(CancellationToken cancellationToken)
    {
        var project = await context.Data.GetFirstProjectAsync(cancellationToken);
        var detail = await context.Client.Projects.GetProjectAsync(project.ResourceId, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Project detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Name", detail.Name);
        SampleOutput.WriteField("Status", detail.Status);
        SampleOutput.WriteField("Contact", detail.ContactName);
    }

    [ConsoleSample(Name = "List projects for contact")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListProjectsForContactAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var page = await context.Client.Projects.ListAsync(
            perPage: 25,
            contactId: contact.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Projects for {contact.DisplayName} ({page.Items.Count})");
        SampleOutput.WriteRows(page.Items, project => $"{project.ResourceId,8}  {project.Name}");
    }

    [ConsoleSample(Name = "Create probe project and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeProjectAndDeleteAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleProjectAsync(cancellationToken);

        SampleOutput.WriteHeader("Created probe project");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Name", created.Name);

        await context.Client.Projects.DeleteProjectAsync(created.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted probe project");
        SampleOutput.WriteField("Id", created.ResourceId);
    }

    [ConsoleSample(Name = "Update project name")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateProjectNameAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleProjectAsync(cancellationToken);
        created.Name = $"{created.Name} (updated)";

        var updated = await context.Client.Projects.UpdateProjectAsync(created.ResourceId, created, cancellationToken);

        SampleOutput.WriteHeader("Updated project name");
        SampleOutput.WriteField("Id", updated.ResourceId);
        SampleOutput.WriteField("Name", updated.Name);

        await context.Client.Projects.DeleteProjectAsync(updated.ResourceId, cancellationToken);
    }

    private async Task<Project> CreateSampleProjectAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);

        return await context.Client.Projects.CreateProjectAsync(
            new Project
            {
                Name = $"Console probe project {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}",
                BillingContact = ContactReference.ForEnvironment(context.Client.Environment, contact.ResourceId),
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
}
