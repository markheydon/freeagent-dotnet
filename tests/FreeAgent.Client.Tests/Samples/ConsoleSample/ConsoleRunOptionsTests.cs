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
}

#endif
