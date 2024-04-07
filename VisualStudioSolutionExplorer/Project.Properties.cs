namespace SlnExplorer;

using System;
using System.Collections.Generic;

/// <summary>
/// Reads and parses a project file.
/// </summary>
public partial class Project
{
    /// <summary>
    /// Gets the parent solution.
    /// </summary>
    public Solution ParentSolution { get; }

    /// <summary>
    /// Gets the project name.
    /// </summary>
    public string ProjectName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the project relative path.
    /// </summary>
    public string RelativePath { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the project GUID.
    /// </summary>
    public string ProjectGuid { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the project type.
    /// </summary>
    public ProjectType ProjectType { get; private set; }

    /// <summary>
    /// Gets the extension.
    /// </summary>
    public string Extension { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the dependencies.
    /// </summary>
    public IReadOnlyList<string> Dependencies { get; private set; } = new List<string>().AsReadOnly();

    /// <summary>
    /// Gets the project references.
    /// </summary>
    public IReadOnlyList<string> ProjectReferences { get; private set; } = new List<string>().AsReadOnly();

    /// <summary>
    /// Gets the parent project GUID.
    /// </summary>
    public string ParentProjectGuid { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the project configurations.
    /// </summary>
    public IReadOnlyList<Configuration> ProjectConfigurations { get; private set; } = new List<Configuration>().AsReadOnly();

    /// <summary>
    /// Gets the dependency level.
    /// </summary>
    public int DependencyLevel { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the project is a static library.
    /// </summary>
    public bool IsStaticLibrary { get; private set; }

    /// <summary>
    /// Gets the Sdk type.
    /// </summary>
    public SdkType SdkType { get; private set; }

    /// <summary>
    /// Gets the project output type.
    /// </summary>
    public string OutputType { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the projct uses WPF.
    /// </summary>
    public bool UseWpf { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the projct uses Windows Forms.
    /// </summary>
    public bool UseWindowsForms { get; private set; }

    /// <summary>
    /// Gets the project version.
    /// </summary>
    public string Version { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the project has a version.
    /// </summary>
    public bool HasVersion => Version.Length > 0;

    /// <summary>
    /// Gets the assembly version.
    /// </summary>
    public string AssemblyVersion { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the project has a valid assembly version.
    /// </summary>
    public bool IsAssemblyVersionValid => AssemblyVersion.Length > 0;

    /// <summary>
    /// Gets the file version.
    /// </summary>
    public string FileVersion { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the project has a valid file version.
    /// </summary>
    public bool IsFileVersionValid => FileVersion.Length > 0;

    /// <summary>
    /// Gets the project author.
    /// </summary>
    public string Author { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the project description.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the project copyright text.
    /// </summary>
    public string Copyright { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the project repository URL.
    /// </summary>
    public Uri? RepositoryUrl { get; private set; }

    /// <summary>
    /// Gets the project application icon.
    /// </summary>
    public string ApplicationIcon { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the project has a repository URL.
    /// </summary>
    public bool HasRepositoryUrl => RepositoryUrl is not null;

    /// <summary>
    /// Gets the list of parsed project frameworks.
    /// </summary>
    public IReadOnlyList<Framework> FrameworkList { get; private set; } = new List<Framework>().AsReadOnly();

    /// <summary>
    /// Gets a value indicating whether the project has target frameworks.
    /// </summary>
    public bool HasTargetFrameworks => FrameworkList.Count > 0;

    /// <summary>
    /// Gets the language version.
    /// </summary>
    public string LanguageVersion { get; private set; } = string.Empty;

    /// <summary>
    /// Gets project nullable values setting.
    /// </summary>
    public NullableAnnotation Nullable { get; private set; }

    /// <summary>
    /// Gets the neutral langauge.
    /// </summary>
    public string NeutralLanguage { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether project warnings are treated as errors.
    /// </summary>
    public bool IsTreatWarningsAsErrors { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the project is a test project.
    /// </summary>
    public bool IsTestProject { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the project is not packable.
    /// </summary>
    public bool IsNotPackable { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the project links to the solution's editor config.
    /// </summary>
    public bool IsEditorConfigLinked { get; private set; }

    /// <summary>
    /// Gets the list of package references.
    /// </summary>
    public IReadOnlyList<PackageReference> PackageReferenceList { get; private set; } = new List<PackageReference>().AsReadOnly();
}
