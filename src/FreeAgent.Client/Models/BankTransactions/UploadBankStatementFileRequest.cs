namespace FreeAgent.Client;

/// <summary>
/// File statement upload (OFX, QIF, or a supported CSV).
/// </summary>
public sealed class UploadBankStatementFileRequest
{
    /// <summary>
    /// Creates a file statement upload.
    /// </summary>
    /// <param name="statement">Readable statement file contents.</param>
    /// <param name="fileName">File name sent as the multipart <c>statement</c> part.</param>
    public UploadBankStatementFileRequest(Stream statement, string fileName)
    {
        ArgumentNullException.ThrowIfNull(statement);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        if (!statement.CanRead)
        {
            throw new ArgumentException("Statement stream must be readable.", nameof(statement));
        }

        Statement = statement;
        FileName = fileName;
    }

    /// <summary>
    /// Statement file contents.
    /// </summary>
    public Stream Statement { get; }

    /// <summary>
    /// File name for the multipart <c>statement</c> part.
    /// </summary>
    public string FileName { get; }
}
