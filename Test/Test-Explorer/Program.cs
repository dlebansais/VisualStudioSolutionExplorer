namespace TestEaslyNumber;

using System;
using System.Diagnostics;
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
#if NET48
        Console.WriteLine($"Framework: net48");
#elif NET6_0
        Console.WriteLine($"Framework: net6.0");
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
                Solution NewSolution = new Solution(Path.Combine(RootPath, SolutionName));
                DisplaySolutionProperties(NewSolution);

                foreach (Project Project in NewSolution.ProjectList)
                {
                    if (Project.ProjectType == ProjectType.Unknown || Project.ProjectType == ProjectType.KnownToBeMSBuildFormat)
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
        Console.WriteLine($"    Dependencies:     {project.Dependencies.Count}");

        foreach (string Dependency in project.Dependencies)
            Console.WriteLine($"                      {Dependency}");

        Console.WriteLine($"    References:       {project.ProjectReferences.Count}");

        foreach (string Reference in project.ProjectReferences)
            Console.WriteLine($"                      {Reference}");

        Console.WriteLine($"    Parent Guid:      '{project.ParentProjectGuid}'");
        Console.WriteLine($"    Configurations:   {project.ProjectConfigurations.Count}");

        foreach (Configuration Configuration in project.ProjectConfigurations)
        {
            Console.WriteLine($"                      {Configuration.Project.ProjectName}");
            Console.WriteLine($"                        {Configuration.ConfigurationName}");
            Console.WriteLine($"                        {Configuration.PlatformName}");
            Console.WriteLine($"                        {(Configuration.IncludeInBuild ? "Included" : "Not Included")}");
        }

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
        Console.WriteLine($"    Frameworks:       {project.FrameworkList.Count}");

        foreach (Framework Framework in project.FrameworkList)
        {
            Console.WriteLine($"                        {Framework.Name}");
            Console.WriteLine($"                        {Framework.Type}");
            Console.WriteLine($"                        {Framework.Major}.{Framework.Minor}-{Framework.Moniker}{Framework.MonikerMajor}.{Framework.MonikerMinor}");
        }

        Console.WriteLine($"    Language:         {project.LanguageVersion}");
        Console.WriteLine($"    Nullable:         {project.Nullable}");
        Console.WriteLine($"    Neutral Language: '{project.NeutralLanguage}'");
        Console.WriteLine($"    Warning as error: {(project.IsTreatWarningsAsErrors ? "Yes" : "No")}");
        Console.WriteLine($"    Editor Config:    {(project.IsEditorConfigLinked ? "Linked" : "Not Linked")}");
        Console.WriteLine($"    Packages:         {project.PackageReferenceList.Count}");

        foreach (PackageReference Package in project.PackageReferenceList)
        {
            Console.WriteLine($"                        {Package.Project.ProjectName}");
            Console.WriteLine($"                          {Package.Name}");
            Console.WriteLine($"                          {Package.Version}");
            Console.WriteLine($"                          {(Package.Condition == string.Empty ? "Always" : "If " + Package.Condition)}");
        }
    }
}
