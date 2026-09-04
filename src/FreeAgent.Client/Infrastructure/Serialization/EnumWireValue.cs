using System.Reflection;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Resolves enum wire names from <see cref="JsonStringEnumMemberNameAttribute"/> values.
/// </summary>
internal static class EnumWireValue
{
    /// <summary>
    /// Returns the JSON wire name for an enum value, using <see cref="JsonStringEnumMemberNameAttribute"/> when present.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The enum value.</param>
    /// <returns>The wire name for serialisation.</returns>
    public static string Get<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        var fieldInfo = typeof(TEnum).GetField(value.ToString(), BindingFlags.Public | BindingFlags.Static);
        var customName = fieldInfo?.GetCustomAttribute<JsonStringEnumMemberNameAttribute>()?.Name;

        return string.IsNullOrWhiteSpace(customName) ? value.ToString() : customName;
    }
}
