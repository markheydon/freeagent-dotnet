using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Tasks;
using ProjectTaskStatus = FreeAgent.Client.Models.Tasks.ProjectTaskStatus;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Tasks endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Tasks")]
internal sealed class ProjectTaskSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List tasks")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListTasksAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.ProjectTasks.ListAsync(perPage: 25, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Tasks (showing {page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(page.Items, task => $"{task.ResourceId,8}  {task.Name}  ({task.Status})");
    }

    [ConsoleSample(Name = "List tasks for project")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListTasksForProjectAsync(CancellationToken cancellationToken)
    {
        var project = await context.Data.GetRandomProjectAsync(cancellationToken);
        var page = await context.Client.ProjectTasks.ListAsync(
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
        var detail = await context.Client.ProjectTasks.GetProjectTaskAsync(task.ResourceId, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Task detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Name", detail.Name);
        SampleOutput.WriteField("Status", detail.Status);
        SampleOutput.WriteField("Billing rate", detail.BillingRate);
    }

    [ConsoleSample(Name = "Create probe task and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeTaskAndDeleteAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleTaskAsync(cancellationToken);

        SampleOutput.WriteHeader("Created probe task");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Name", created.Name);

        await context.Client.ProjectTasks.DeleteProjectTaskAsync(created.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted probe task");
        SampleOutput.WriteField("Id", created.ResourceId);
    }

    [ConsoleSample(Name = "Update task name")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateTaskNameAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleTaskAsync(cancellationToken);
        created.Name = $"{created.Name} (updated)";

        var updated = await context.Client.ProjectTasks.UpdateProjectTaskAsync(created.ResourceId, created, cancellationToken);

        SampleOutput.WriteHeader("Updated task name");
        SampleOutput.WriteField("Id", updated.ResourceId);
        SampleOutput.WriteField("Name", updated.Name);

        await context.Client.ProjectTasks.DeleteProjectTaskAsync(updated.ResourceId, cancellationToken);
    }

    private async Task<ProjectTask> CreateSampleTaskAsync(CancellationToken cancellationToken)
    {
        var project = await context.Data.GetFirstProjectAsync(cancellationToken);

        return await context.Client.ProjectTasks.CreateProjectTaskAsync(
            project.ResourceId,
            new ProjectTask
            {
                Name = $"Console probe task {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}",
                IsBillable = true,
                Status = ProjectTaskStatus.Active,
                BillingRate = 0m,
                BillingPeriod = Models.Tasks.ProjectTaskBillingPeriod.Hour
            },
            cancellationToken);
    }
}
