namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Resolves the repository-root <c>canon/</c> directory for Turpinverse snapshot files.
/// </summary>
internal static class TurpinverseCanonPaths
{
    public static string GetCanonDirectory(IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        foreach (var candidate in GetCandidateDirectories(environment))
        {
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException(
            $"Turpinverse canon directory not found. Checked: {string.Join(", ", GetCandidateDirectories(environment))}");
    }

    private static IEnumerable<string> GetCandidateDirectories(IWebHostEnvironment environment)
    {
        yield return Path.GetFullPath(Path.Combine(environment.ContentRootPath, "..", "..", "canon"));
        yield return Path.Combine(environment.ContentRootPath, "canon");
        yield return Path.Combine(AppContext.BaseDirectory, "canon");
    }
}
