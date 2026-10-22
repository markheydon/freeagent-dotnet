using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Account locks endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Account locks")]
internal sealed class AccountLocksSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List account locks")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListAccountLocksAsync(CancellationToken cancellationToken)
    {
        var result = await context.Client.AccountLocks.ListAsync(cancellationToken);

        SampleOutput.WriteHeader("Account locks");
        SampleOutput.WriteField("Count", result.AccountLocks.Count);
        SampleOutput.WriteField("Earliest user lock date", result.EarliestLockDate);
        SampleOutput.WriteField("Latest user lock date", result.LatestLockDate);

        if (result.AccountLocks.Count > 0)
        {
            var first = result.AccountLocks[0];
            SampleOutput.WriteField("First locked to", first.LockedToDate);
            SampleOutput.WriteField("User lock", first.UserLock);
        }
    }
}
