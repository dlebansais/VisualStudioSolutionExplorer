namespace SlnExplorer
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Reads and parses a project file.
    /// </summary>
    [DebuggerDisplay("{ConfigurationName}|{PlatformName}")]
    public class Configuration
    {
        #region Init
        static Configuration()
        {
            ProjectConfigurationInSolutionType = ReflectionTools.GetProjectInSolutionType("ProjectConfigurationInSolution");

            ProjectConfigurationInSolutionIncludeInBuild = ReflectionTools.GetTypeProperty(ProjectConfigurationInSolutionType, nameof(IncludeInBuild));
        }

        private static readonly Type ProjectConfigurationInSolutionType;
        private static readonly PropertyInfo ProjectConfigurationInSolutionIncludeInBuild;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="project">The project containing the configuration.</param>
        /// <param name="solutionConfiguration">The configuration object.</param>
        /// <param name="configurationName">The configuration name.</param>
        /// <param name="platformName">The platform name.</param>
        internal Configuration(Project project, object solutionConfiguration, string configurationName, string platformName)
        {
            Project = project;
            ConfigurationName = configurationName;
            PlatformName = platformName;

            IncludeInBuild = (bool)ReflectionTools.GetPropertyValue(ProjectConfigurationInSolutionIncludeInBuild, solutionConfiguration);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the project.
        /// </summary>
        public Project Project { get; }

        /// <summary>
        /// Gets the configuration name.
        /// </summary>
        public string ConfigurationName { get; }

        /// <summary>
        /// Gets the platform name.
        /// </summary>
        public string PlatformName { get; }

        /// <summary>
        /// Gets a value indicating whether the configuration is included in build.
        /// </summary>
        public bool IncludeInBuild { get; }
        #endregion
    }
}
