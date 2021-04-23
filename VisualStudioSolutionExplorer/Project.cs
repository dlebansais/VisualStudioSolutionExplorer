namespace SlnExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Xml.Linq;
    using Contracts;

    /// <summary>
    /// Reads and parses a project file.
    /// </summary>
    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class Project
    {
        #region Init
        static Project()
        {
            ProjectInSolutionType = ReflectionTools.GetProjectInSolutionType("ProjectInSolution");

            ProjectInSolutionProjectName = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectName));
            ProjectInSolutionRelativePath = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(RelativePath));
            ProjectInSolutionProjectGuid = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectGuid));
            ProjectInSolutionProjectType = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectType));
        }

        private static readonly Type ProjectInSolutionType;
        private static readonly PropertyInfo ProjectInSolutionProjectName;
        private static readonly PropertyInfo ProjectInSolutionRelativePath;
        private static readonly PropertyInfo ProjectInSolutionProjectGuid;
        private static readonly PropertyInfo ProjectInSolutionProjectType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="solutionProject">The project as loaded from a solution.</param>
        public Project(object solutionProject)
        {
            ProjectName = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectName, solutionProject);
            RelativePath = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionRelativePath, solutionProject);
            ProjectGuid = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectGuid, solutionProject);

            object Type = ReflectionTools.GetPropertyValue(ProjectInSolutionProjectType, solutionProject);
            Contract.RequireNotNull(Type.ToString(), out string ProjectTypeName);
            ProjectType = ProjectTypeName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the project name.
        /// </summary>
        public string ProjectName { get; init; }

        /// <summary>
        /// Gets the project relative path.
        /// </summary>
        public string RelativePath { get; init; }

        /// <summary>
        /// Gets the project GUID.
        /// </summary>
        public string ProjectGuid { get; init; }

        /// <summary>
        /// Gets the project type.
        /// </summary>
        public string ProjectType { get; init; }

        /// <summary>
        /// Gets the project version.
        /// </summary>
        public string Version { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the project has a version.
        /// </summary>
        public bool HasVersion => Version.Length > 0;

        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        public string AssemblyVersion { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the project has a valid assembly version.
        /// </summary>
        public bool IsAssemblyVersionValid => AssemblyVersion.Length > 0;

        /// <summary>
        /// Gets the file version.
        /// </summary>
        public string FileVersion { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the project has a valid file version.
        /// </summary>
        public bool IsFileVersionValid => FileVersion.Length > 0;

        /// <summary>
        /// Gets the project author.
        /// </summary>
        public string Author { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the project description.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the project copyright text.
        /// </summary>
        public string Copyright { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the project repository URL.
        /// </summary>
        public Uri? RepositoryUrl { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the project has a repository URL.
        /// </summary>
        public bool HasRepositoryUrl => RepositoryUrl != null;

        /// <summary>
        /// Gets the list of parsed project frameworks.
        /// </summary>
        public IReadOnlyList<Framework> FrameworkList { get; private set; } = new List<Framework>().AsReadOnly();

        /// <summary>
        /// Gets a value indicating whether the project has target frameworks.
        /// </summary>
        public bool HasTargetFrameworks => FrameworkList.Count > 0;
        #endregion

        #region Client Interface
        /// <summary>
        /// Parses a loaded project.
        /// </summary>
        /// <param name="warningOrErrorText">A warning or error text upon return.</param>
        /// <returns>True upon return if an error was found; otherwise, false.</returns>
        public bool Parse(out string warningOrErrorText)
        {
            warningOrErrorText = string.Empty;
            bool HasErrors = false;

            ParsePropertyGroupElements(out string LocalAssemblyVersion, out string LocalFileVersion);

            if (HasVersion)
            {
                if (LocalAssemblyVersion.StartsWith(Version, StringComparison.InvariantCulture))
                    AssemblyVersion = LocalAssemblyVersion;
                else
                {
                    HasErrors = true;
                    warningOrErrorText = $"{LocalAssemblyVersion} not compatible with {Version}";
                }

                if (LocalFileVersion.StartsWith(Version, StringComparison.InvariantCulture))
                    FileVersion = LocalFileVersion;
                else
                {
                    HasErrors = true;
                    warningOrErrorText = $"{LocalFileVersion} not compatible with {Version}";
                }
            }
            else
                warningOrErrorText = "Ignored because no version";

            List<Framework> ParsedFrameworkList = new List<Framework>();

            if (TargetFrameworks.Length > 0)
                ParseTargetFrameworks(ParsedFrameworkList);

            FrameworkList = ParsedFrameworkList.AsReadOnly();

            return HasErrors;
        }
        #endregion

        #region Implementation
        private void ParsePropertyGroupElements(out string assemblyVersion, out string fileVersion)
        {
            Version = string.Empty;

            assemblyVersion = string.Empty;
            fileVersion = string.Empty;

            XElement Root = XElement.Load(RelativePath);

            foreach (XElement ProjectElement in Root.Descendants("PropertyGroup"))
                ParseProjectElement(ProjectElement, ref assemblyVersion, ref fileVersion);
        }

        private void ParseProjectElement(XElement projectElement, ref string assemblyVersion, ref string fileVersion)
        {
            ParseProjectElementVersion(projectElement, ref assemblyVersion, ref fileVersion);
            ParseProjectElementInfo(projectElement);
            ParseProjectElementFrameworks(projectElement);
        }

        private void ParseProjectElementVersion(XElement projectElement, ref string assemblyVersion, ref string fileVersion)
        {
            XElement? VersionElement = projectElement.Element("Version");
            if (VersionElement != null)
                Version = VersionElement.Value;

            XElement? AssemblyVersionElement = projectElement.Element("AssemblyVersion");
            if (AssemblyVersionElement != null)
                assemblyVersion = AssemblyVersionElement.Value;

            XElement? FileVersionElement = projectElement.Element("FileVersion");
            if (FileVersionElement != null)
                fileVersion = FileVersionElement.Value;
        }

        private void ParseProjectElementInfo(XElement projectElement)
        {
            XElement? AuthorElement = projectElement.Element("Authors");
            if (AuthorElement != null)
                Author = AuthorElement.Value;

            XElement? DescriptionElement = projectElement.Element("Description");
            if (DescriptionElement != null)
                Description = DescriptionElement.Value;

            XElement? CopyrightElement = projectElement.Element("Copyright");
            if (CopyrightElement != null)
                Copyright = CopyrightElement.Value;

            XElement? RepositoryUrlElement = projectElement.Element("RepositoryUrl");
            if (RepositoryUrlElement != null)
                RepositoryUrl = new Uri(RepositoryUrlElement.Value);
        }

        private void ParseProjectElementFrameworks(XElement projectElement)
        {
            XElement? TargetFrameworkElement = projectElement.Element("TargetFramework");
            if (TargetFrameworkElement != null)
                TargetFrameworks = TargetFrameworkElement.Value;
            else
            {
                XElement? TargetFrameworksElement = projectElement.Element("TargetFrameworks");
                if (TargetFrameworksElement != null)
                    TargetFrameworks = TargetFrameworksElement.Value;
            }
        }

        private void ParseTargetFrameworks(List<Framework> parsedFrameworkList)
        {
            string[] Frameworks = TargetFrameworks.Split(';');

            foreach (string Framework in Frameworks)
                ParseTargetFramework(parsedFrameworkList, Framework);
        }

        private void ParseTargetFramework(List<Framework> parsedFrameworkList, string frameworkName)
        {
            string FrameworkString = frameworkName;

            string NetStandardPattern = "netstandard";
            string NetCorePattern = "netcoreapp";
            string NetFrameworkPattern = "net";

            Framework? NewFramework = null;
            int Major;
            int Minor;
            FrameworkMoniker Moniker = FrameworkMoniker.none;

            foreach (FrameworkMoniker? MonikerValue in typeof(FrameworkMoniker).GetEnumValues())
            {
                if (MonikerValue == null || MonikerValue == FrameworkMoniker.none)
                    continue;

                string MonikerName = MonikerValue.Value.ToString();
                string MonikerPattern = $"-{MonikerName}";
                if (FrameworkString.EndsWith(MonikerPattern, StringComparison.InvariantCulture))
                {
                    Moniker = MonikerValue.Value;
                    FrameworkString = FrameworkString.Substring(0, FrameworkString.Length - MonikerPattern.Length);
                    break;
                }
            }

            if (FrameworkString.StartsWith(NetStandardPattern, StringComparison.InvariantCulture) && ParseNetVersion(FrameworkString.Substring(NetStandardPattern.Length), out Major, out Minor))
                NewFramework = new Framework(frameworkName, FrameworkType.NetStandard, Major, Minor, Moniker);
            else if (FrameworkString.StartsWith(NetCorePattern, StringComparison.InvariantCulture) && ParseNetVersion(FrameworkString.Substring(NetCorePattern.Length), out Major, out Minor))
                NewFramework = new Framework(frameworkName, FrameworkType.NetCore, Major, Minor, Moniker);
            else if (FrameworkString.StartsWith(NetFrameworkPattern, StringComparison.InvariantCulture) && ParseNetVersion(FrameworkString.Substring(NetFrameworkPattern.Length), out Major, out Minor))
                NewFramework = new Framework(frameworkName, FrameworkType.NetFramework, Major, Minor, Moniker);

            if (NewFramework != null)
                parsedFrameworkList.Add(NewFramework);
        }

        private static bool ParseNetVersion(string text, out int major, out int minor)
        {
            major = -1;
            minor = -1;

            string[] Versions = text.Split('.');
            if (Versions.Length == 2)
            {
                if (int.TryParse(Versions[0], out major) && int.TryParse(Versions[1], out minor))
                    return true;
            }
            else if (Versions.Length == 1)
            {
                string Version = Versions[0];
                if (Version.Length > 1 && int.TryParse(Version.Substring(0, 1), out major) && int.TryParse(Version.Substring(1), out minor))
                    return true;
            }

            return false;
        }

        private string TargetFrameworks = string.Empty;
        #endregion
    }
}
