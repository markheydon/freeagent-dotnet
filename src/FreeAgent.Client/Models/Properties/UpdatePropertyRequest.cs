namespace FreeAgent.Client.Models.Properties;

/// <summary>
/// Attributes for updating a landlord property.
/// </summary>
public sealed class UpdatePropertyRequest
{
    /// <summary>
    /// First line of the property address.
    /// </summary>
    public string? Address1 { get; set; }

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

    /// <summary>
    /// Country of the property address.
    /// </summary>
    public string? Country { get; set; }
}
