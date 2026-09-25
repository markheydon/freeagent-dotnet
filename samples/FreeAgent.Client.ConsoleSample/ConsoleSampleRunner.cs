using System.Reflection;
using System.Text.RegularExpressions;
using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample.Samples;
using Microsoft.Extensions.DependencyInjection;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Discovers attributed SDK examples and runs the interactive menu or batch mode.
/// </summary>
internal sealed class ConsoleSampleRunner
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IConsoleSampleProvider> _providers;
    private readonly SampleRuntimeContext _runtime;
    private readonly List<ConsoleSampleEntry> _entries = [];

    /// <summary>
    /// Initialises the runner and discovers registered examples.
    /// </summary>
    /// <param name="serviceProvider">Application service provider.</param>
    /// <param name="providers">Attributed sample provider instances.</param>
    /// <param name="runtime">Resolved environment and write-guard settings.</param>
    public ConsoleSampleRunner(
        IServiceProvider serviceProvider,
        IEnumerable<IConsoleSampleProvider> providers,
        SampleRuntimeContext runtime)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
        DiscoverSamples();
    }

    /// <summary>
    /// Whether any discovered example mutates API data.
    /// </summary>
    public bool HasMutatingExamples => _entries.Exists(e => e.MutatesData);

    /// <summary>
    /// Returns discovered examples for unit tests.
    /// </summary>
    internal IReadOnlyList<DiscoveredConsoleSample> GetDiscoveredSamples() =>
        _entries
            .Select(e => new DiscoveredConsoleSample(e.Category, e.Name, e.IncludeInRunAll, e.MutatesData))
            .ToList();

    /// <summary>
    /// Invokes a discovered example by name for unit tests.
    /// </summary>
    /// <param name="name">Example display name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    internal Task InvokeSampleByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entry = _entries.FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"Sample '{name}' was not discovered.");

        return RunEntryAsync(entry, cancellationToken);
    }

    /// <summary>
    /// Runs the interactive menu loop.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see langword="true"/> when the user chose to quit.</returns>
    public async Task<bool> RunInteractiveAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine();
            Console.WriteLine("=== FreeAgent SDK Console Sample ===");

            var categories = _entries.Select(e => e.Category).Distinct().ToList();
            for (var i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }

            Console.WriteLine("0. Quit");
            Console.WriteLine();
            Console.Write("Enter category number: ");

            var input = await Console.In.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            if (input is "0" or "q" or "Q")
            {
                Console.WriteLine("Goodbye.");
                return true;
            }

            if (!int.TryParse(input, out var categoryChoice) || categoryChoice < 1 || categoryChoice > categories.Count)
            {
                Console.WriteLine("Invalid selection.");
                continue;
            }

            var selectedCategory = categories[categoryChoice - 1];
            if (await RunCategoryMenuAsync(selectedCategory, cancellationToken))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Runs all examples sequentially and prints a dotnet test-style summary.
    /// </summary>
    /// <param name="options">Run options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Process exit code.</returns>
    public async Task<int> RunAllAsync(ConsoleRunOptions options, CancellationToken cancellationToken = default)
    {
        var reporter = new SampleRunReporter();
        var entries = FilterEntries(options.CategoryFilter)
            .Where(e => e.IncludeInRunAll)
            .ToList();

        if (entries.Count == 0)
        {
            Console.WriteLine("No examples matched the requested filter.");
            return 1;
        }

        Console.WriteLine();
        Console.WriteLine($"Running {entries.Count} example(s)...");
        Console.WriteLine();

        foreach (var entry in entries)
        {
            await reporter.RunExampleAsync(
                entry.Category,
                entry.Name,
                ct => RunEntryAsync(entry, ct),
                retryOnRateLimit: true,
                cancellationToken);
        }

        return reporter.WriteSummaryAndGetExitCode();
    }

    private async Task<bool> RunCategoryMenuAsync(string category, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var items = _entries.Where(e => e.Category == category).ToList();
            Console.WriteLine();
            Console.WriteLine($"--- {category} ---");

            for (var i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {items[i].Name}");
            }

            Console.WriteLine("0. Back");
            Console.WriteLine("q. Quit");
            Console.WriteLine();
            Console.Write("Enter number to run: ");

            var input = await Console.In.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            if (input is "0")
            {
                return false;
            }

            if (input is "q" or "Q")
            {
                Console.WriteLine("Goodbye.");
                return true;
            }

            if (!int.TryParse(input, out var choice) || choice < 1 || choice > items.Count)
            {
                Console.WriteLine("Invalid selection.");
                continue;
            }

            var selected = items[choice - 1];
            Console.WriteLine();
            Console.WriteLine($"--- Running: {selected.Name} ---");
            Console.WriteLine();

            try
            {
                await RunEntryAsync(selected, cancellationToken);
            }
            catch (SampleSkippedException ex)
            {
                Console.WriteLine($"Skipped: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine($"--- Finished: {selected.Name} ---");
            Console.WriteLine("Press any key to return to the category menu...");
            Console.ReadKey(intercept: true);
        }

        return false;
    }

    private IEnumerable<ConsoleSampleEntry> FilterEntries(string? categoryFilter)
    {
        if (string.IsNullOrWhiteSpace(categoryFilter))
        {
            return _entries;
        }

        return _entries.Where(
            e => string.Equals(e.Category, categoryFilter, StringComparison.OrdinalIgnoreCase));
    }

    private void DiscoverSamples()
    {
        var registeredNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var provider in _providers)
        {
            var providerType = provider.GetType();
            var classAttribute = providerType.GetCustomAttribute<ConsoleSamplesAttribute>();
            var defaultCategory = classAttribute?.Category ?? "Misc";

            var methods = providerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<ConsoleSampleAttribute>() is not null);

            foreach (var method in methods)
            {
                var methodAttribute = method.GetCustomAttribute<ConsoleSampleAttribute>()!;
                var name = string.IsNullOrWhiteSpace(methodAttribute.Name)
                    ? DeriveDisplayName(method.Name)
                    : methodAttribute.Name;

                var category = string.IsNullOrWhiteSpace(methodAttribute.Category)
                    ? defaultCategory
                    : methodAttribute.Category;

                if (!registeredNames.Add(name))
                {
                    throw new InvalidOperationException(
                        $"Duplicate console sample name '{name}' in {providerType.Name}.{method.Name}.");
                }

                _entries.Add(new ConsoleSampleEntry(
                    category,
                    name,
                    !methodAttribute.ExcludeFromRunAll,
                    methodAttribute.MutatesData,
                    CreateAction(provider, method)));
            }
        }

        _entries.Sort((left, right) =>
        {
            var categoryCompare = string.Compare(left.Category, right.Category, StringComparison.OrdinalIgnoreCase);
            return categoryCompare != 0
                ? categoryCompare
                : string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);
        });
    }

    private static Func<IServiceProvider, CancellationToken, Task> CreateAction(object provider, MethodInfo method)
    {
        if (method.ReturnType != typeof(Task))
        {
            throw new InvalidOperationException(
                $"Sample method {provider.GetType().Name}.{method.Name} must return Task.");
        }

        var parameters = method.GetParameters();
        if (parameters.Length > 1
            || (parameters.Length == 1 && parameters[0].ParameterType != typeof(CancellationToken)))
        {
            throw new InvalidOperationException(
                $"Sample method {provider.GetType().Name}.{method.Name} must be parameterless or accept CancellationToken only.");
        }

        return async (services, cancellationToken) =>
        {
            using var scope = services.CreateScope();
            var scopedProvider = ResolveProvider(scope.ServiceProvider, provider.GetType());
            object?[] invokeArgs = parameters.Length == 0
                ? []
                : [cancellationToken];

            var task = method.Invoke(scopedProvider, invokeArgs) as Task
                ?? throw new InvalidOperationException($"Sample method {method.Name} did not return a Task.");

            await task;
        };
    }

    private static object ResolveProvider(IServiceProvider scope, Type providerType) =>
        scope.GetService(providerType)
        ?? throw new InvalidOperationException($"Could not resolve sample provider {providerType.Name}.");

    private async Task RunEntryAsync(ConsoleSampleEntry entry, CancellationToken cancellationToken)
    {
        if (entry.MutatesData)
        {
            SandboxWriteGuard.EnsureMutatingExampleAllowed(
                _runtime.Environment,
                _runtime.AllowProductionWrites);

            if (_runtime.Environment != FreeAgentEnvironment.Sandbox && _runtime.AllowProductionWrites)
            {
                Console.WriteLine(
                    "Warning: Production writes enabled — this example will modify live account data.");
                Console.WriteLine();
            }
        }

        await entry.Action(_serviceProvider, cancellationToken);
    }

    private static string DeriveDisplayName(string methodName) =>
        DisplayNameRegex.Replace(methodName, "$1 $2").Trim();

    private static readonly Regex DisplayNameRegex = new("([a-z])([A-Z])", RegexOptions.Compiled);

    private sealed record ConsoleSampleEntry(
        string Category,
        string Name,
        bool IncludeInRunAll,
        bool MutatesData,
        Func<IServiceProvider, CancellationToken, Task> Action);
}

/// <summary>
/// Discovered console sample metadata exposed for unit tests.
/// </summary>
/// <param name="Category">Example category.</param>
/// <param name="Name">Example name.</param>
/// <param name="IncludeInRunAll">Whether the example runs in <c>--run-all</c> mode.</param>
/// <param name="MutatesData">Whether the example mutates API data.</param>
internal sealed record DiscoveredConsoleSample(
    string Category,
    string Name,
    bool IncludeInRunAll,
    bool MutatesData);
