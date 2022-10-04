﻿namespace SlnExplorer;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
#if NET48
using Contracts;
#else
using System.Reflection.PortableExecutable;
using Contracts;
using Microsoft.Build.Construction;
#endif

/// <summary>
/// Reads and parses a solution file.
/// </summary>
[DebuggerDisplay("{Name}")]
public class Solution
{
    #region Init
#if NET48
    static Solution()
    {
        SolutionParserType = ReflectionTools.GetProjectInSolutionType("SolutionParser");

        SolutionParserReader = ReflectionTools.GetTypeProperty(SolutionParserType, "SolutionReader");
        SolutionParserProjects = ReflectionTools.GetTypeProperty(SolutionParserType, "Projects");
        SolutionParserParseSolution = ReflectionTools.GetTypeMethod(SolutionParserType, "ParseSolution");
    }

    private static readonly Type SolutionParserType;
    private static readonly PropertyInfo SolutionParserReader;
    private static readonly PropertyInfo SolutionParserProjects;
    private static readonly MethodInfo SolutionParserParseSolution;
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
#if NET48
    public Solution(string name, StreamReader reader)
    {
        SolutionFileName = string.Empty;
        Name = name;

        Initialize(reader);
    }

    private void Initialize(string fileName)
    {
        using StreamReader Reader = new StreamReader(fileName);
        Initialize(Reader);
    }

    private void Initialize(StreamReader reader)
    {
        ConstructorInfo Constructor = ReflectionTools.GetFirstTypeConstructor(SolutionParserType);
        var SolutionParser = Constructor.Invoke(null);

        SolutionParserReader.SetValue(SolutionParser, reader, null);
        SolutionParserParseSolution.Invoke(SolutionParser, null);

        Array ProjetctArray = (Array)ReflectionTools.GetPropertyValue(SolutionParserProjects, SolutionParser);
        for (int i = 0; i < ProjetctArray.Length; i++)
        {
            Contract.RequireNotNull(ProjetctArray.GetValue(i), out object SolutionProject);
            Project NewProject = new Project(this, SolutionProject);

            ProjectList.Add(NewProject);
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
        Microsoft.Build.Construction.SolutionFile SolutionFile = Microsoft.Build.Construction.SolutionFile.Parse(fileName);

        foreach (ProjectInSolution Item in SolutionFile.ProjectsInOrder)
        {
            Project NewProject = new Project(this, Item);
            ProjectList.Add(NewProject);
        }
    }
#endif
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
    public List<Project> ProjectList { get; } = new();
    #endregion
}
