namespace SlnExplorer;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
#if !NET481
using Microsoft.Build.Construction;
#endif

/// <summary>
/// Reads and parses a project file.
/// </summary>
[DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectType}")]
public partial class Project
{
#if NET481
    private static readonly Type ProjectInSolutionType = ReflectionTools.GetProjectInSolutionType("ProjectInSolution");
    private static readonly PropertyInfo ProjectInSolutionProjectName = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectName));
    private static readonly PropertyInfo ProjectInSolutionRelativePath = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(RelativePath));
    private static readonly PropertyInfo ProjectInSolutionProjectGuid = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectGuid));
    private static readonly PropertyInfo ProjectInSolutionProjectType = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectType));
    private static readonly PropertyInfo ProjectInSolutionExtension = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(Extension));
    private static readonly PropertyInfo ProjectInSolutionDependencies = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(Dependencies));
    private static readonly PropertyInfo ProjectInSolutionProjectReferences = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectReferences));
    private static readonly PropertyInfo ProjectInSolutionParentProjectGuid = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ParentProjectGuid));
    private static readonly PropertyInfo ProjectInSolutionProjectConfigurations = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectConfigurations));
    private static readonly PropertyInfo ProjectInSolutionDependencyLevel = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(DependencyLevel));
    private static readonly PropertyInfo ProjectInSolutionIsStaticLibrary = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(IsStaticLibrary));
#endif
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
#if NET481
        ProjectName = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectName, solutionProject);
        RelativePath = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionRelativePath, solutionProject);
        ProjectGuid = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectGuid, solutionProject);
        Extension = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionExtension, solutionProject);
        DependencyLevel = (int)ReflectionTools.GetPropertyValue(ProjectInSolutionDependencyLevel, solutionProject);
        IsStaticLibrary = (bool)ReflectionTools.GetPropertyValue(ProjectInSolutionIsStaticLibrary, solutionProject);
#else
        ProjectInSolution ProjectInSolution = (ProjectInSolution)solutionProject;
        ProjectName = ProjectInSolution.ProjectName;
        RelativePath = ProjectInSolution.RelativePath;
        ProjectGuid = ProjectInSolution.ProjectGuid;
        Extension = Path.GetExtension(RelativePath);
        DependencyLevel = -1;
        IsStaticLibrary = false;
#endif
    }

    private void InitProjectType(object solutionProject)
    {
#if NET481
        var ProjectTypeValue = ReflectionTools.GetPropertyValue(ProjectInSolutionProjectType, solutionProject);

        ProjectType = ProjectTypeValue.ToString() switch
        {
            "Unknown" => ProjectType.Unknown,
            "KnownToBeMSBuildFormat" => ProjectType.KnownToBeMSBuildFormat,
            "SolutionFolder" => ProjectType.SolutionFolder,
            "WebProject" => ProjectType.WebProject,
            "WebDeploymentProject" => ProjectType.WebDeploymentProject,
            "EtpSubProject" => ProjectType.EtpSubProject,
            "SharedProject" => ProjectType.SharedProject,
            _ => ProjectType.Invalid,
        };
#else
        ProjectInSolution ProjectInSolution = (ProjectInSolution)solutionProject;

        ProjectType = ProjectInSolution.ProjectType switch
        {
            SolutionProjectType.Unknown => ProjectType.Unknown,
            SolutionProjectType.KnownToBeMSBuildFormat => ProjectType.KnownToBeMSBuildFormat,
            SolutionProjectType.SolutionFolder => ProjectType.SolutionFolder,
            SolutionProjectType.WebProject => ProjectType.WebProject,
            SolutionProjectType.WebDeploymentProject => ProjectType.WebDeploymentProject,
            SolutionProjectType.EtpSubProject => ProjectType.EtpSubProject,
            SolutionProjectType.SharedProject => ProjectType.SharedProject,
            _ => ProjectType.Invalid,
        };
#endif
    }

    private void InitDependencies(object solutionProject)
    {
#if NET481
        System.Collections.IEnumerable DependenciesValue = (System.Collections.IEnumerable)ReflectionTools.GetPropertyValue(ProjectInSolutionDependencies, solutionProject);
        Dependencies = DependenciesValue.Cast<string>().ToList().AsReadOnly();

        System.Collections.IEnumerable ProjectReferencesValue = (System.Collections.IEnumerable)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectReferences, solutionProject);
        ProjectReferences = ProjectReferencesValue.Cast<string>().ToList().AsReadOnly();

        // If ParentProjectGuidValue is null, ParentProjectGuid is set to string.Empty.
        object? ParentProjectGuidValue = ProjectInSolutionParentProjectGuid.GetValue(solutionProject);
        ParentProjectGuid = Convert.ToString(ParentProjectGuidValue, CultureInfo.InvariantCulture);
#else
        ProjectInSolution ProjectInSolution = (ProjectInSolution)solutionProject;

        List<string> DependencyList = new(ProjectInSolution.Dependencies);
        List<string> ProjectReferenceList = new();

        Dependencies = DependencyList.AsReadOnly();
        ProjectReferences = ProjectReferenceList.AsReadOnly();

        // If ProjectInSolution.ParentProjectGuid is null, ParentProjectGuid is set to string.Empty.
        object ParentProjectGuidValue = ProjectInSolution.ParentProjectGuid;
        ParentProjectGuid = Convert.ToString(ParentProjectGuidValue, CultureInfo.InvariantCulture)!;
#endif
    }

    private void InitConfigurations(object solutionProject)
    {
#if NET481
        System.Collections.IDictionary ProjectConfigurationsValue = (System.Collections.IDictionary)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectConfigurations, solutionProject);
        List<Configuration> ConfigurationList = new();
        foreach (string Key in ProjectConfigurationsValue.Keys)
        {
            object Value = ProjectConfigurationsValue[Key];

            // Ensures there is at least two elements in Splits.
            string ExtendedKey = Key + "|";

            string[] Splits = ExtendedKey.Split('|');
            string ConfigurationName = Splits[0];
            string PlatformName = Splits[1];
            ConfigurationList.Add(new Configuration(this, Value, ConfigurationName, PlatformName));
        }

        ProjectConfigurations = ConfigurationList.AsReadOnly();
#else
        ProjectInSolution ProjectInSolution = (ProjectInSolution)solutionProject;
        IReadOnlyDictionary<string, ProjectConfigurationInSolution> ProjectConfigurationsValue = ProjectInSolution.ProjectConfigurations;

        List<Configuration> ConfigurationList = new();
        foreach (string Key in ProjectConfigurationsValue.Keys)
        {
            object Value = ProjectConfigurationsValue[Key];

            // Ensures there is at least two elements in Splits.
            string ExtendedKey = Key + "|";

            string[] Splits = ExtendedKey.Split('|');
            string ConfigurationName = Splits[0];
            string PlatformName = Splits[1];
            ConfigurationList.Add(new Configuration(this, Value, ConfigurationName, PlatformName));
        }

        ProjectConfigurations = ConfigurationList.AsReadOnly();
#endif
    }
}
