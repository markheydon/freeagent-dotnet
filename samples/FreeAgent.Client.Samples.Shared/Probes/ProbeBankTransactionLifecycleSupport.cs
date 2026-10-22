using FreeAgent.Client;

namespace FreeAgent.Client.Samples.Shared.Probes;

/// <summary>
/// Upload and cleanup helpers for live bank transaction smoke probes.
/// </summary>
public static class ProbeBankTransactionLifecycleSupport
{
    /// <summary>
    /// Creates a unique FIT id for probe statement uploads.
    /// </summary>
    public static string CreateProbeFitId(string prefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        return $"{prefix}-{Guid.NewGuid():N}";
    }

    /// <summary>
    /// Returns <see langword="true"/> when the transaction is unexplained and matches the uploaded statement amount.
    /// </summary>
    public static bool IsUnexplainedForProbe(BankTransaction detail, decimal expectedStatementAmount)
    {
        ArgumentNullException.ThrowIfNull(detail);

        if (detail.UnexplainedAmount is not { } unexplained || unexplained == 0m)
        {
            return false;
        }

        var expectedMagnitude = Math.Abs(expectedStatementAmount);
        return Math.Abs(unexplained) == expectedMagnitude;
    }

    /// <summary>
    /// Deletes explanations on a probe transaction, then deletes the transaction. Ignores cleanup API failures.
    /// </summary>
    public static async Task DeleteExplanationsAndBankTransactionAsync(
        FreeAgentClient client,
        long bankTransactionId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (bankTransactionId <= 0)
        {
            return;
        }

        try
        {
            var detail = await client.BankTransactions.GetBankTransactionAsync(bankTransactionId, cancellationToken);
            if (detail.BankTransactionExplanations is { Count: > 0 } explanations)
            {
                foreach (var explanation in explanations)
                {
                    if (explanation.ResourceId <= 0)
                    {
                        continue;
                    }

                    try
                    {
                        await client.BankTransactionExplanations.DeleteBankTransactionExplanationAsync(
                            explanation.ResourceId,
                            cancellationToken);
                    }
                    catch (FreeAgentApiException)
                    {
                        // Best-effort cleanup for probe fixtures.
                    }
                }
            }
        }
        catch (FreeAgentApiException)
        {
            // Transaction may already be gone; still attempt delete below.
        }

        try
        {
            await client.BankTransactions.DeleteBankTransactionAsync(bankTransactionId, cancellationToken);
        }
        catch (FreeAgentApiException)
        {
            // Explanation delete may have removed the transaction; ignore cleanup failures.
        }
    }

}
