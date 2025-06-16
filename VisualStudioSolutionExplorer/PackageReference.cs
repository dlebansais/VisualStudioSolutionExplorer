namespace SlnExplorer;

using System.Diagnostics;

/// <summary>
/// Reads and parses a project file.
/// </summary>
/// <param name="Project">Gets the project.</param>
/// <param name="Name">Gets the package name.</param>
/// <param name="Version">Gets the package version.</param>
/// <param name="Condition">Gets the package condition.</param>
/// <param name="IsAllPrivateAssets">Indicate whether all assets are private.</param>
[DebuggerDisplay("{Name} Version {Version}")]
public record PackageReference(Project Project, string Name, string Version, string Condition, bool IsAllPrivateAssets);
