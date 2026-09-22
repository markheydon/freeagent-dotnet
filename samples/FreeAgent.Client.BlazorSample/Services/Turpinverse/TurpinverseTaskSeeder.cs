using FreeAgent.Client;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Tasks;
using TaskModel = FreeAgent.Client.Models.Tasks.Task;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Seeds FreeAgent tasks under Turpinverse canon projects.
/// </summary>
public sealed class TurpinverseTaskSeeder
{
    public const string TaskNamePrefix = "turpinverse:";

    private readonly TurpinverseProjectCatalog _projectCatalog;

    public TurpinverseTaskSeeder(TurpinverseProjectCatalog projectCatalog)
    {
        _projectCatalog = projectCatalog ?? throw new ArgumentNullException(nameof(projectCatalog));
    }

    public async Task<TurpinverseTaskSeedResult> CreateBlackBessGeneralTaskAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await _projectCatalog.EnsureLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var project = await ResolveProjectByContractReferenceAsync(
            client,
            TurpinverseProjectMapper.BuildContractReference(TurpinverseProjectCatalog.BlackBessRouteOptimiserProjectId),
            cancellationToken);

        return await UpsertTaskAsync(
            client,
            project,
            BuildTaskName(TurpinverseProjectCatalog.BlackBessRouteOptimiserProjectId, "general"),
            cancellationToken);
    }

    public async Task<TurpinverseTaskBulkSeedResult> CreateAllTurpinverseTasksAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await _projectCatalog.EnsureLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var existingProjects = await LoadTurpinverseProjectsAsync(client, cancellationToken);
        var created = new List<TaskModel>();
        var updated = new List<TaskModel>();
        var failures = new List<TurpinverseTaskSeedFailure>();

        foreach (var canonProject in _projectCatalog.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var contractReference = TurpinverseProjectMapper.BuildContractReference(canonProject.Id);
            if (!existingProjects.TryGetValue(contractReference, out var project))
            {
                failures.Add(new TurpinverseTaskSeedFailure(
                    canonProject.Id,
                    canonProject.Title,
                    "Matching Turpinverse project was not found in FreeAgent. Seed projects first."));
                continue;
            }

            try
            {
                var result = await UpsertTaskAsync(
                    client,
                    project,
                    BuildTaskName(canonProject.Id, "general"),
                    cancellationToken);

                if (result.Action == TaskSeedAction.Created)
                {
                    created.Add(result.Task);
                }
                else
                {
                    updated.Add(result.Task);
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or FreeAgentApiException)
            {
                failures.Add(new TurpinverseTaskSeedFailure(canonProject.Id, canonProject.Title, ex.Message));
            }
        }

        return new TurpinverseTaskBulkSeedResult(created, updated, failures);
    }

    public static string BuildTaskName(string projectId, string taskSlug) =>
        $"{TaskNamePrefix}{projectId}:{taskSlug}";

    private static async System.Threading.Tasks.Task<TurpinverseTaskSeedResult> UpsertTaskAsync(
        FreeAgentClient client,
        Project project,
        string taskName,
        CancellationToken cancellationToken)
    {
        var projectId = project.GetResourceId();
        var existingTasks = await LoadTasksByNameForProjectAsync(client, projectId, cancellationToken);
        var desired = new TaskModel
        {
            Name = taskName,
            IsBillable = true,
            Status = Models.Tasks.TaskStatus.Active,
            BillingRate = project.NormalBillingRate ?? 0m,
            BillingPeriod = project.BillingPeriod == ProjectBillingPeriod.Day
                ? TaskBillingPeriod.Day
                : TaskBillingPeriod.Hour
        };

        if (!existingTasks.TryGetValue(taskName, out var existingMatch))
        {
            var created = await client.Tasks.CreateTaskAsync(projectId, desired, cancellationToken);
            return new TurpinverseTaskSeedResult(created, TaskSeedAction.Created);
        }

        var taskId = existingMatch.GetResourceId();
        var current = await client.Tasks.GetTaskAsync(taskId, cancellationToken);
        current.Name = desired.Name;
        current.IsBillable = desired.IsBillable;
        current.Status = desired.Status;
        current.BillingRate = desired.BillingRate;
        current.BillingPeriod = desired.BillingPeriod;
        var updated = await client.Tasks.UpdateTaskAsync(taskId, current, cancellationToken);
        return new TurpinverseTaskSeedResult(updated, TaskSeedAction.Updated);
    }

    private static async Task<Project> ResolveProjectByContractReferenceAsync(
        FreeAgentClient client,
        string contractReference,
        CancellationToken cancellationToken)
    {
        var projects = await LoadTurpinverseProjectsAsync(client, cancellationToken);
        if (!projects.TryGetValue(contractReference, out var project))
        {
            throw new InvalidOperationException(
                $"No FreeAgent project exists for contract reference '{contractReference}'. Seed projects first.");
        }

        return project;
    }

    private static async Task<Dictionary<string, Project>> LoadTurpinverseProjectsAsync(
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

    private static async Task<Dictionary<string, TaskModel>> LoadTasksByNameForProjectAsync(
        FreeAgentClient client,
        long projectId,
        CancellationToken cancellationToken)
    {
        var tasksByName = new Dictionary<string, TaskModel>(StringComparer.Ordinal);

        await foreach (var task in client.Tasks.ListAutoPagingAsync(projectId: projectId, cancellationToken: cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(task.Name)
                || !task.Name.StartsWith(TaskNamePrefix, StringComparison.Ordinal))
            {
                continue;
            }

            tasksByName[task.Name] = task;
        }

        return tasksByName;
    }
}

public enum TaskSeedAction
{
    Created,
    Updated
}

public sealed record TurpinverseTaskSeedResult(TaskModel Task, TaskSeedAction Action);

public sealed record TurpinverseTaskSeedFailure(string ProjectId, string Title, string Message);

public sealed record TurpinverseTaskBulkSeedResult(
    IReadOnlyList<TaskModel> Created,
    IReadOnlyList<TaskModel> Updated,
    IReadOnlyList<TurpinverseTaskSeedFailure> Failures);
