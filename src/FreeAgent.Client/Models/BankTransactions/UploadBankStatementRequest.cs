namespace FreeAgent.Client;

/// <summary>
/// JSON statement upload: an array of bank transaction lines.
/// </summary>
public sealed class UploadBankStatementRequest
{
    /// <summary>
    /// Creates a statement upload from transaction lines.
    /// </summary>
    /// <param name="statement">Transaction lines. FreeAgent requires at least one line.</param>
    public UploadBankStatementRequest(IReadOnlyList<BankStatementTransaction> statement)
    {
        ArgumentNullException.ThrowIfNull(statement);

        if (statement.Count == 0)
        {
            throw new ArgumentException(
                "A statement must contain at least one transaction.",
                nameof(statement));
        }

        Statement = statement;
    }

    /// <summary>
    /// Transaction lines sent as the <c>statement</c> array.
    /// </summary>
    public IReadOnlyList<BankStatementTransaction> Statement { get; }
}
