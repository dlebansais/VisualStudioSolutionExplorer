namespace SlnExplorer;

using System.Diagnostics;

/// <summary>
/// Defines a framework supported by the project.
/// </summary>
[DebuggerDisplay("{Name}")]
#pragma warning disable CA1724 // Type names should not match namespaces
public class Framework
#pragma warning restore CA1724 // Type names should not match namespaces
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Framework"/> class.
    /// </summary>
    /// <param name="name">The framework namr.</param>
    /// <param name="type">The framework type.</param>
    /// <param name="major">The framework major version.</param>
    /// <param name="minor">The framework minor version.</param>
    public Framework(string name, FrameworkType type, int major, int minor)
    {
        Name = name;
        Type = type;
        Major = major;
        Minor = minor;
        Moniker = FrameworkMoniker.none;
        MonikerMajor = -1;
        MonikerMinor = -1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Framework"/> class.
    /// </summary>
    /// <param name="name">The framework namr.</param>
    /// <param name="type">The framework type.</param>
    /// <param name="major">The framework major version.</param>
    /// <param name="minor">The framework minor version.</param>
    /// <param name="moniker">The target framework moniker (TFM).</param>
    public Framework(string name, FrameworkType type, int major, int minor, FrameworkMoniker moniker)
    {
        Name = name;
        Type = type;
        Major = major;
        Minor = minor;
        Moniker = moniker;
        MonikerMajor = -1;
        MonikerMinor = -1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Framework"/> class.
    /// </summary>
    /// <param name="name">The framework namr.</param>
    /// <param name="type">The framework type.</param>
    /// <param name="major">The framework major version.</param>
    /// <param name="minor">The framework minor version.</param>
    /// <param name="moniker">The target framework moniker (TFM).</param>
    /// <param name="monikerMajor">The moniker major version.</param>
    /// <param name="monikerMinor">The moniker minor version.</param>
    public Framework(string name, FrameworkType type, int major, int minor, FrameworkMoniker moniker, int monikerMajor, int monikerMinor)
    {
        Name = name;
        Type = type;
        Major = major;
        Minor = minor;
        Moniker = moniker;
        MonikerMajor = monikerMajor;
        MonikerMinor = monikerMinor;
    }

    /// <summary>
    /// Gets the framework type.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the framework type.
    /// </summary>
    public FrameworkType Type { get; init; }

    /// <summary>
    /// Gets the framework major version.
    /// </summary>
    public int Major { get; init; }

    /// <summary>
    /// Gets the framework minor version.
    /// </summary>
    public int Minor { get; init; }

    /// <summary>
    /// Gets the target framework moniker (TFM).
    /// </summary>
    public FrameworkMoniker Moniker { get; init; }

    /// <summary>
    /// Gets the moniker major version.
    /// </summary>
    public int MonikerMajor { get; init; }

    /// <summary>
    /// Gets the moniker minor version.
    /// </summary>
    public int MonikerMinor { get; init; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>The current object as a string.</returns>
    public override string ToString() => Name;
}
