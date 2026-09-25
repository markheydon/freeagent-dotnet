namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Configuration for loading Turpinverse canon JSON from GitHub.
/// </summary>
public sealed class TurpinverseCanonOptions
{
    public const string SectionName = "Turpinverse";

    /// <summary>
    /// GitHub repository in <c>owner/name</c> form.
    /// </summary>
    public string Repository { get; set; } = "markheydon/turpinverse";

    /// <summary>
    /// Git branch, tag, or commit SHA for canon files under <c>canon/</c>.
    /// </summary>
    public string CanonRef { get; set; } = "dc51006a80c9";
}
