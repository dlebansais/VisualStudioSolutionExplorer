namespace SlnExplorer;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
#if !NET48
using Microsoft.Build.Construction;
#endif

/// <summary>
/// Reads and parses a project file.
/// </summary>
[DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectType}")]
public partial class Project
{
#if NET48
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
#if NET48
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
#if NET48
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
            case "SharedProject":
                ProjectType = ProjectType.SharedProject;
                break;
            default:
                ProjectType = ProjectType.Invalid;
                break;
        }
#else
        ProjectInSolution ProjectInSolution = (ProjectInSolution)solutionProject;

        switch (ProjectInSolution.ProjectType)
        {
            case SolutionProjectType.Unknown:
                ProjectType = ProjectType.Unknown;
                break;
            case SolutionProjectType.KnownToBeMSBuildFormat:
                ProjectType = ProjectType.KnownToBeMSBuildFormat;
                break;
            case SolutionProjectType.SolutionFolder:
                ProjectType = ProjectType.SolutionFolder;
                break;
            case SolutionProjectType.WebProject:
                ProjectType = ProjectType.WebProject;
                break;
            case SolutionProjectType.WebDeploymentProject:
                ProjectType = ProjectType.WebDeploymentProject;
                break;
            case SolutionProjectType.EtpSubProject:
                ProjectType = ProjectType.EtpSubProject;
                break;
            case SolutionProjectType.SharedProject:
                ProjectType = ProjectType.SharedProject;
                break;
            default:
                ProjectType = ProjectType.Invalid;
                break;
        }
#endif
    }

    private void InitDependencies(object solutionProject)
    {
#if NET48
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
#else
        ProjectInSolution ProjectInSolution = (ProjectInSolution)solutionProject;

        List<string> DependencyList = new(ProjectInSolution.Dependencies);
        List<string> ProjectReferenceList = new();

        Dependencies = DependencyList.AsReadOnly();
        ProjectReferences = ProjectReferenceList.AsReadOnly();

        if (ProjectInSolution.ParentProjectGuid is not null)
            ParentProjectGuid = (string)ProjectInSolution.ParentProjectGuid;
        else
            ParentProjectGuid = string.Empty;
#endif
    }

    private void InitConfigurations(object solutionProject)
    {
#if NET48
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
#else
        ProjectInSolution ProjectInSolution = (ProjectInSolution)solutionProject;
        IReadOnlyDictionary<string, ProjectConfigurationInSolution> ProjectConfigurationsValue = ProjectInSolution.ProjectConfigurations;

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
#endif
    }
}
