namespace SlnExplorer;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#if NET481
using System.Reflection;
using Contracts;
#else
using Microsoft.Build.Construction;
#endif

/// <summary>
/// Reads and parses a solution file.
/// </summary>
[DebuggerDisplay("{Name}")]
public class Solution
{
    #region Init
#if NET481
    private static readonly Type SolutionParserType = ReflectionTools.GetProjectInSolutionType("SolutionParser");
    private static readonly PropertyInfo SolutionParserReader = ReflectionTools.GetTypeProperty(SolutionParserType, "SolutionReader");
    private static readonly PropertyInfo SolutionParserProjects = ReflectionTools.GetTypeProperty(SolutionParserType, "Projects");
    private static readonly MethodInfo SolutionParserParseSolution = ReflectionTools.GetTypeMethod(SolutionParserType, "ParseSolution");
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="Solution"/> class.
    /// </summary>
    /// <param name="solutionFileName">The solution file name.</param>
    public Solution(string solutionFileName)
    {
        SolutionFileName = solutionFileName;
        Name = Path.GetFileNameWithoutExtension(solutionFileName);

        Initialize(solutionFileName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Solution"/> class.
    /// </summary>
    /// <param name="name">The solution name.</param>
    /// <param name="reader">A stream with the solution content.</param>
#if NET481
    public Solution(string name, StreamReader reader)
    {
        SolutionFileName = string.Empty;
        Name = name;

        Initialize(reader);
    }

    private void Initialize(string fileName)
    {
        using StreamReader Reader = new(fileName);
        Initialize(Reader);
    }

    private void Initialize(StreamReader reader)
    {
        ConstructorInfo Constructor = ReflectionTools.GetFirstTypeConstructor(SolutionParserType);
        object SolutionParser = Constructor.Invoke(null);

        SolutionParserReader.SetValue(SolutionParser, reader, null);
        _ = SolutionParserParseSolution.Invoke(SolutionParser, null);

        Array ProjetctArray = (Array)ReflectionTools.GetPropertyValue(SolutionParserProjects, SolutionParser);
        for (int i = 0; i < ProjetctArray.Length; i++)
        {
            object SolutionProject = Contract.AssertNotNull(ProjetctArray.GetValue(i));
            Project NewProject = new(this, SolutionProject);

            ProjectListInternal.Add(NewProject);
        }
    }
#else
    [Obsolete("Streams not supported except in .NET Framework 4.8. Use a solution file name.")]
    public Solution(string name, StreamReader reader)
    {
        throw new NotImplementedException("Streams not supported except in .NET Framework 4.8. Use a solution file name.");
    }

    private void Initialize(string fileName)
    {
        SolutionFile SolutionFile = SolutionFile.Parse(fileName);

        foreach (ProjectInSolution Item in SolutionFile.ProjectsInOrder)
        {
            Project NewProject = new(this, Item);
            ProjectListInternal.Add(NewProject);
        }
    }
#endif

    private List<Project> ProjectListInternal { get; } = [];
    #endregion

    #region Properties
    /// <summary>
    /// Gets the solution file name.
    /// </summary>
    public string SolutionFileName { get; init; }

    /// <summary>
    /// Gets the solution name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the list of projects in the solution.
    /// </summary>
    public IReadOnlyList<Project> ProjectList => ProjectListInternal.AsReadOnly();
    #endregion
}
