#if NET10_0
using System.Reflection;
using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample;
using FreeAgent.Client.ConsoleSample.Samples;
using Microsoft.Extensions.DependencyInjection;

namespace FreeAgent.Client.Tests.Samples.ConsoleSample;

public class ConsoleSampleClassificationTests
{
    private const int ExpectedExampleCount = 120;
    private const int ExpectedMutatingCount = 65;

    [Fact]
    public void DiscoverSamples_RegistersExpectedCounts()
    {
        var runner = CreateRunner(FreeAgentEnvironment.Sandbox, allowProductionWrites: false);
        var entries = runner.GetDiscoveredSamples();

        Assert.Equal(ExpectedExampleCount, entries.Count);
        Assert.Equal(ExpectedMutatingCount, entries.Count(e => e.MutatesData));
    }

    [Fact]
    public void DiscoverSamples_MutatingMethodsHaveMutatesDataAttribute()
    {
        var assembly = typeof(DependencyInjection).Assembly;
        var providerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetCustomAttribute<ConsoleSamplesAttribute>() is not null);

        foreach (var providerType in providerTypes)
        {
            var methods = providerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<ConsoleSampleAttribute>() is not null);

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<ConsoleSampleAttribute>()!;
                var name = string.IsNullOrWhiteSpace(attribute.Name) ? method.Name : attribute.Name;

                if (LooksMutating(name))
                {
                    Assert.True(attribute.MutatesData, $"Expected MutatesData on {providerType.Name}.{method.Name} ({name}).");
                }
            }
        }
    }

    private static bool LooksMutating(string name) =>
        name.Contains("Create", StringComparison.OrdinalIgnoreCase)
        || name.Contains("Update", StringComparison.OrdinalIgnoreCase)
        || name.Contains("Delete", StringComparison.OrdinalIgnoreCase)
        || name.Contains("Mark ", StringComparison.OrdinalIgnoreCase)
        || name.Contains("Send ", StringComparison.OrdinalIgnoreCase)
        || name.Contains("Duplicate", StringComparison.OrdinalIgnoreCase)
        || name.Contains("Convert", StringComparison.OrdinalIgnoreCase)
        || name.Contains("Start and stop", StringComparison.OrdinalIgnoreCase)
        || name.Contains("Take direct debit", StringComparison.OrdinalIgnoreCase);

    private static ConsoleSampleRunner CreateRunner(FreeAgentEnvironment environment, bool allowProductionWrites)
    {
        var services = new ServiceCollection();
        services.AddSingleton(new FreeAgentClient("test-token", environment));
        services.AddSingleton(new SampleRuntimeContext(environment, allowProductionWrites));
        services.AddSingleton<SampleContext>();
        services.AddConsoleSamples();

        using var scope = services.BuildServiceProvider().CreateScope();
        return scope.ServiceProvider.GetRequiredService<ConsoleSampleRunner>();
    }
}

#endif
