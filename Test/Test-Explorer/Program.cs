namespace TestEaslyNumber;

using System;
using System.Collections.Generic;
using System.IO;
using SlnExplorer;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length < 1)
            return -1;

        string SolutionNameArg = args[0];

        Console.WriteLine($"Current directory: {Environment.CurrentDirectory}");
#if NET481
        Console.WriteLine($"Framework: net481");
#elif NET8_0
        Console.WriteLine($"Framework: net8.0");
#endif
        Console.WriteLine();

        string[] Directories = Directory.GetDirectories(@"C:\Projects");
        foreach (string Directory in Directories)
        {
            if (!Directory.EndsWith(SolutionNameArg))
                continue;

            string RootPath = @$"{Directory}\";
            string SolutionName = Path.GetFileName(Directory) + ".sln";
            if (File.Exists($"{RootPath}{SolutionName}"))
            {
                Solution NewSolution = new(Path.Combine(RootPath, SolutionName));
                DisplaySolutionProperties(NewSolution);

                foreach (Project Project in NewSolution.ProjectList)
                {
                    if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
                    {
                        string ProjectPath = Path.Combine(RootPath, Project.RelativePath);
                        Project.LoadDetails(ProjectPath);
                    }

                    DisplayProjectProperties(Project);

                    Project.CheckVersionConsistency(out _);
                }
            }
        }

        return 0;
    }

    private static void DisplaySolutionProperties(Solution solution)
    {
        Console.WriteLine($"Solution:             {solution.Name}");
        Console.WriteLine($"    FileName:         {solution.SolutionFileName}");
        Console.WriteLine($"    Projects:         {solution.ProjectList.Count}");
    }

    private static void DisplayProjectProperties(Project project)
    {
        Console.WriteLine();
        Console.WriteLine($"Project:              {project.ProjectName}");
        Console.WriteLine($"    Parent:           {project.ParentSolution.Name}");
        Console.WriteLine($"    Path:             {project.RelativePath}");
        Console.WriteLine($"    Guid:             {project.ProjectGuid}");
        Console.WriteLine($"    Type:             {project.ProjectType}");
        Console.WriteLine($"    Extension:        '{project.Extension}'");

        DisplayProjectPropertiesDependencies(project.Dependencies);
        DisplayProjectPropertiesReferences(project.ProjectReferences);

        Console.WriteLine($"    Parent Guid:      '{project.ParentProjectGuid}'");

        DisplayProjectPropertiesConfigurations(project.ProjectConfigurations);

        Console.WriteLine($"    Dep. Level:       {project.DependencyLevel}");
        Console.WriteLine($"    Static Library:   {(project.IsStaticLibrary ? "Yes" : "No")}");
        Console.WriteLine($"    Sdk Type:         {project.SdkType}");
        Console.WriteLine($"    Output Type:      {project.OutputType}");
        Console.WriteLine($"    Use Wpf:          {(project.UseWpf ? "Yes" : "No")}");
        Console.WriteLine($"    Use Forms:        {(project.UseWindowsForms ? "Yes" : "No")}");
        Console.WriteLine($"    Version");
        Console.WriteLine($"      Has Version:    {(project.HasVersion ? "Yes" : "No")}");
        Console.WriteLine($"      Version:        {project.Version}");
        Console.WriteLine($"      Has Assembly:   {(project.IsAssemblyVersionValid ? "Yes" : "No")}");
        Console.WriteLine($"      Assembly:       {project.AssemblyVersion}");
        Console.WriteLine($"      Has File:       {(project.IsFileVersionValid ? "Yes" : "No")}");
        Console.WriteLine($"      File:           {project.FileVersion}");
        Console.WriteLine($"    Author:           {project.Author}");
        Console.WriteLine($"    Description:      {project.Description}");
        Console.WriteLine($"    Copyright:        {project.Copyright}");
        Console.WriteLine($"    Repository Url:   {(project.RepositoryUrl is null ? "None" : project.RepositoryUrl)}");
        Console.WriteLine($"    Icon:             {project.ApplicationIcon}");
        Console.WriteLine($"    Package Icon:     {project.PackageIcon}");
        Console.WriteLine($"    Package License:  {project.PackageLicenseExpression}");
        Console.WriteLine($"    Package readme:   {project.PackageReadmeFile}");

        DisplayProjectPropertiesFrameworkList(project.FrameworkList);

        Console.WriteLine($"    Language:         {project.LanguageVersion}");
        Console.WriteLine($"    Nullable:         {project.Nullable}");
        Console.WriteLine($"    Neutral Language: '{project.NeutralLanguage}'");
        Console.WriteLine($"    Warning as error: {(project.IsTreatWarningsAsErrors ? "Yes" : "No")}");
        Console.WriteLine($"    Test project:     {(project.IsTestProject ? "Yes" : "No")}");
        Console.WriteLine($"    Packable:         {(project.IsNotPackable ? "No" : "Yes")}");
        Console.WriteLine($"    Editor Config:    {(project.IsEditorConfigLinked ? "Linked" : "Not Linked")}");

        DisplayProjectPropertiesPackageReferenceList(project.PackageReferenceList);
    }

    private static void DisplayProjectPropertiesDependencies(IReadOnlyList<string> dependencies)
    {
        Console.WriteLine($"    Dependencies:     {dependencies.Count}");

        foreach (string Dependency in dependencies)
            Console.WriteLine($"                      {Dependency}");
    }

    private static void DisplayProjectPropertiesReferences(IReadOnlyList<string> projectReferences)
    {
        Console.WriteLine($"    References:       {projectReferences.Count}");

        foreach (string Reference in projectReferences)
            Console.WriteLine($"                      {Reference}");
    }

    private static void DisplayProjectPropertiesConfigurations(IReadOnlyList<Configuration> configurations)
    {
        Console.WriteLine($"    Configurations:   {configurations.Count}");

        foreach (Configuration Configuration in configurations)
        {
            Console.WriteLine($"                      {Configuration.Project.ProjectName}");
            Console.WriteLine($"                        {Configuration.ConfigurationName}");
            Console.WriteLine($"                        {Configuration.PlatformName}");
            Console.WriteLine($"                        {(Configuration.IncludeInBuild ? "Included" : "Not Included")}");
        }
    }

    private static void DisplayProjectPropertiesFrameworkList(IReadOnlyList<Framework> frameworkList)
    {
        Console.WriteLine($"    Frameworks:       {frameworkList.Count}");

        foreach (Framework Framework in frameworkList)
        {
            Console.WriteLine($"                        {Framework.Name}");
            Console.WriteLine($"                        {Framework.Type}");
            Console.WriteLine($"                        {Framework.Major}.{Framework.Minor}-{Framework.Moniker}{Framework.MonikerMajor}.{Framework.MonikerMinor}");
        }
    }

    private static void DisplayProjectPropertiesPackageReferenceList(IReadOnlyList<PackageReference> packageReferenceList)
    {
        Console.WriteLine($"    Packages:         {packageReferenceList.Count}");

        foreach (PackageReference Package in packageReferenceList)
        {
            Console.WriteLine($"                        {Package.Project.ProjectName}");
            Console.WriteLine($"                          {Package.Name}");
            Console.WriteLine($"                          {Package.Version}");
            Console.WriteLine($"                          {(Package.Condition == string.Empty ? "Always" : "If " + Package.Condition)}");
        }
    }
}
