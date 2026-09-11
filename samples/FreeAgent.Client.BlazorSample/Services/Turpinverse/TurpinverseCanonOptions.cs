namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

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
    public string CanonRef { get; set; } = "main";

    /// <summary>
    /// When <see langword="true"/>, read bundled fallback canon under <c>Data/canon-fallback/</c>
    /// before attempting GitHub. When GitHub is unreachable, fallback files are used automatically if present.
    /// </summary>
    public bool PreferLocalFallback { get; set; }
}
