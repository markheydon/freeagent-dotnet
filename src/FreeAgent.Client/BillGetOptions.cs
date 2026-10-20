namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Bills.BillService.GetBillAsync(long, BillGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class BillGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the contact when the bill response contains only a contact URI.
    /// </summary>
    public bool IncludeContact { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the project when the bill response contains only a project URI.
    /// </summary>
    public bool IncludeProject { get; init; }
}
