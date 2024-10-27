namespace OLT.Utility.AssemblyScanner;

/// <summary>
/// Extensions for <seealso cref="OltAssemblyScanBuilder"/>
/// </summary>
public static class OltAssemblyScanBuilderExtensions
{
    /// <summary>
    /// Adds "Microsoft.", "mscorlib", "netstandard", "Swashbuckle", "System.", "Windows."
    /// </summary>
    /// <returns></returns>
    public static OltAssemblyScanBuilder ExcludeMicrosoft(this OltAssemblyScanBuilder builder)
    {
        builder.ExcludeFilter("Microsoft.", "mscorlib", "netstandard", "Swashbuckle", "System.", "Windows.");
        return builder;
    }

    /// <summary>
    /// Adds "Automapper"
    /// </summary>
    /// <returns></returns>
    public static OltAssemblyScanBuilder ExcludeAutomapper(this OltAssemblyScanBuilder builder)
    {
        builder.ExcludeFilter("Automapper");
        return builder;
    }
}
