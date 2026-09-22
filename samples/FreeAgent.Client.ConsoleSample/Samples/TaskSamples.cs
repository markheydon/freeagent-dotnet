using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Tasks endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Tasks")]
internal sealed class TaskSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List tasks")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListTasksAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.Tasks.ListAsync(perPage: 25, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Tasks (showing {page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(page.Items, task => $"{task.ResourceId,8}  {task.Name}  ({task.Status})");
    }

    [ConsoleSample(Name = "List tasks for project")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListTasksForProjectAsync(CancellationToken cancellationToken)
    {
        var project = await context.Data.GetRandomProjectAsync(cancellationToken);
        var page = await context.Client.Tasks.ListAsync(
            perPage: 25,
            projectId: project.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Tasks for {project.Name} ({page.Items.Count})");
        SampleOutput.WriteRows(page.Items, task => $"{task.ResourceId,8}  {task.Name}  ({task.Status})");
    }

    [ConsoleSample(Name = "Get task detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetTaskDetailAsync(CancellationToken cancellationToken)
    {
        var task = await context.Data.GetFirstTaskAsync(cancellationToken);
        var detail = await context.Client.Tasks.GetTaskAsync(task.ResourceId, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Task detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Name", detail.Name);
        SampleOutput.WriteField("Status", detail.Status);
        SampleOutput.WriteField("Billing rate", detail.BillingRate);
    }
}
