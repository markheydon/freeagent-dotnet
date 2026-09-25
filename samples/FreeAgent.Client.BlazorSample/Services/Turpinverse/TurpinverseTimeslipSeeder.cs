using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.Tasks;
using FreeAgent.Client.Models.Timeslips;

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
        var task = taskResult.ProjectTask;

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
            client, task,
            projectId,
            userId,
            ProbeDatedOn,
            hours: 1.5m,
            comment: "Turpinverse SDK probe timeslip",
            cancellationToken);
    }

    private static async Task<TurpinverseTimeslipSeedResult> UpsertTimeslipAsync(
        FreeAgentClient client,
        ProjectTask task,
        long projectId,
        long userId,
        DateOnly datedOn,
        decimal hours,
        string comment,
        CancellationToken cancellationToken)
    {
        if (!task.TryGetResourceId(out var projectTaskId))
        {
            throw new InvalidOperationException("Could not parse task ID from URL.");
        }

        var existing = await FindExistingTimeslipAsync(client, projectTaskId, userId, datedOn, comment, cancellationToken);
        var desired = new Timeslip
        {
            ProjectTaskId = projectTaskId,
            UserId = userId,
            ProjectId = projectId,
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
        long projectTaskId,
        long userId,
        DateOnly datedOn,
        string comment,
        CancellationToken cancellationToken)
    {
        await foreach (var timeslip in client.Timeslips.ListAutoPagingAsync(
                           perPage: 100,
                           fromDate: datedOn,
                           toDate: datedOn,
                           projectTaskId: projectTaskId,
                           userId: userId,
                           cancellationToken: cancellationToken))
        {
            if (timeslip.DatedOn == datedOn
                && string.Equals(timeslip.Comment, comment, StringComparison.Ordinal))
            {
                return timeslip;
            }
        }

        return null;
    }
}

public enum TimeslipSeedAction
{
    Created,
    Updated
}

public sealed record TurpinverseTimeslipSeedResult(Timeslip Timeslip, TimeslipSeedAction Action);
