namespace VisualStudioSolutionExplorer.Test;

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SlnExplorer;

[TestFixture]
public class TestVisualStudioSolutionExplorer
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
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
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
    public void TestfromReader()
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
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
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
    public void TestfromReader()
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
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
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
    public void TestInvalidVersion1()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-001-1";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.AssemblyVersion != string.Empty)
                {
                    bool HasError = Project.CheckVersionConsistency(out _);

                    Assert.That(HasError, Is.True);
                }
            }
        }
    }

    [Test]
    public void TestInvalidVersion2()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-001-2";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.AssemblyVersion != string.Empty)
                {
                    bool HasError = Project.CheckVersionConsistency(out _);

                    Assert.That(HasError, Is.True);
                }
            }
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
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
                Project.LoadDetails(ProjectPath);
            }

            bool HasError = Project.CheckVersionConsistency(out _);

            Assert.That(HasError, Is.False);
        }
    }

    [Test]
    public void TestNoFramework()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-002";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                Assert.That(Project.FrameworkList, Is.Empty);
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
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
                Project.LoadDetails(ProjectPath);
            }

            bool HasError = Project.CheckVersionConsistency(out _);

            Assert.That(HasError, Is.False);
        }
    }

    [Test]
    public void TestInvalidSdk()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-003";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.SdkType, Is.EqualTo(SdkType.Unknown));
                }
            }
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
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
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
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
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
    public void TestInvalidOutputType()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-004";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
#if NETFRAMEWORK
                    Assert.That(Project.ProjectType, Is.EqualTo(ProjectType.Unknown));
#else
                    Assert.That(Project.ProjectType, Is.EqualTo(ProjectType.KnownToBeMSBuildFormat));
#endif
                }
            }
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
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, TestSolution, Project.RelativePath);
                Project.LoadDetails(ProjectPath);
            }

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
    public void TestNullableDisabled()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string NullableSolutionFolder = "Nullable-Disabled";

        Solution NewSolution = new(Path.Combine(RootPath, NullableSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, NullableSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.Nullable, Is.EqualTo(NullableAnnotation.Disable));
                }
            }
        }
    }

    [Test]
    public void TestNullableWarnings()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string NullableSolutionFolder = "Nullable-Warnings";

        Solution NewSolution = new(Path.Combine(RootPath, NullableSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, NullableSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.Nullable, Is.EqualTo(NullableAnnotation.Warnings));
                }
            }
        }
    }

    [Test]
    public void TestNullableAnnotations()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string NullableSolutionFolder = "Nullable-Annotations";

        Solution NewSolution = new(Path.Combine(RootPath, NullableSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, NullableSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.Nullable, Is.EqualTo(NullableAnnotation.Annotations));
                }
            }
        }
    }

    [Test]
    public void TestInvalidNullable()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-005";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.Nullable, Is.EqualTo(NullableAnnotation.None));
                }
            }
        }
    }

    [Test]
    public void TestInvalidFramework()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-006";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.FrameworkList, Is.Empty);
                }
            }
        }
    }

    [Test]
    public void TestInvalidNetStandardVersion()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-007";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.FrameworkList, Is.Empty);
                }
            }
        }
    }

    [Test]
    public void TestInvalidNetCoreVersion()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-008";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.FrameworkList, Is.Empty);
                }
            }
        }
    }

    [Test]
    public void TestInvalidNetFrameworkVersion()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-009";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.FrameworkList, Is.Empty);
                }
            }
        }
    }

    [Test]
    public void TestInvalidFrameworkVersion()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-010";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.FrameworkList, Is.Empty);
                }
            }
        }
    }

    [Test]
    public void TestInvalidMonikerVersion1()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-011";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.FrameworkList.Count, Is.EqualTo(1));
                    Framework FirstFramework = Project.FrameworkList[0];
                    Assert.That(FirstFramework.MonikerMajor, Is.EqualTo(-1));
                    Assert.That(FirstFramework.MonikerMinor, Is.EqualTo(-1));
                }
            }
        }
    }

    [Test]
    public void TestInvalidMonikerVersion2()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-012";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.FrameworkList.Count, Is.EqualTo(1));
                    Framework FirstFramework = Project.FrameworkList[0];
                    Assert.That(FirstFramework.MonikerMajor, Is.EqualTo(-1));
                    Assert.That(FirstFramework.MonikerMinor, Is.EqualTo(-1));
                }
            }
        }
    }

    [Test]
    public void TestInvalidMonikerVersion3()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-013";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.FrameworkList.Count, Is.EqualTo(1));
                    Framework FirstFramework = Project.FrameworkList[0];
                    Assert.That(FirstFramework.MonikerMajor, Is.EqualTo(-1));
                    Assert.That(FirstFramework.MonikerMinor, Is.EqualTo(-1));
                }
            }
        }
    }

    [Test]
    public void TestInvalidPackageNoVersion()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-014";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.PackageReferenceList.Where(package => package.Name == "PolySharp"), Is.Empty);
                }
            }
        }
    }

    [Test]
    public void TestInvalidPackageNoName()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-015";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == TestSolution)
                {
                    Assert.That(Project.PackageReferenceList.Where(package => package.Name == "PolySharp"), Is.Empty);
                }
            }
        }
    }

    [Test]
    public void TestUnknownProjectReference()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-016";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == $"{TestSolution}.Test")
                {
                    Assert.That(Project.ProjectReferences.Where(projectName => projectName == TestSolution), Is.Empty);
                }
            }
        }
    }

    [Test]
    public void TestDuplicateProjectReference()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-017";

        Solution NewSolution = new(Path.Combine(RootPath, InvalidSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
            {
                string ProjectPath = Path.Combine(RootPath, InvalidSolutionFolder, Project.RelativePath);
                using FileStream Stream = new(ProjectPath, FileMode.Open, FileAccess.Read);
                Project.LoadDetails(Stream);

                if (Project.ProjectName == $"{TestSolution}.Test")
                {
                    Assert.That(Project.ProjectReferences.Count(projectName => projectName == TestSolution), Is.EqualTo(1));
                }
            }
        }
    }
}
