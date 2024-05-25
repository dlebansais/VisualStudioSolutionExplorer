namespace VisualStudioSolutionExplorer.Test;

using System.IO;
using NUnit.Framework;
using SlnExplorer;

[TestFixture]
public class TestVisualStudioSolutionExplorer
{
    private const string TestSolutionsFolder = "TestSolutions";

    [Test]
    public void TestSuccess()
    {
        string RootPath = TestTools.GetExecutingProjectRootPath();
        const string TestSolution = "Method.Contracts";

        Solution NewSolution = new(Path.Combine(RootPath, Path.Combine(TestSolutionsFolder, TestSolution), $"{TestSolution}.sln"));

        Assert.That(NewSolution.Name, Is.EqualTo(TestSolution));
    }
}
