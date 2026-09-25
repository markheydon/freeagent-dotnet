#if NET10_0
using FreeAgent.Client.ConsoleSample;

namespace FreeAgent.Client.Tests.Samples.ConsoleSample;

public class ConsoleRunOptionsTests
{
    [Fact]
    public void Parse_RunAll_SetsRunAll()
    {
        var options = ConsoleRunOptions.Parse(["--run-all"]);

        Assert.True(options.RunAll);
        Assert.Null(options.CategoryFilter);
    }

    [Fact]
    public void Parse_CategoryWithoutRunAll_Throws()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => ConsoleRunOptions.Parse(["--category", "Contacts"]));

        Assert.Contains("--run-all", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Parse_CategoryWithRunAll_SetsCategoryFilter()
    {
        var options = ConsoleRunOptions.Parse(["--run-all", "--category", "Contacts"]);

        Assert.True(options.RunAll);
        Assert.Equal("Contacts", options.CategoryFilter);
    }

    [Fact]
    public void Parse_CategoryWithoutValue_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => ConsoleRunOptions.Parse(["--category"]));
    }

    [Fact]
    public void Parse_BootstrapRefreshToken_SetsBootstrapRefreshToken()
    {
        var options = ConsoleRunOptions.Parse(["--bootstrap-refresh-token"]);

        Assert.True(options.BootstrapRefreshToken);
        Assert.False(options.RunAll);
    }

    [Fact]
    public void Parse_BootstrapRefreshTokenWithRunAll_Throws()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => ConsoleRunOptions.Parse(["--bootstrap-refresh-token", "--run-all"]));

        Assert.Contains("--run-all", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Parse_AllowProductionWrites_SetsAllowProductionWrites()
    {
        var options = ConsoleRunOptions.Parse(["--allow-production-writes"]);

        Assert.True(options.AllowProductionWrites);
        Assert.False(options.RunAll);
    }

    [Fact]
    public void Parse_AllowProductionWritesWithRunAll_Throws()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => ConsoleRunOptions.Parse(["--allow-production-writes", "--run-all"]));

        Assert.Contains("--run-all", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}

#endif
