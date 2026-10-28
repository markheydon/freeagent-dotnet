namespace FreeAgent.Client;

/// <summary>
/// FreeAgent UI billability category for a timeslip, derived from documented wire fields only.
/// </summary>
/// <remarks>
/// Maps to labels shown in the FreeAgent web UI for time entries. This is not a separate API resource.
/// </remarks>
public enum TimeslipBillabilityCategory
{
    /// <summary>
    /// Task billability could not be determined from the supplied models (for example <c>is_billable</c> was omitted and the task was not nested or hydrated on the timeslip).
    /// </summary>
    Unknown,

    /// <summary>
    /// UI label <c>Unbillable</c>: linked task <c>is_billable</c> is <see langword="false"/>.
    /// </summary>
    Unbillable,

    /// <summary>
    /// UI label <c>Billable</c>: linked task is billable and <c>billed_on_invoice</c> is not set.
    /// </summary>
    Unbilled,

    /// <summary>
    /// UI label <c>Billed</c>: linked task is billable and <c>billed_on_invoice</c> is present.
    /// </summary>
    Billed
}

/// <summary>
/// Composes FreeAgent UI billability for a timeslip from already-fetched models (no HTTP).
/// </summary>
public static class TimeslipBillability
{
    /// <summary>
    /// Resolves the billability category using the timeslip and optional linked task details.
    /// </summary>
    /// <param name="timeslip">Timeslip response (required).</param>
    /// <param name="task">
    /// Linked task when already loaded; when <see langword="null"/>, uses <see cref="Timeslip.ProjectTask"/> from nested or hydrated responses.
    /// </param>
    /// <returns>
    /// <see cref="TimeslipBillabilityCategory.Unknown"/> when task billability cannot be determined without an extra API call.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="timeslip"/> is <see langword="null"/>.</exception>
    public static TimeslipBillabilityCategory Resolve(Timeslip timeslip, ProjectTask? task = null)
    {
        ArgumentNullException.ThrowIfNull(timeslip);

        task ??= timeslip.ProjectTask;

        if (task?.IsBillable == false)
        {
            return TimeslipBillabilityCategory.Unbillable;
        }

        if (task?.IsBillable != true)
        {
            return TimeslipBillabilityCategory.Unknown;
        }

        return HasBilledOnInvoice(timeslip)
            ? TimeslipBillabilityCategory.Billed
            : TimeslipBillabilityCategory.Unbilled;
    }

    private static bool HasBilledOnInvoice(Timeslip timeslip) =>
        timeslip.BilledOnInvoiceId is > 0;
}
