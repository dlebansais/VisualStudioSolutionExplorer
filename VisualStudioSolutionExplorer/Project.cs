namespace SlnExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;

    /// <summary>
    /// Reads and parses a project file.
    /// </summary>
    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectType}")]
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
            ProjectInSolutionExtension = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(Extension));
            ProjectInSolutionDependencies = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(Dependencies));
            ProjectInSolutionProjectReferences = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectReferences));
            ProjectInSolutionParentProjectGuid = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ParentProjectGuid));
            ProjectInSolutionProjectConfigurations = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(ProjectConfigurations));
            ProjectInSolutionDependencyLevel = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(DependencyLevel));
            ProjectInSolutionIsStaticLibrary = ReflectionTools.GetTypeProperty(ProjectInSolutionType, nameof(IsStaticLibrary));
        }

        private static readonly Type ProjectInSolutionType;
        private static readonly PropertyInfo ProjectInSolutionProjectName;
        private static readonly PropertyInfo ProjectInSolutionRelativePath;
        private static readonly PropertyInfo ProjectInSolutionProjectGuid;
        private static readonly PropertyInfo ProjectInSolutionProjectType;
        private static readonly PropertyInfo ProjectInSolutionExtension;
        private static readonly PropertyInfo ProjectInSolutionDependencies;
        private static readonly PropertyInfo ProjectInSolutionProjectReferences;
        private static readonly PropertyInfo ProjectInSolutionParentProjectGuid;
        private static readonly PropertyInfo ProjectInSolutionProjectConfigurations;
        private static readonly PropertyInfo ProjectInSolutionDependencyLevel;
        private static readonly PropertyInfo ProjectInSolutionIsStaticLibrary;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="solution">The solution containing the project.</param>
        /// <param name="solutionProject">The project as loaded from a solution.</param>
        internal Project(Solution solution, object solutionProject)
        {
            ParentSolution = solution;
            ProjectName = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectName, solutionProject);
            RelativePath = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionRelativePath, solutionProject);
            ProjectGuid = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectGuid, solutionProject);

            var ProjectTypeValue = ReflectionTools.GetPropertyValue(ProjectInSolutionProjectType, solutionProject);
            switch (ProjectTypeValue.ToString())
            {
                case "Unknown":
                    ProjectType = ProjectType.Unknown;
                    break;
                case "KnownToBeMSBuildFormat":
                    ProjectType = ProjectType.KnownToBeMSBuildFormat;
                    break;
                case "SolutionFolder":
                    ProjectType = ProjectType.SolutionFolder;
                    break;
                case "WebProject":
                    ProjectType = ProjectType.WebProject;
                    break;
                case "WebDeploymentProject":
                    ProjectType = ProjectType.WebDeploymentProject;
                    break;
                case "EtpSubProject":
                    ProjectType = ProjectType.EtpSubProject;
                    break;
                default:
                    ProjectType = ProjectType.Invalid;
                    break;
            }

            Extension = (string)ReflectionTools.GetPropertyValue(ProjectInSolutionExtension, solutionProject);

            System.Collections.IEnumerable DependenciesValue = (System.Collections.IEnumerable)ReflectionTools.GetPropertyValue(ProjectInSolutionDependencies, solutionProject);
            List<string> DependencyList = new();
            foreach (string Item in DependenciesValue)
                DependencyList.Add(Item);
            Dependencies = DependencyList.AsReadOnly();

            System.Collections.IEnumerable ProjectReferencesValue = (System.Collections.IEnumerable)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectReferences, solutionProject);
            List<string> ProjectReferenceList = new();
            foreach (string Item in ProjectReferencesValue)
                ProjectReferenceList.Add(Item);
            ProjectReferences = ProjectReferenceList.AsReadOnly();

            object? ParentProjectGuidValue = ProjectInSolutionParentProjectGuid.GetValue(solutionProject);
            if (ParentProjectGuidValue != null)
                ParentProjectGuid = (string)ParentProjectGuidValue;
            else
                ParentProjectGuid = string.Empty;

            System.Collections.IDictionary ProjectConfigurationsValue = (System.Collections.IDictionary)ReflectionTools.GetPropertyValue(ProjectInSolutionProjectConfigurations, solutionProject);
            List<Configuration> ConfigurationList = new();
            foreach (string Key in ProjectConfigurationsValue.Keys)
            {
                object? Value = ProjectConfigurationsValue[Key];

                string[] Splits = Key.Split('|');
                if (Splits.Length >= 2 && Value != null)
                {
                    string ConfigurationName = Splits[0];
                    string PlatformName = Splits[1];
                    ConfigurationList.Add(new Configuration(this, Value, ConfigurationName, PlatformName));
                }
            }

            ProjectConfigurations = ConfigurationList.AsReadOnly();

            DependencyLevel = (int)ReflectionTools.GetPropertyValue(ProjectInSolutionDependencyLevel, solutionProject);
            IsStaticLibrary = (bool)ReflectionTools.GetPropertyValue(ProjectInSolutionIsStaticLibrary, solutionProject);
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
        public ProjectType ProjectType { get; private set; }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public string Extension { get; init; }

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        public IReadOnlyCollection<string> Dependencies { get; init; }

        /// <summary>
        /// Gets the project references.
        /// </summary>
        public IReadOnlyCollection<string> ProjectReferences { get; init; }

        /// <summary>
        /// Gets the parent project GUID.
        /// </summary>
        public string ParentProjectGuid { get; init; }

        /// <summary>
        /// Gets the parent solution.
        /// </summary>
        public Solution ParentSolution { get; init; }

        /// <summary>
        /// Gets the project configurations.
        /// </summary>
        public IReadOnlyCollection<Configuration> ProjectConfigurations { get; init; }

        /// <summary>
        /// Gets the dependency level.
        /// </summary>
        public int DependencyLevel { get; init; }

        /// <summary>
        /// Gets a value indicating whether the project is a static library.
        /// </summary>
        public bool IsStaticLibrary { get; init; }

        /// <summary>
        /// Gets the Sdk type.
        /// </summary>
        public SdkType SdkType { get; private set; }

        /// <summary>
        /// Gets the project output type.
        /// </summary>
        public string OutputType { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the projct uses WPF.
        /// </summary>
        public bool UseWpf { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the projct uses Windows Forms.
        /// </summary>
        public bool UseWindowsForms { get; private set; }

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

        /// <summary>
        /// Gets the language version.
        /// </summary>
        public string LanguageVersion { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the project has nullable enabled.
        /// </summary>
        public bool IsNullable { get; private set; }

        /// <summary>
        /// Gets the neutral langauge.
        /// </summary>
        public string NeutralLanguage { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether project warnings are treated as errors.
        /// </summary>
        public bool IsTreatWarningsAsErrors { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the project links to the solution's editor config.
        /// </summary>
        public bool IsEditorConfigLinked { get; private set; }

        /// <summary>
        /// Gets the list of package references.
        /// </summary>
        public IReadOnlyList<PackageReference> PackageReferenceList { get; private set; } = new List<PackageReference>().AsReadOnly();
        #endregion

        #region Client Interface
        /// <summary>
        /// Loads project details from a file.
        /// </summary>
        /// <param name="fileName">The path to the file.</param>
        public void LoadDetails(string fileName)
        {
            using FileStream Stream = new(fileName, FileMode.Open, FileAccess.Read);
            ParseProjectElements(Stream);
        }

        /// <summary>
        /// Loads project details from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void LoadDetails(Stream stream)
        {
            ParseProjectElements(stream);
        }

        /// <summary>
        /// Checks a loaded for version consistency.
        /// </summary>
        /// <param name="warningOrErrorText">A warning or error text upon return.</param>
        /// <returns>True upon return if an error was found; otherwise, false.</returns>
        public bool CheckVersionConsistency(out string warningOrErrorText)
        {
            warningOrErrorText = string.Empty;
            bool HasErrors = false;

            if (HasVersion)
            {
                if (!IsVersionCompatible(AssemblyVersion, Version))
                {
                    HasErrors = true;
                    warningOrErrorText = $"{AssemblyVersion} not compatible with {Version}";
                }

                if (!IsVersionCompatible(FileVersion, Version))
                {
                    HasErrors = true;
                    warningOrErrorText = $"{FileVersion} not compatible with {Version}";
                }
            }
            else
                warningOrErrorText = "Ignored because no version";

            return HasErrors;
        }

        private bool IsVersionCompatible(string longVersion, string shortVersion)
        {
            string ComparedText = (longVersion.Length <= shortVersion.Length) ? shortVersion : $"{shortVersion}.";
            return longVersion.StartsWith(ComparedText, StringComparison.InvariantCulture);
        }
        #endregion

        #region Implementation
        private void ParseProjectElements(Stream stream)
        {
            XElement Root = XElement.Load(stream);

            foreach (XAttribute ProjectAttribute in Root.Attributes())
                ParseProjectAttribute(ProjectAttribute);

            foreach (XElement ProjectElement in Root.Descendants("PropertyGroup"))
                ParseProjectPropertyGroup(ProjectElement);

            List<Framework> ParsedFrameworkList = new List<Framework>();

            if (TargetFrameworks.Length > 0)
                ParseTargetFrameworks(ParsedFrameworkList);

            FrameworkList = ParsedFrameworkList.AsReadOnly();

            List<PackageReference> ParsedPackageReferenceList = new();
            foreach (XElement ProjectElement in Root.Descendants("ItemGroup"))
                ParseProjectItemGroup(ParsedPackageReferenceList, ProjectElement);

            PackageReferenceList = ParsedPackageReferenceList.AsReadOnly();
        }

        private void ParseProjectAttribute(XAttribute projectAttribute)
        {
            if (projectAttribute.Name == "Sdk")
            {
                switch (projectAttribute.Value)
                {
                    default:
                        break;
                    case "Microsoft.NET.Sdk":
                        SdkType = SdkType.Sdk;
                        break;
                    case "Microsoft.NET.Sdk.WindowsDesktop":
                        SdkType = SdkType.WindowsDesktop;
                        break;
                }
            }
        }

        private void ParseProjectPropertyGroup(XElement projectElement)
        {
            ParseProjectElementOutputType(projectElement);
            ParseProjectElementOptions(projectElement);
            ParseProjectElementVersion(projectElement);
            ParseProjectElementInfo(projectElement);
            ParseProjectElementFrameworks(projectElement);
        }

        private void ParseProjectElementOutputType(XElement projectElement)
        {
            XElement? OutputTypeElement = projectElement.Element("OutputType");
            if (OutputTypeElement != null)
                OutputType = OutputTypeElement.Value;

            XElement? UseWPFElement = projectElement.Element("UseWPF");
            if (UseWPFElement != null)
                UseWpf = UseWPFElement.Value.ToUpper() == "TRUE";

            XElement? UseWindowsFormsElement = projectElement.Element("UseWindowsForms");
            if (UseWindowsFormsElement != null)
                UseWindowsForms = UseWindowsFormsElement.Value.ToUpper() == "TRUE";

            if (ProjectType == ProjectType.Unknown)
            {
                if (OutputType.Length == 0 || OutputType == "Library")
                    ProjectType = ProjectType.Library;
                else if (OutputType == "WinExe")
                    ProjectType = ProjectType.WinExe;
                else if (OutputType == "Exe")
                    ProjectType = (UseWpf || UseWindowsForms) ? ProjectType.WinExe : ProjectType.Console;
            }
        }

        private void ParseProjectElementOptions(XElement projectElement)
        {
            XElement? LanguageVersionElement = projectElement.Element("LanguageVersion");
            if (LanguageVersionElement != null)
                LanguageVersion = LanguageVersionElement.Value;

            XElement? NullableElement = projectElement.Element("Nullable");
            if (NullableElement != null)
                IsNullable = NullableElement.Value.ToUpper() == "TRUE";

            XElement? NeutralLanguageElement = projectElement.Element("NeutralLanguage");
            if (NeutralLanguageElement != null)
                NeutralLanguage = NeutralLanguageElement.Value;

            XElement? TreatWarningsAsErrorsElement = projectElement.Element("TreatWarningsAsErrors");
            if (TreatWarningsAsErrorsElement != null)
                IsTreatWarningsAsErrors = TreatWarningsAsErrorsElement.Value.ToUpper() == "TRUE";
        }

        private void ParseProjectElementVersion(XElement projectElement)
        {
            XElement? VersionElement = projectElement.Element("Version");
            if (VersionElement != null)
                Version = VersionElement.Value;

            XElement? AssemblyVersionElement = projectElement.Element("AssemblyVersion");
            if (AssemblyVersionElement != null)
                AssemblyVersion = AssemblyVersionElement.Value;

            XElement? FileVersionElement = projectElement.Element("FileVersion");
            if (FileVersionElement != null)
                FileVersion = FileVersionElement.Value;
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

        private void ParseProjectItemGroup(List<PackageReference> packageReferenceList, XElement projectElement)
        {
            ParseProjectElementEditorConfigLink(projectElement);
            ParseProjectElementPackageReference(packageReferenceList, projectElement);
        }

        private void ParseProjectElementEditorConfigLink(XElement projectElement)
        {
            XElement? NoneElement = projectElement.Element("None");
            if (NoneElement != null)
            {
                bool IncludeEditorConfig = false;
                bool LinkEditorConfig = false;

                foreach (XAttribute Attribute in NoneElement.Attributes())
                {
                    if (Attribute.Name == "Include" && Attribute.Value.EndsWith(".editorconfig"))
                        IncludeEditorConfig = true;

                    if (Attribute.Name == "Link" && Attribute.Value == ".editorconfig")
                        LinkEditorConfig = true;
                }

                if (IncludeEditorConfig && LinkEditorConfig)
                    IsEditorConfigLinked = true;
            }
        }

        private void ParseProjectElementPackageReference(List<PackageReference> packageReferenceList, XElement projectElement)
        {
            XElement? PackageReferenceElement = projectElement.Element("PackageReference");
            if (PackageReferenceElement != null)
            {
                string Name = string.Empty;
                string Version = string.Empty;
                string Condition = string.Empty;

                foreach (XAttribute Attribute in PackageReferenceElement.Attributes())
                {
                    if (Attribute.Name == "Include")
                        Name = Attribute.Value;

                    if (Attribute.Name == "Version")
                        Version = Attribute.Value;

                    if (Attribute.Name == "Condition")
                        Condition = Attribute.Value;
                }

                if (Name.Length > 0 && Version.Length > 0)
                {
                    PackageReference NewPackageReference = new(this, Name, Version, Condition);
                    packageReferenceList.Add(NewPackageReference);
                }
            }
        }

        private string TargetFrameworks = string.Empty;
        #endregion
    }
}
