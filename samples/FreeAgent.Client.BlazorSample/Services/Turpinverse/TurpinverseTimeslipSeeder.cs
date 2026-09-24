using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.Timeslips;
using TaskModel = FreeAgent.Client.Models.Tasks.Task;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Seeds FreeAgent timeslips against Turpinverse canon tasks.
/// </summary>
public sealed class TurpinverseTimeslipSeeder
{
    public static readonly DateOnly ProbeDatedOn = new(2024, 3, 18);

    private readonly TurpinverseTaskSeeder _taskSeeder;

    public TurpinverseTimeslipSeeder(TurpinverseTaskSeeder taskSeeder)
    {
        _taskSeeder = taskSeeder ?? throw new ArgumentNullException(nameof(taskSeeder));
    }

    public async Task<TurpinverseTimeslipSeedResult> CreateBlackBessProbeTimeslipAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        var taskResult = await _taskSeeder.CreateBlackBessGeneralTaskAsync(client, cancellationToken);
        var task = taskResult.Task;

        if (task.ProjectId is not long projectId)
        {
            throw new InvalidOperationException("Black Bess task is missing a parent project ID.");
        }

        var currentUser = await client.Users.GetCurrentUserAsync(cancellationToken);
        if (!currentUser.TryGetResourceId(out var userId))
        {
            throw new InvalidOperationException("Could not parse the current user ID.");
        }

        return await UpsertTimeslipAsync(
            client,
            task,
            projectId,
            userId,
            ProbeDatedOn,
            hours: 1.5m,
            comment: "Turpinverse SDK probe timeslip",
            cancellationToken);
    }

    private static async Task<TurpinverseTimeslipSeedResult> UpsertTimeslipAsync(
        FreeAgentClient client,
        TaskModel task,
        long projectId,
        long userId,
        DateOnly datedOn,
        decimal hours,
        string comment,
        CancellationToken cancellationToken)
    {
        if (!task.TryGetResourceId(out var taskId))
        {
            throw new InvalidOperationException("Could not parse task ID from URL.");
        }

        var existing = await FindExistingTimeslipAsync(client, taskId, userId, datedOn, cancellationToken);
        var desired = new Timeslip
        {
            LinkedTask = client.Urls.Task(taskId),
            LinkedUser = client.Urls.User(userId),
            LinkedProject = client.Urls.Project(projectId),
            DatedOn = datedOn,
            Hours = hours,
            Comment = comment
        };

        if (existing is null)
        {
            var created = await client.Timeslips.CreateTimeslipAsync(desired, cancellationToken);
            return new TurpinverseTimeslipSeedResult(created, TimeslipSeedAction.Created);
        }

        desired.Comment = comment;
        desired.Hours = hours;
        var updated = await client.Timeslips.UpdateTimeslipAsync(existing.ResourceId, desired, cancellationToken);
        return new TurpinverseTimeslipSeedResult(updated, TimeslipSeedAction.Updated);
    }

    private static async Task<Timeslip?> FindExistingTimeslipAsync(
        FreeAgentClient client,
        long taskId,
        long userId,
        DateOnly datedOn,
        CancellationToken cancellationToken)
    {
        var page = await client.Timeslips.ListAsync(
            perPage: 100,
            fromDate: datedOn,
            toDate: datedOn,
            taskId: taskId,
            userId: userId,
            cancellationToken: cancellationToken);

        return page.Items.FirstOrDefault(timeslip => timeslip.DatedOn == datedOn);
    }
}

public enum TimeslipSeedAction
{
    Created,
    Updated
}

public sealed record TurpinverseTimeslipSeedResult(Timeslip Timeslip, TimeslipSeedAction Action);
