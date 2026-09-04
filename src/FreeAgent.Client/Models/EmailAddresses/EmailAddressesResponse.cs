using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.EmailAddresses;

/// <summary>
/// Wrapper for verified sender email addresses API responses.
/// </summary>
public class EmailAddressesResponse
{
    /// <summary>
    /// Verified sender email addresses formatted as <c>Name &lt;email@example.com&gt;</c>.
    /// </summary>
    [JsonPropertyName("email_addresses")]
    public List<string>? EmailAddresses { get; set; }
}
