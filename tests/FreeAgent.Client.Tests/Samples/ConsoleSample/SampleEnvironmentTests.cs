#if NET10_0
using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample;

namespace FreeAgent.Client.Tests.Samples.ConsoleSample;

public class SampleEnvironmentTests
{
    [Fact]
    public void Resolve_Unset_ReturnsSandboxDefault()
    {
        var previous = Environment.GetEnvironmentVariable(SampleEnvironment.EnvironmentVariableName);
        try
        {
            Environment.SetEnvironmentVariable(SampleEnvironment.EnvironmentVariableName, null);

            Assert.Equal(FreeAgentEnvironment.Sandbox, SampleEnvironment.Resolve());
        }
        finally
        {
            Environment.SetEnvironmentVariable(SampleEnvironment.EnvironmentVariableName, previous);
        }
    }

    [Theory]
    [InlineData("Sandbox")]
    [InlineData("sandbox")]
    [InlineData("Production")]
    [InlineData("production")]
    public void Resolve_ValidName_ReturnsEnvironment(string value)
    {
        var previous = Environment.GetEnvironmentVariable(SampleEnvironment.EnvironmentVariableName);
        try
        {
            Environment.SetEnvironmentVariable(SampleEnvironment.EnvironmentVariableName, value);

            var expected = Enum.Parse<FreeAgentEnvironment>(value, ignoreCase: true);
            Assert.Equal(expected, SampleEnvironment.Resolve());
        }
        finally
        {
            Environment.SetEnvironmentVariable(SampleEnvironment.EnvironmentVariableName, previous);
        }
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("0")]
    [InlineData("1")]
    public void Resolve_InvalidValue_Throws(string value)
    {
        var previous = Environment.GetEnvironmentVariable(SampleEnvironment.EnvironmentVariableName);
        try
        {
            Environment.SetEnvironmentVariable(SampleEnvironment.EnvironmentVariableName, value);

            var exception = Assert.Throws<InvalidOperationException>(() => SampleEnvironment.Resolve());

            Assert.Contains(SampleEnvironment.EnvironmentVariableName, exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Environment.SetEnvironmentVariable(SampleEnvironment.EnvironmentVariableName, previous);
        }
    }
}

#endif
