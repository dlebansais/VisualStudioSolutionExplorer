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
    public partial class Project
    {
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
            List<string> ParsedProjectReferenceList = new();
            foreach (XElement ProjectElement in Root.Descendants("ItemGroup"))
                ParseProjectItemGroup(ParsedPackageReferenceList, ParsedProjectReferenceList, ProjectElement);

            PackageReferenceList = ParsedPackageReferenceList.AsReadOnly();
            ProjectReferences = ParsedProjectReferenceList.AsReadOnly();
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
            XElement? LanguageVersionElement = projectElement.Element("LangVersion");
            if (LanguageVersionElement != null)
                LanguageVersion = LanguageVersionElement.Value;

            XElement? NullableElement = projectElement.Element("Nullable");
            if (NullableElement != null)
                IsNullable = NullableElement.Value.ToUpper() == "ENABLE";

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

        private void ParseProjectItemGroup(List<PackageReference> packageReferenceList, List<string> projectReferenceList, XElement projectElement)
        {
            ParseProjectElementEditorConfigLink(projectElement);
            ParseProjectElementPackageReference(packageReferenceList, projectElement);
            ParseProjectElementProjectReference(projectReferenceList, projectElement);
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

        private void ParseProjectElementProjectReference(List<string> projectReferenceList, XElement projectElement)
        {
            XElement? ProjectReferenceElement = projectElement.Element("ProjectReference");
            if (ProjectReferenceElement != null)
            {
                string Name = string.Empty;

                foreach (XAttribute Attribute in ProjectReferenceElement.Attributes())
                    if (Attribute.Name == "Include")
                        Name = Attribute.Value;

                if (Name.Length > 0)
                {
                    string ProjectFileName = Path.GetFileNameWithoutExtension(Name);

                    if (!projectReferenceList.Contains(ProjectFileName))
                        projectReferenceList.Add(ProjectFileName);
                }
            }
        }

        private bool IsVersionCompatible(string longVersion, string shortVersion)
        {
            string ComparedText = (longVersion.Length <= shortVersion.Length) ? shortVersion : $"{shortVersion}.";
            return longVersion.StartsWith(ComparedText, StringComparison.InvariantCulture);
        }

        private string TargetFrameworks = string.Empty;
    }
}
