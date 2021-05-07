namespace SlnExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Reads and parses a project file.
    /// </summary>
    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectType}")]
    public partial class Project
    {
        static Project()
        {
            ProjectInSolutionType = ReflectionTools.GetProjectInSolutionType("ProjectInSolution");

            ProjectInSolutionProjectName = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectName));
            ProjectInSolutionRelativePath = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(RelativePath));
            ProjectInSolutionProjectGuid = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectGuid));
            ProjectInSolutionProjectType = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectType));
            ProjectInSolutionExtension = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(Extension));
            ProjectInSolutionDependencies = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(Dependencies));
            ProjectInSolutionProjectReferences = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectReferences));
            ProjectInSolutionParentProjectGuid = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ParentProjectGuid));
            ProjectInSolutionProjectConfigurations = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectConfigurations));
            ProjectInSolutionDependencyLevel = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(DependencyLevel));
            ProjectInSolutionIsStaticLibrary = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(IsStaticLibrary));
        }

        private static readonly Type ProjectInSolutionType;
        private static readonly PropertyInfo ProjectInSolutionProjectName;
        private static readonly PropertyInfo ProjectInSolutionRelativePath;
        private static readonly PropertyInfo ProjectInSolutionProjectGuid;
        private static readonly PropertyInfo ProjectInSolutionProjectType;
        private static readonly PropertyInfo ProjectInSolutionExtension;
        private static readonly PropertyInfo ProjectInSolutionDependencies;
        private static readonly PropertyInfo ProjectInSolutionProjectReferences;
        private static readonly PropertyInfo ProjectInSolutionParentProjectGuid;
        private static readonly PropertyInfo ProjectInSolutionProjectConfigurations;
        private static readonly PropertyInfo ProjectInSolutionDependencyLevel;
        private static readonly PropertyInfo ProjectInSolutionIsStaticLibrary;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="solution">The solution containing the project.</param>
        /// <param name="solutionProject">The project as loaded from a solution.</param>
        internal Project(Solution solution, object solutionProject)
        {
            ParentSolution = solution;

            InitBasic(solutionProject);
            InitProjectType(solutionProject);
            InitDependencies(solutionProject);
            InitConfigurations(solutionProject);
        }

        private void InitBasic(object solutionProject)
        {
            ProjectName = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectName, solutionProject);
            RelativePath = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionRelativePath, solutionProject);
            ProjectGuid = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectGuid, solutionProject);
            Extension = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionExtension, solutionProject);
            DependencyLevel = (int)ReflectionTools.GetPropertyValue(ProjectInSolutionDependencyLevel, solutionProject);
            IsStaticLibrary = (bool)ReflectionTools.GetPropertyValue(ProjectInSolutionIsStaticLibrary, solutionProject);
        }

        private void InitProjectType(object solutionProject)
        {
            var ProjectTypeValue = ReflectionTools.GetPropertyValue(ProjectInSolutionProjectType, solutionProject);

            switch (ProjectTypeValue.ToString())
            {
                case "Unknown":
                    ProjectType = ProjectType.Unknown;
                    break;
                case "KnownToBeMSBuildFormat":
                    ProjectType = ProjectType.KnownToBeMSBuildFormat;
                    break;
                case "SolutionFolder":
                    ProjectType = ProjectType.SolutionFolder;
                    break;
                case "WebProject":
                    ProjectType = ProjectType.WebProject;
                    break;
                case "WebDeploymentProject":
                    ProjectType = ProjectType.WebDeploymentProject;
                    break;
                case "EtpSubProject":
                    ProjectType = ProjectType.EtpSubProject;
                    break;
                default:
                    ProjectType = ProjectType.Invalid;
                    break;
            }
        }

        private void InitDependencies(object solutionProject)
        {
            System.Collections.IEnumerable DependenciesValue = (System.Collections.IEnumerable)ReflectionTools.GetPropertyValue(ProjectInSolutionDependencies, solutionProject);
            List<string> DependencyList = new();
            foreach (string Item in DependenciesValue)
                DependencyList.Add(Item);
            Dependencies = DependencyList.AsReadOnly();

            System.Collections.IEnumerable ProjectReferencesValue = (System.Collections.IEnumerable)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectReferences, solutionProject);
            List<string> ProjectReferenceList = new();
            foreach (string Item in ProjectReferencesValue)
                ProjectReferenceList.Add(Item);
            ProjectReferences = ProjectReferenceList.AsReadOnly();

            object? ParentProjectGuidValue = ProjectInSolutionParentProjectGuid.GetValue(solutionProject);
            if (ParentProjectGuidValue != null)
                ParentProjectGuid = (string)ParentProjectGuidValue;
            else
                ParentProjectGuid = string.Empty;
        }

        private void InitConfigurations(object solutionProject)
        {
            System.Collections.IDictionary ProjectConfigurationsValue = (System.Collections.IDictionary)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectConfigurations, solutionProject);
            List<Configuration> ConfigurationList = new();
            foreach (string Key in ProjectConfigurationsValue.Keys)
            {
                object? Value = ProjectConfigurationsValue[Key];

                string[] Splits = Key.Split('|');
                if (Splits.Length >= 2 && Value != null)
                {
                    string ConfigurationName = Splits[0];
                    string PlatformName = Splits[1];
                    ConfigurationList.Add(new Configuration(this, Value, ConfigurationName, PlatformName));
                }
            }

            ProjectConfigurations = ConfigurationList.AsReadOnly();
        }
    }
}
