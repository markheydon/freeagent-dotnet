namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Marks a class as a provider of console SDK examples.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class ConsoleSamplesAttribute : Attribute
{
    /// <summary>
    /// Default category name for examples in this class.
    /// </summary>
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Marks a method that should appear in the console sample menu.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class ConsoleSampleAttribute : Attribute
{
    /// <summary>
    /// Friendly menu name. When empty, the method name is derived automatically.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category override. When empty, the class-level <see cref="ConsoleSamplesAttribute.Category"/> is used.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// When <see langword="true"/>, the example is omitted from <c>--run-all</c> smoke runs.
    /// </summary>
    public bool ExcludeFromRunAll { get; set; }
}

/// <summary>
/// Marker interface for DI registration of example provider classes.
/// </summary>
internal interface IConsoleSampleProvider;
