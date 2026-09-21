namespace FreeAgent.Client.BlazorSample.Services;

/// <summary>
/// Shared helpers for sample probe pages backed by primitive string collections.
/// </summary>
public static class StringListProbeSupport
{
    /// <summary>
    /// One row in a string-list diagnostics table.
    /// </summary>
    public sealed record Row(int Index, string Value);

    /// <summary>
    /// Builds table rows from a loaded string collection.
    /// </summary>
    public static IReadOnlyList<Row> BuildRows(IReadOnlyList<string>? values) =>
        values?.Select(static (value, index) => new Row(index, value)).ToList() ?? [];

    /// <summary>
    /// Builds wire-to-model diagnostics for one string collection item.
    /// </summary>
    public static ModelProbeSnapshot BuildProbeSnapshot(
        string value,
        string wirePayload,
        string arrayPropertyName,
        int index) =>
        ModelWireDiagnostics.BuildStringArrayItemFromWirePayload(value, wirePayload, arrayPropertyName, index);
}
