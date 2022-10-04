namespace SlnExplorer;

/// <summary>
/// Project types.
/// </summary>
public enum ProjectType
{
    /// <summary>
    /// Valid but unknown project type.
    /// </summary>
    Unknown,

    /// <summary>
    /// A library.
    /// </summary>
    Library,

    /// <summary>
    /// A console application.
    /// </summary>
    Console,

    /// <summary>
    /// An application with user interface.
    /// </summary>
    WinExe,

    /// <summary>
    /// For MSBuild.
    /// </summary>
    KnownToBeMSBuildFormat,

    /// <summary>
    /// Solution folder.
    /// </summary>
    SolutionFolder,

    /// <summary>
    /// Web Project.
    /// </summary>
    WebProject,

    /// <summary>
    /// Web Deployment Project.
    /// </summary>
    WebDeploymentProject,

    /// <summary>
    /// Subproject.
    /// </summary>
    EtpSubProject,

    /// <summary>
    /// A collection of shared files that is not buildable on its own.
    /// </summary>
    SharedProject,

    /// <summary>
    /// Invalid project type.
    /// </summary>
    Invalid,
}
