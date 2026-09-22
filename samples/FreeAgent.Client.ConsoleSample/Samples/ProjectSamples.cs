using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Projects;

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
}
