namespace SlnExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Contracts;

    /// <summary>
    /// Reads and parses a solution file.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class Solution
    {
        #region Init
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Solution"/> class.
        /// </summary>
        /// <param name="solutionFileName">The solution file name.</param>
        public Solution(string solutionFileName)
        {
            SolutionFileName = solutionFileName;
            Name = Path.GetFileNameWithoutExtension(solutionFileName);

            using StreamReader Reader = new StreamReader(solutionFileName);
            Initialize(Reader);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Solution"/> class.
        /// </summary>
        /// <param name="name">The solution name.</param>
        /// <param name="reader">A stream with the solution content.</param>
        public Solution(string name, StreamReader reader)
        {
            SolutionFileName = string.Empty;
            Name = name;

            Initialize(reader);
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
}
