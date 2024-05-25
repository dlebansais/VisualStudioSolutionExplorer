namespace VisualStudioSolutionExplorer.Test;

using System;
using System.Diagnostics;
using NUnit.Framework;
using SlnExplorer;

[TestFixture]
public class TestProjectType
{
    [Test]
    public void TestConversion()
    {
        string[] Names = typeof(ProjectType).GetEnumNames();
        Array Values = typeof(ProjectType).GetEnumValues();

        Debug.Assert(Names.Length == Values.Length);

        for (int i = 0; i < Names.Length; i++)
        {
            ProjectType? Type = (ProjectType?)Values.GetValue(i);
            Debug.Assert(Type is not null);

            bool IsLibrary = Type == ProjectType.Library;
            bool IsConsole = Type == ProjectType.Console;
            bool IsWinExe = Type == ProjectType.WinExe;

            if (!IsLibrary && !IsConsole && !IsWinExe)
            {
                Assert.That(Project.ConvertToProjectType(Names[i]), Is.EqualTo(Type));
            }
        }
    }
}
