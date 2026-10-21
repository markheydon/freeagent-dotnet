using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;
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

        if (page.HasNextPage)
        {
            Console.WriteLine();
            Console.WriteLine("  (First page only. Use ListAutoPagingAsync to stream all pages.)");
        }
    }

    [ConsoleSample(Name = "Stream all expenses", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task StreamAllExpensesAsync(CancellationToken cancellationToken)
    {
        var count = 0;
        await foreach (var expense in context.Client.Expenses.ListAutoPagingAsync(cancellationToken: cancellationToken))
        {
            count++;
            if (count <= 5)
            {
                Console.WriteLine($"  {expense.ResourceId,8}  {expense.DatedOn:yyyy-MM-dd}  {expense.Description}");
            }
        }

        SampleOutput.WriteHeader("Streamed expenses via ListAutoPagingAsync");
        Console.WriteLine($"  Total expenses streamed: {count}");
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

    [ConsoleSample(Name = "Create probe expense and delete", MutatesData = true)]
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

    [ConsoleSample(Name = "Create probe mileage expense and delete", MutatesData = true)]
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

    [ConsoleSample(Name = "Update expense description and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateExpenseDescriptionAndDeleteAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleExpenseAsync(cancellationToken);
        created.Description = $"{created.Description} (updated)";

        var updated = await context.Client.Expenses.UpdateExpenseAsync(created.ResourceId, created, cancellationToken);

        SampleOutput.WriteHeader("Updated probe expense");
        SampleOutput.WriteField("Id", updated.ResourceId);
        SampleOutput.WriteField("Description", updated.Description);

        await context.Client.Expenses.DeleteExpenseAsync(updated.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted probe expense");
        SampleOutput.WriteField("Id", updated.ResourceId);
    }

    [ConsoleSample(Name = "Create probe expenses batch and delete", MutatesData = true)]
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
        var datedOn = DateOnly.FromDateTime(DateTime.UtcNow);
        var settings = await context.Client.Expenses.GetMileageSettingsAsync(cancellationToken);

        var expense = new Expense
        {
            UserId = user.ResourceId,
            IsMileageCategory = true,
            DatedOn = datedOn,
            Mileage = 12m,
            VehicleType = ExpenseVehicleType.Car,
            Description = $"SDK probe mileage {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss}"
        };

        if (TryResolveMileageEngine(settings, datedOn, out var engineType, out var engineSize))
        {
            expense.EngineType = engineType;
            expense.EngineSize = engineSize;
        }
        else
        {
            expense.VehicleType = ExpenseVehicleType.Bicycle;
        }

        return await context.Client.Expenses.CreateExpenseAsync(expense, cancellationToken);
    }

    private static bool TryResolveMileageEngine(
        MileageSettings settings,
        DateOnly datedOn,
        out ExpenseEngineType engineType,
        out string engineSize)
    {
        engineType = default;
        engineSize = string.Empty;

        var optionSet = settings.EngineTypeAndSizeOptions?
            .FirstOrDefault(option => option.From <= datedOn && (option.To is null || option.To >= datedOn))
            ?? settings.EngineTypeAndSizeOptions?.LastOrDefault();

        if (optionSet?.Value is null || optionSet.Value.Count == 0)
        {
            return false;
        }

        foreach (var (engineTypeName, sizes) in optionSet.Value)
        {
            if (sizes.Count == 0)
            {
                continue;
            }

            var mappedEngineType = MapEngineTypeName(engineTypeName);
            if (mappedEngineType is null)
            {
                continue;
            }

            engineType = mappedEngineType.Value;
            engineSize = sizes[0];
            return true;
        }

        return false;
    }

    private static ExpenseEngineType? MapEngineTypeName(string engineTypeName)
    {
        foreach (var field in typeof(ExpenseEngineType).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var wireName = field.GetCustomAttribute<JsonStringEnumMemberNameAttribute>()?.Name ?? field.Name;
            if (string.Equals(wireName, engineTypeName, StringComparison.Ordinal))
            {
                return (ExpenseEngineType)field.GetValue(null)!;
            }
        }

        return null;
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
