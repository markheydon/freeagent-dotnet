using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.BlazorSample.Components;

/// <summary>
/// Formats typed list query parameters for probe-page display and diagnostics URLs.
/// </summary>
public static class ListQueryWire
{
    /// <summary>Returns the API wire value for a view filter enum.</summary>
    public static string View<T>(T view)
        where T : struct, Enum => EnumWireValue.Get(view);

    /// <summary>Returns the API wire value when a view filter is set.</summary>
    public static string? View<T>(T? view)
        where T : struct, Enum => view is null ? null : EnumWireValue.Get(view.Value);

    /// <summary>Returns the API wire value for a credit note view.</summary>
    public static string View(CreditNoteView view) => view.WireValue;

    /// <summary>Returns the API wire value when a credit note view is set.</summary>
    public static string? View(CreditNoteView? view) => view?.WireValue;
}
