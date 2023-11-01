namespace SlnExplorer;

using System.IO;

/// <summary>
/// Reads and parses a project file.
/// </summary>
public partial class Project
{
    /// <summary>
    /// Loads project details from a file.
    /// </summary>
    /// <param name="fileName">The path to the file.</param>
    public void LoadDetails(string fileName)
    {
        using FileStream Stream = new(fileName, FileMode.Open, FileAccess.Read);
        ParseProjectElements(Stream);
    }

    /// <summary>
    /// Loads project details from a stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public void LoadDetails(Stream stream)
    {
        ParseProjectElements(stream);
    }

    /// <summary>
    /// Checks a loaded for version consistency.
    /// </summary>
    /// <param name="warningOrErrorText">A warning or error text upon return.</param>
    /// <returns>True upon return if an error was found; otherwise, false.</returns>
    public bool CheckVersionConsistency(out string warningOrErrorText)
    {
        warningOrErrorText = string.Empty;
        bool HasErrors = false;

        if (HasVersion)
        {
            if (!Project.IsVersionCompatible(AssemblyVersion, Version))
            {
                HasErrors = true;
                warningOrErrorText = $"{AssemblyVersion} not compatible with {Version}";
            }

            if (!Project.IsVersionCompatible(FileVersion, Version))
            {
                HasErrors = true;
                warningOrErrorText = $"{FileVersion} not compatible with {Version}";
            }
        }
        else
            warningOrErrorText = "Ignored because no version";

        return HasErrors;
    }
}
