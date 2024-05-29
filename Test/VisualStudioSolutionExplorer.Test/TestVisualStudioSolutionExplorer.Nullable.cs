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
    public void TestNullableDisabled()
    {
        string RootPath = Path.Combine(TestTools.GetExecutingProjectRootPath(), TestSolutionsFolder);
        const string TestSolution = "Method.Contracts";
        const string NullableSolutionFolder = "Nullable-Disabled";

        Solution NewSolution = new(Path.Combine(RootPath, NullableSolutionFolder, $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));

        foreach (Project Project in NewSolution.ProjectList)
        {
            if (Project.IsProjectWithOutput)
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
            if (Project.IsProjectWithOutput)
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
            if (Project.IsProjectWithOutput)
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
}
