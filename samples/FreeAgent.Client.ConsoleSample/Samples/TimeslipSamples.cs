using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Timeslips;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Timeslips endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Timeslips")]
internal sealed class TimeslipSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List timeslips")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListTimeslipsAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.Timeslips.ListAsync(perPage: 25, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Timeslips (showing {page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(
            page.Items,
            timeslip => $"{timeslip.ResourceId,8}  {timeslip.DatedOn:yyyy-MM-dd}  {timeslip.Hours,6:0.##}h");
    }

    [ConsoleSample(Name = "List unbilled timeslips")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListUnbilledTimeslipsAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.Timeslips.ListAsync(
            perPage: 25,
            view: TimeslipViews.Unbilled,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Unbilled timeslips ({page.Items.Count})");
        SampleOutput.WriteRows(
            page.Items,
            timeslip => $"{timeslip.ResourceId,8}  {timeslip.DatedOn:yyyy-MM-dd}  {timeslip.Hours,6:0.##}h");
    }

    [ConsoleSample(Name = "List timeslips for task")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListTimeslipsForTaskAsync(CancellationToken cancellationToken)
    {
        var task = await context.Data.GetFirstTaskAsync(cancellationToken);
        var page = await context.Client.Timeslips.ListAsync(
            perPage: 25,
            taskId: task.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Timeslips for {task.Name} ({page.Items.Count})");
        SampleOutput.WriteRows(
            page.Items,
            timeslip => $"{timeslip.ResourceId,8}  {timeslip.DatedOn:yyyy-MM-dd}  {timeslip.Hours,6:0.##}h");
    }

    [ConsoleSample(Name = "Get timeslip detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetTimeslipDetailAsync(CancellationToken cancellationToken)
    {
        var timeslip = await context.Data.GetFirstTimeslipAsync(cancellationToken);
        var detail = await context.Client.Timeslips.GetTimeslipAsync(
            timeslip.ResourceId,
            new TimeslipGetOptions
            {
                IncludeTask = true,
                IncludeProject = true,
                IncludeUser = true
            },
            cancellationToken);

        SampleOutput.WriteHeader("Timeslip detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Date", detail.DatedOn);
        SampleOutput.WriteField("Hours", detail.Hours);
        SampleOutput.WriteField("Task", detail.Task?.Name);
        SampleOutput.WriteField("Project", detail.Project?.Name);
        SampleOutput.WriteField("User", detail.User?.DisplayName);
        SampleOutput.WriteField("Billed on invoice", detail.BilledOnInvoiceId);
    }

    [ConsoleSample(Name = "Create probe timeslip and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeTimeslipAndDeleteAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleTimeslipAsync(cancellationToken);

        SampleOutput.WriteHeader("Created probe timeslip");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Hours", created.Hours);

        await context.Client.Timeslips.DeleteTimeslipAsync(created.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted probe timeslip");
        SampleOutput.WriteField("Id", created.ResourceId);
    }

    [ConsoleSample(Name = "Create probe timeslips batch and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeTimeslipsBatchAndDeleteAsync(CancellationToken cancellationToken)
    {
        var probe = await BuildSampleTimeslipAsync(cancellationToken);
        var created = await context.Client.Timeslips.CreateTimeslipsAsync([probe], cancellationToken);

        SampleOutput.WriteHeader("Batch created timeslips");
        SampleOutput.WriteField("Count", created.Count);
        SampleOutput.WriteField("First ID", created[0].ResourceId);

        foreach (var timeslip in created)
        {
            await context.Client.Timeslips.DeleteTimeslipAsync(timeslip.ResourceId, cancellationToken);
        }

        SampleOutput.WriteHeader("Deleted batch timeslips");
        SampleOutput.WriteField("Count", created.Count);
    }

    [ConsoleSample(Name = "Update timeslip hours")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateTimeslipHoursAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleTimeslipAsync(cancellationToken);
        created.Hours = (created.Hours ?? 0m) + 0.25m;

        var updated = await context.Client.Timeslips.UpdateTimeslipAsync(created.ResourceId, created, cancellationToken);

        SampleOutput.WriteHeader("Updated timeslip hours");
        SampleOutput.WriteField("Id", updated.ResourceId);
        SampleOutput.WriteField("Hours", updated.Hours);

        await context.Client.Timeslips.DeleteTimeslipAsync(updated.ResourceId, cancellationToken);
    }

    [ConsoleSample(Name = "Start and stop timeslip timer", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task StartAndStopTimeslipTimerAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleTimeslipAsync(cancellationToken);
        var started = await context.Client.Timeslips.StartTimerAsync(created.ResourceId, cancellationToken);
        var stopped = await context.Client.Timeslips.StopTimerAsync(created.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Started and stopped timeslip timer");
        SampleOutput.WriteField("Id", stopped.ResourceId);
        SampleOutput.WriteField("Hours after stop", stopped.Hours);
        SampleOutput.WriteField("Timer running", stopped.Timer?.Running);

        await context.Client.Timeslips.DeleteTimeslipAsync(stopped.ResourceId, cancellationToken);
    }

    private async Task<Timeslip> CreateSampleTimeslipAsync(CancellationToken cancellationToken)
    {
        var probe = await BuildSampleTimeslipAsync(cancellationToken);
        return await context.Client.Timeslips.CreateTimeslipAsync(probe, cancellationToken);
    }

    private async Task<Timeslip> BuildSampleTimeslipAsync(CancellationToken cancellationToken)
    {
        var task = await context.Data.GetFirstTaskAsync(cancellationToken);
        var project = await context.Data.GetFirstProjectAsync(cancellationToken);
        var user = await context.Data.GetFirstUserAsync(cancellationToken);

        return new Timeslip
        {
            LinkedTask = context.Client.Urls.Task(task.ResourceId),
            LinkedProject = context.Client.Urls.Project(project.ResourceId),
            LinkedUser = context.Client.Urls.User(user.ResourceId),
            DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
            Hours = 0.5m,
            Comment = $"Console probe timeslip {DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}"
        };
    }
}
