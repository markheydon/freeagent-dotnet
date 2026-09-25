#if NET10_0
using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample;
using FreeAgent.Client.ConsoleSample.Samples;
using Microsoft.Extensions.DependencyInjection;

namespace FreeAgent.Client.Tests.Samples.ConsoleSample;

public class ConsoleSampleRunnerTests
{
    [Fact]
    public void Constructor_DiscoversSamplesWithoutDuplicateNames()
    {
        var services = CreateServices(FreeAgentEnvironment.Sandbox, allowProductionWrites: false);

        using var scope = services.BuildServiceProvider().CreateScope();

        var exception = Record.Exception(() => scope.ServiceProvider.GetRequiredService<ConsoleSampleRunner>());

        Assert.Null(exception);
    }

    [Fact]
    public async Task InvokeSampleByNameAsync_MutatingOnProductionWithoutHatch_ThrowsSampleSkippedException()
    {
        var services = CreateServices(FreeAgentEnvironment.Production, allowProductionWrites: false);

        using var scope = services.BuildServiceProvider().CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<ConsoleSampleRunner>();

        await Assert.ThrowsAsync<SampleSkippedException>(
            () => runner.InvokeSampleByNameAsync("Update contact organisation name"));
    }

    [Fact]
    public async Task InvokeSampleByNameAsync_ReadOnlyOnProduction_DoesNotThrowGuardException()
    {
        var services = CreateServices(FreeAgentEnvironment.Production, allowProductionWrites: false);

        using var scope = services.BuildServiceProvider().CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<ConsoleSampleRunner>();

        var exception = await Record.ExceptionAsync(
            () => runner.InvokeSampleByNameAsync("List active contacts"));

        Assert.IsNotType<SampleSkippedException>(exception);
    }

    private static ServiceCollection CreateServices(FreeAgentEnvironment environment, bool allowProductionWrites)
    {
        var services = new ServiceCollection();
        services.AddSingleton(new FreeAgentClient("test-token", environment));
        services.AddSingleton(new SampleRuntimeContext(environment, allowProductionWrites));
        services.AddSingleton<SampleContext>();
        services.AddConsoleSamples();
        return services;
    }
}

#endif
