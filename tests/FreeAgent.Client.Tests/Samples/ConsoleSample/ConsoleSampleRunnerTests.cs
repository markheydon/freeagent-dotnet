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
        var services = new ServiceCollection();
        services.AddSingleton(new FreeAgentClient("test-token"));
        services.AddSingleton<SampleContext>();
        services.AddConsoleSamples();

        using var scope = services.BuildServiceProvider().CreateScope();

        var exception = Record.Exception(() => scope.ServiceProvider.GetRequiredService<ConsoleSampleRunner>());

        Assert.Null(exception);
    }
}

#endif
