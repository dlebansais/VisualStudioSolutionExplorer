namespace VisualStudioSolutionExplorer.Test;

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SlnExplorer;

[TestFixture]
public partial class TestVisualStudioSolutionExplorer
{
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
    public void TestInvalidNetFrameworkVersion1()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-009-1";

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
    public void TestInvalidNetFrameworkVersion2()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-009-2";

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
    public void TestInvalidNetFrameworkVersion3()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string InvalidSolutionFolder = "Invalid-009-3";

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
