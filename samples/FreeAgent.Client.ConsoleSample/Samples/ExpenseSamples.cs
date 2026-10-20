using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client;
using FreeAgent.Client.Models.Expenses;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Expenses endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Expenses")]
internal sealed class ExpenseSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List expenses")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListExpensesAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.Expenses.ListAsync(perPage: 25, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Expenses (showing {page.Items.Count} of {page.Total})");
        SampleOutput.WriteRows(
            page.Items,
            expense => $"{expense.ResourceId,8}  {expense.DatedOn:yyyy-MM-dd}  {expense.Description}");
    }

    [ConsoleSample(Name = "List recent expenses")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListRecentExpensesAsync(CancellationToken cancellationToken)
    {
        var page = await context.Client.Expenses.ListAsync(
            perPage: 25,
            view: ExpenseViews.Recent,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Recent expenses ({page.Items.Count})");
        SampleOutput.WriteRows(
            page.Items,
            expense => $"{expense.ResourceId,8}  {expense.DatedOn:yyyy-MM-dd}  {expense.Description}");
    }

    [ConsoleSample(Name = "List expenses for project")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListExpensesForProjectAsync(CancellationToken cancellationToken)
    {
        var project = await context.Data.GetFirstProjectAsync(cancellationToken);
        var page = await context.Client.Expenses.ListAsync(
            perPage: 25,
            projectId: project.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Expenses for {project.Name} ({page.Items.Count})");
        SampleOutput.WriteRows(
            page.Items,
            expense => $"{expense.ResourceId,8}  {expense.DatedOn:yyyy-MM-dd}  {expense.Description}");
    }

    [ConsoleSample(Name = "Get expense detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetExpenseDetailAsync(CancellationToken cancellationToken)
    {
        var expense = await context.Data.GetFirstExpenseAsync(cancellationToken);
        var detail = await context.Client.Expenses.GetExpenseAsync(
            expense.ResourceId,
            new ExpenseGetOptions
            {
                IncludeUser = true,
                IncludeCategory = true,
                IncludeProject = true
            },
            cancellationToken);

        SampleOutput.WriteHeader("Expense detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Date", detail.DatedOn);
        SampleOutput.WriteField("Description", detail.Description);
        SampleOutput.WriteField("Gross value", detail.GrossValue);
        SampleOutput.WriteField("Mileage category", detail.IsMileageCategory);
        SampleOutput.WriteField("Category", detail.Category?.Description ?? detail.CategoryNominalCode);
        SampleOutput.WriteField("User", detail.User?.DisplayName);
        SampleOutput.WriteField("Project", detail.Project?.Name);
    }

    [ConsoleSample(Name = "Get mileage settings")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetMileageSettingsAsync(CancellationToken cancellationToken)
    {
        var settings = await context.Client.Expenses.GetMileageSettingsAsync(cancellationToken);

        SampleOutput.WriteHeader("Mileage settings");
        SampleOutput.WriteField(
            "Engine option ranges",
            settings.EngineTypeAndSizeOptions?.Count ?? 0);
    }

    [ConsoleSample(Name = "Create probe expense and delete", ExcludeFromRunAll = true, MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeExpenseAndDeleteAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleExpenseAsync(cancellationToken);

        SampleOutput.WriteHeader("Created probe expense");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Description", created.Description);

        await context.Client.Expenses.DeleteExpenseAsync(created.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted probe expense");
        SampleOutput.WriteField("Id", created.ResourceId);
    }

    [ConsoleSample(Name = "Create probe mileage expense and delete", ExcludeFromRunAll = true, MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeMileageExpenseAndDeleteAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleMileageExpenseAsync(cancellationToken);

        SampleOutput.WriteHeader("Created probe mileage expense");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Mileage", created.Mileage);

        await context.Client.Expenses.DeleteExpenseAsync(created.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted probe mileage expense");
        SampleOutput.WriteField("Id", created.ResourceId);
    }

    [ConsoleSample(Name = "Create probe expenses batch and delete", ExcludeFromRunAll = true, MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeExpensesBatchAndDeleteAsync(CancellationToken cancellationToken)
    {
        var probe = await BuildSampleExpenseAsync(cancellationToken);
        var second = await BuildSampleExpenseAsync(cancellationToken);
        second.Description = $"{probe.Description} (batch 2)";

        var created = await context.Client.Expenses.CreateExpensesAsync([probe, second], cancellationToken);

        SampleOutput.WriteHeader($"Batch created {created.Count} expenses");
        foreach (var expense in created)
        {
            SampleOutput.WriteField("Id", expense.ResourceId);
        }

        foreach (var expense in created)
        {
            await context.Client.Expenses.DeleteExpenseAsync(expense.ResourceId, cancellationToken);
        }
    }

    private async Task<Expense> CreateSampleExpenseAsync(CancellationToken cancellationToken)
    {
        var expense = await BuildSampleExpenseAsync(cancellationToken);
        return await context.Client.Expenses.CreateExpenseAsync(expense, cancellationToken);
    }

    private async Task<Expense> CreateSampleMileageExpenseAsync(CancellationToken cancellationToken)
    {
        var user = await context.Data.GetFirstUserAsync(cancellationToken);
        var expense = new Expense
        {
            UserId = user.ResourceId,
            IsMileageCategory = true,
            DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
            Mileage = 12m,
            VehicleType = ExpenseVehicleType.Car,
            EngineType = ExpenseEngineType.Petrol,
            Description = $"SDK probe mileage {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss}"
        };

        return await context.Client.Expenses.CreateExpenseAsync(expense, cancellationToken);
    }

    private async Task<Expense> BuildSampleExpenseAsync(CancellationToken cancellationToken)
    {
        var user = await context.Data.GetFirstUserAsync(cancellationToken);
        var nominalCode = await context.Data.GetFirstAdminExpensesCategoryNominalCodeAsync(cancellationToken);

        return new Expense
        {
            UserId = user.ResourceId,
            CategoryNominalCode = nominalCode,
            DatedOn = DateOnly.FromDateTime(DateTime.UtcNow),
            GrossValue = -12.5m,
            Description = $"SDK probe expense {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss}"
        };
    }
}
