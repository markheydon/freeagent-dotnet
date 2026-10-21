namespace FreeAgent.Client;

/// <summary>
/// Attributes for creating a landlord property.
/// </summary>
public sealed class CreatePropertyRequest
{
    /// <summary>
    /// First line of the property address.
    /// </summary>
    public required string Address1 { get; set; }

    /// <summary>
    /// Second line of the property address.
    /// </summary>
    public string? Address2 { get; set; }

    /// <summary>
    /// Third line of the property address.
    /// </summary>
    public string? Address3 { get; set; }

    /// <summary>
    /// Town of the property address.
    /// </summary>
    public string? Town { get; set; }

    /// <summary>
    /// Region of the property address.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Postcode of the property address.
    /// </summary>
    public string? Postcode { get; set; }
}
