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
}
