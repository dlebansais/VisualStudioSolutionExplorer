namespace VisualStudioSolutionExplorer.Test;

using System;
using System.IO;
using NUnit.Framework;
using SlnExplorer;

[TestFixture]
public partial class TestVisualStudioSolutionExplorer
{
    private const string TestSolutionsFolder = "TestSolutions";

    [Test]
    public void TestSuccess()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";

        string SolutionPath = Path.Combine(RootPath, TestSolution, $"{TestSolution}.sln");
        Solution NewSolution = new(SolutionPath);

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.IsProjectWithOutput)
            {
                string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
                Project.LoadDetails(ProjectPath);
            }

            bool HasError = Project.CheckVersionConsistency(out _);

            Assert.That(HasError, Is.False);

            if (Project.ProjectName == TestSolution)
            {
                Assert.That(Project.ParentSolution, Is.EqualTo(NewSolution));
                Assert.That(Project.IsAssemblyVersionValid, Is.True);
                Assert.That(Project.IsFileVersionValid, Is.True);
                Assert.That(Project.HasRepositoryUrl, Is.True);
                Assert.That(Project.HasTargetFrameworks, Is.True);

                Assert.That(Project.ProjectConfigurations.Count , Is.EqualTo(2));

                Configuration FirstConfiguration = Project.ProjectConfigurations[0];
                Assert.That(FirstConfiguration.Project, Is.EqualTo(Project));
                Assert.That(FirstConfiguration.ConfigurationName, Is.EqualTo("Debug"));
                Assert.That(FirstConfiguration.PlatformName, Is.EqualTo("x64"));
                Assert.That(FirstConfiguration.IncludeInBuild, Is.True);

                Assert.That(Project.FrameworkList.Count, Is.EqualTo(5));

                Framework FirstFramework = Project.FrameworkList[0];
                Assert.That(FirstFramework.ToString(), Is.EqualTo(FirstFramework.Name));

                Assert.That(Project.PackageReferenceList.Count, Is.EqualTo(6));

                PackageReference FirstPackageReference = Project.PackageReferenceList[0];
                Assert.That(FirstPackageReference.Project, Is.EqualTo(Project));
                Assert.That(FirstPackageReference.Name, Is.EqualTo("StyleCop.Analyzers.Unstable"));
                Assert.That(FirstPackageReference.Version, Is.EqualTo("1.2.0.556"));
                Assert.That(FirstPackageReference.Condition, Is.Empty);
            }
        }
    }

#if NET481_OR_GREATER
    [Test]
    public void TestFromReader()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";

        string SolutionPath = Path.Combine(RootPath, TestSolution, $"{TestSolution}.sln");
        using FileStream Stream = new(SolutionPath, FileMode.Open, FileAccess.Read);
        using StreamReader Reader = new(Stream);
        Solution NewSolution = new(TestSolution, Reader);

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.IsProjectWithOutput)
            {
                string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
                Project.LoadDetails(ProjectPath);
            }

            bool HasError = Project.CheckVersionConsistency(out _);

            Assert.That(HasError, Is.False);

            if (Project.ProjectName == TestSolution)
            {
                Assert.That(Project.ProjectConfigurations.Count, Is.EqualTo(2));

                Configuration FirstConfiguration = Project.ProjectConfigurations[0];
                Assert.That(FirstConfiguration.Project, Is.EqualTo(Project));
                Assert.That(FirstConfiguration.ConfigurationName, Is.EqualTo("Debug"));
                Assert.That(FirstConfiguration.PlatformName, Is.EqualTo("x64"));
                Assert.That(FirstConfiguration.IncludeInBuild, Is.True);

                Assert.That(Project.FrameworkList.Count, Is.EqualTo(5));

                Framework FirstFramework = Project.FrameworkList[0];
                Assert.That(FirstFramework.ToString(), Is.EqualTo(FirstFramework.Name));

                Assert.That(Project.PackageReferenceList.Count, Is.EqualTo(6));

                PackageReference FirstPackageReference = Project.PackageReferenceList[0];
                Assert.That(FirstPackageReference.Project, Is.EqualTo(Project));
                Assert.That(FirstPackageReference.Name, Is.EqualTo("StyleCop.Analyzers.Unstable"));
                Assert.That(FirstPackageReference.Version, Is.EqualTo("1.2.0.556"));
                Assert.That(FirstPackageReference.Condition, Is.Empty);
            }
        }
    }
#else
    [Test]
    public void TestFromReader()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";

        string SolutionPath = Path.Combine(RootPath, TestSolution, $"{TestSolution}.sln");
        using FileStream Stream = new(SolutionPath, FileMode.Open, FileAccess.Read);
        using StreamReader Reader = new(Stream);

#pragma warning disable CS0618
        _ = Assert.Throws<NotImplementedException>(() => _ = new Solution(TestSolution, Reader));
#pragma warning restore CS0618
    }
#endif

    [Test]
    public void TestLoadDetailStringStream()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";

        Solution NewSolution = new(Path.Combine(RootPath, TestSolution, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            bool IsProjectWithOutput = false;
            IsProjectWithOutput |= Project.ProjectType == ProjectType.Unknown;
            IsProjectWithOutput |= Project.ProjectType == ProjectType.KnownToBeMSBuildFormat;

            if (IsProjectWithOutput)
            {
                string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);
            }

            bool HasError = Project.CheckVersionConsistency(out _);

            Assert.That(HasError, Is.False);
        }
    }

    [Test]
    public void TestImport()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "ConsistencyAnalyzer";

        Solution NewSolution = new(Path.Combine(RootPath, TestSolution, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.IsProjectWithOutput)
            {
                string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
                Project.LoadDetails(ProjectPath);
            }

            bool HasError = Project.CheckVersionConsistency(out _);

            Assert.That(HasError, Is.False);
        }
    }

    [Test]
    public void TestUseWinForms()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "TaskbarTools";

        Solution NewSolution = new(Path.Combine(RootPath, TestSolution, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.IsProjectWithOutput)
            {
                string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
                Project.LoadDetails(ProjectPath);
            }

            bool HasError = Project.CheckVersionConsistency(out _);

            Assert.That(HasError, Is.False);

            if (Project.ProjectName == TestSolution)
            {
                Assert.That(Project.UseWindowsForms, Is.True);
            }
        }
    }

    [Test]
    public void TestConsoleProject()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Packager";

        Solution NewSolution = new(Path.Combine(RootPath, TestSolution, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.IsProjectWithOutput)
            {
                string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
                Project.LoadDetails(ProjectPath);
            }

            bool HasError = Project.CheckVersionConsistency(out _);

            Assert.That(HasError, Is.False);

            if (Project.ProjectName == TestSolution)
            {
                Assert.That(Project.ProjectType, Is.EqualTo(ProjectType.WinExe));
            }
        }
    }

    [Test]
    public void TestOldSdk()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "PgSearch";

        Solution NewSolution = new(Path.Combine(RootPath, TestSolution, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.IsProjectWithOutput)
            {
                string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
                Project.LoadDetails(ProjectPath);
            }

            bool HasError = Project.CheckVersionConsistency(out _);

            Assert.That(HasError, Is.False);
        }
    }

    [Test]
    public void TestUseNeitherWpfOrWinForms()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts.Analyzers";

        Solution NewSolution = new(Path.Combine(RootPath, TestSolution, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
            Project.LoadDetails(ProjectPath);

            bool HasError = Project.CheckVersionConsistency(out _);

            Assert.That(HasError, Is.False);

            if (Project.ProjectName == $"{TestSolution}.Demo")
            {
                Assert.That(Project.ProjectType, Is.EqualTo(ProjectType.Console));
                Assert.That(Project.UseWpf, Is.False);
                Assert.That(Project.UseWindowsForms, Is.False);
            }
        }
    }

    [Test]
    public void TestSolutionItems()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "UpdateCheck";

        Solution NewSolution = new(Path.Combine(RootPath, TestSolution, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
            if (Project.ProjectType != ProjectType.SolutionFolder)
            {
                string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
                Project.LoadDetails(ProjectPath);

                bool HasError = Project.CheckVersionConsistency(out _);

                Assert.That(HasError, Is.False);

                if (Project.ProjectName == $"{TestSolution}.Demo")
                {
                    Assert.That(Project.ProjectType, Is.EqualTo(ProjectType.WinExe));
                    Assert.That(Project.UseWpf, Is.True);
                    Assert.That(Project.UseWindowsForms, Is.False);
                }
            }
    }
}
