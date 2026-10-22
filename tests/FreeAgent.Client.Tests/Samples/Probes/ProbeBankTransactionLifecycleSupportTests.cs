#if NET10_0
using FreeAgent.Client;
using FreeAgent.Client.Samples.Shared.Probes;

namespace FreeAgent.Client.Tests.Samples.Probes;

public class ProbeBankTransactionLifecycleSupportTests
{
    [Fact]
    public void CreateProbeFitId_IncludesPrefixAndUniqueSuffix()
    {
        var first = ProbeBankTransactionLifecycleSupport.CreateProbeFitId("sdk-console-bte");
        var second = ProbeBankTransactionLifecycleSupport.CreateProbeFitId("sdk-console-bte");

        Assert.StartsWith("sdk-console-bte-", first, StringComparison.Ordinal);
        Assert.StartsWith("sdk-console-bte-", second, StringComparison.Ordinal);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void IsUnexplainedForProbe_ReturnsTrueWhenMagnitudeMatches()
    {
        var detail = new BankTransaction { UnexplainedAmount = -0.01m };

        Assert.True(ProbeBankTransactionLifecycleSupport.IsUnexplainedForProbe(detail, -0.01m));
        Assert.True(ProbeBankTransactionLifecycleSupport.IsUnexplainedForProbe(detail, 0.01m));
    }

    [Fact]
    public void IsUnexplainedForProbe_ReturnsFalseWhenFullyExplained()
    {
        var detail = new BankTransaction { UnexplainedAmount = 0m };

        Assert.False(ProbeBankTransactionLifecycleSupport.IsUnexplainedForProbe(detail, -0.01m));
    }

    [Fact]
    public void IsUnexplainedForProbe_ReturnsFalseWhenMagnitudeDiffers()
    {
        var detail = new BankTransaction { UnexplainedAmount = -0.02m };

        Assert.False(ProbeBankTransactionLifecycleSupport.IsUnexplainedForProbe(detail, -0.01m));
    }
}
#endif
