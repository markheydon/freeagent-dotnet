#if NET8_0
using System;

namespace System.Text.Json.Serialization;

/// <summary>
/// Polyfill of <c>System.Text.Json.Serialization.JsonStringEnumMemberNameAttribute</c>
/// for .NET 8, where it is not available in the BCL.
/// Provides the same contract as the .NET 10 attribute so reflection-based code works
/// identically across both target frameworks.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class JsonStringEnumMemberNameAttribute : Attribute
{
    /// <summary>Initialises the attribute with the wire name to use during JSON serialisation.</summary>
    /// <param name="name">The JSON member name for this enum field.</param>
    public JsonStringEnumMemberNameAttribute(string name)
    {
        Name = name;
    }

    /// <summary>The JSON member name used for this enum field.</summary>
    public string Name { get; }
}
#endif
