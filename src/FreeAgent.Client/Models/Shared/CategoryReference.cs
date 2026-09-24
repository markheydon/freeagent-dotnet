using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Typed reference to a FreeAgent category resource URI.
/// </summary>
/// <remarks>
/// Categories are keyed by nominal code on the wire, not a numeric resource identifier.
/// Use <see cref="NominalCode"/> for API calls such as <c>GetCategoryAsync</c>.
/// </remarks>
[JsonConverter(typeof(CategoryReferenceJsonConverter))]
public readonly record struct CategoryReference
{
    /// <summary>
    /// Initialises a category reference.
    /// </summary>
    /// <param name="uri">Category resource URI.</param>
    /// <param name="nominalCode">Category nominal code from the URI path.</param>
    public CategoryReference(string uri, string nominalCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        Uri = uri;
        NominalCode = nominalCode;
    }

    /// <summary>
    /// Category resource URI.
    /// </summary>
    public string Uri { get; }

    /// <summary>
    /// Category nominal code parsed from the URI path.
    /// </summary>
    public string NominalCode { get; }

    /// <summary>
    /// Parses a category reference from a resource URI.
    /// </summary>
    /// <param name="uri">Category resource URI.</param>
    /// <returns>Parsed category reference.</returns>
    public static CategoryReference Parse(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        FreeAgentResourceId.ValidateResourceSegment(uri, "categories");

        var nominalCode = ExtractNominalCode(uri);
        return new CategoryReference(uri, nominalCode);
    }

    /// <summary>
    /// Creates a category reference for the selected API environment.
    /// </summary>
    /// <param name="environment">Target API environment.</param>
    /// <param name="nominalCode">Category nominal code.</param>
    /// <returns>Category reference.</returns>
    public static CategoryReference ForEnvironment(FreeAgentEnvironment environment, string nominalCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);

        var uri = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}categories/{nominalCode}";
        return new CategoryReference(uri, nominalCode);
    }

    /// <summary>
    /// Implicit conversion to the wire URI string.
    /// </summary>
    /// <param name="reference">Category reference.</param>
    public static implicit operator string(CategoryReference reference) => reference.Uri;

    internal static string ExtractNominalCode(string uri)
    {
        var segments = uri.TrimEnd('/').Split('/');
        if (segments.Length == 0)
        {
            throw new ArgumentException("The URI does not contain a category nominal code.", nameof(uri));
        }

        var nominalCode = segments[^1];
        if (string.IsNullOrWhiteSpace(nominalCode))
        {
            throw new ArgumentException("The URI does not contain a category nominal code.", nameof(uri));
        }

        return nominalCode;
    }
}
