namespace SlnExplorer
{
    using System.Diagnostics;

    /// <summary>
    /// Reads and parses a project file.
    /// </summary>
    [DebuggerDisplay("{Name} Version {Version}")]
    public class PackageReference
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageReference"/> class.
        /// </summary>
        /// <param name="project">The project containing the configuration.</param>
        /// <param name="name">The package name.</param>
        /// <param name="version">The package version.</param>
        /// <param name="condition">The package condition.</param>
        internal PackageReference(Project project, string name, string version, string condition)
        {
            Project = project;
            Name = name;
            Version = version;
            Condition = condition;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the project.
        /// </summary>
        public Project Project { get; }

        /// <summary>
        /// Gets the package name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the package version.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the package condition.
        /// </summary>
        public string Condition { get; }
        #endregion
    }
}
