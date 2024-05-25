namespace SlnExplorer;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

/// <summary>
/// Reads and parses a project file.
/// </summary>
public partial class Project
{
    private const string NetStandardPattern = "netstandard";
    private const string NetCorePattern = "netcoreapp";
    private const string NetFrameworkPattern = "net";

    private void ParseProjectElements(Stream stream)
    {
        XElement Root = XElement.Load(stream);

        foreach (XAttribute ProjectAttribute in Root.Attributes())
            ParseProjectAttribute(ProjectAttribute);

        foreach (XElement ProjectElement in Root.Descendants("Import"))
            ParseProjectImport(ProjectElement);

        foreach (XElement ProjectElement in Root.Descendants("PropertyGroup"))
            ParseProjectPropertyGroup(ProjectElement);

        List<Framework> ParsedFrameworkList = new();

        if (TargetFrameworks.Length > 0)
            ParseTargetFrameworks(ParsedFrameworkList);

        FrameworkList = ParsedFrameworkList.AsReadOnly();

        List<PackageReference> ParsedPackageReferenceList = new();
        List<string> ParsedProjectReferenceList = new();
        IEnumerable<XElement> ItemGroups = Root.Descendants("ItemGroup");

        foreach (XElement ProjectElement in ItemGroups)
            ParseProjectItemGroup(ParsedPackageReferenceList, ParsedProjectReferenceList, ProjectElement);

        PackageReferenceList = ParsedPackageReferenceList.AsReadOnly();
        ProjectReferences = ParsedProjectReferenceList.AsReadOnly();
    }

    private void ParseProjectImport(XElement projectElement)
    {
        foreach (XAttribute ElementAttribute in projectElement.Attributes())
            ParseProjectAttribute(ElementAttribute);
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
        if (OutputTypeElement is not null)
            OutputType = OutputTypeElement.Value;

        XElement? UseWPFElement = projectElement.Element("UseWPF");
        if (UseWPFElement is not null)
            UseWpf = string.Equals(UseWPFElement.Value, "true", StringComparison.OrdinalIgnoreCase);

        XElement? UseWindowsFormsElement = projectElement.Element("UseWindowsForms");
        if (UseWindowsFormsElement is not null)
            UseWindowsForms = string.Equals(UseWindowsFormsElement.Value, "true", StringComparison.OrdinalIgnoreCase);

        if (ProjectType is ProjectType.Unknown or ProjectType.KnownToBeMSBuildFormat)
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
        if (LanguageVersionElement is not null)
            LanguageVersion = LanguageVersionElement.Value;

        XElement? NullableElement = projectElement.Element("Nullable");
        if (NullableElement is not null)
        {
            switch (NullableElement.Value.ToUpper(CultureInfo.InvariantCulture))
            {
                case "ENABLE":
                    Nullable = NullableAnnotation.Enable;
                    break;
                case "WARNINGS":
                    Nullable = NullableAnnotation.Warnings;
                    break;
                case "ANNOTATIONS":
                    Nullable = NullableAnnotation.Annotations;
                    break;
                case "DISABLE":
                    Nullable = NullableAnnotation.Disable;
                    break;
            }
        }

        XElement? NeutralLanguageElement = projectElement.Element("NeutralLanguage");
        if (NeutralLanguageElement is not null)
            NeutralLanguage = NeutralLanguageElement.Value;

        XElement? TreatWarningsAsErrorsElement = projectElement.Element("TreatWarningsAsErrors");
        if (TreatWarningsAsErrorsElement is not null)
            IsTreatWarningsAsErrors = string.Equals(TreatWarningsAsErrorsElement.Value, "true", StringComparison.OrdinalIgnoreCase);

        XElement? IsTestProjectElement = projectElement.Element("IsTestProject");
        if (IsTestProjectElement is not null)
            IsTestProject = string.Equals(IsTestProjectElement.Value, "true", StringComparison.OrdinalIgnoreCase);

        XElement? IsPackableElement = projectElement.Element("IsPackable");
        if (IsPackableElement is not null)
            IsNotPackable = string.Equals(IsPackableElement.Value, "false", StringComparison.OrdinalIgnoreCase);
    }

    private void ParseProjectElementVersion(XElement projectElement)
    {
        XElement? VersionElement = projectElement.Element("Version");
        if (VersionElement is not null)
            Version = VersionElement.Value;

        XElement? AssemblyVersionElement = projectElement.Element("AssemblyVersion");
        if (AssemblyVersionElement is not null)
            AssemblyVersion = AssemblyVersionElement.Value;

        XElement? FileVersionElement = projectElement.Element("FileVersion");
        if (FileVersionElement is not null)
            FileVersion = FileVersionElement.Value;
    }

    private void ParseProjectElementInfo(XElement projectElement)
    {
        XElement? AuthorElement = projectElement.Element("Authors");
        if (AuthorElement is not null)
            Author = AuthorElement.Value;

        XElement? DescriptionElement = projectElement.Element("Description");
        if (DescriptionElement is not null)
            Description = DescriptionElement.Value;

        XElement? CopyrightElement = projectElement.Element("Copyright");
        if (CopyrightElement is not null)
            Copyright = CopyrightElement.Value;

        XElement? RepositoryUrlElement = projectElement.Element("RepositoryUrl");
        if (RepositoryUrlElement is not null)
            RepositoryUrl = new Uri(RepositoryUrlElement.Value);

        XElement? ApplicationIconElement = projectElement.Element("ApplicationIcon");
        if (ApplicationIconElement is not null)
            ApplicationIcon = ApplicationIconElement.Value;
    }

    private void ParseProjectElementFrameworks(XElement projectElement)
    {
        XElement? TargetFrameworkElement = projectElement.Element("TargetFramework");
        if (TargetFrameworkElement is not null)
            TargetFrameworks = TargetFrameworkElement.Value;
        else
        {
            XElement? TargetFrameworksElement = projectElement.Element("TargetFrameworks");
            if (TargetFrameworksElement is not null)
                TargetFrameworks = TargetFrameworksElement.Value;
        }
    }

    private void ParseTargetFrameworks(List<Framework> parsedFrameworkList)
    {
        string[] Frameworks = TargetFrameworks.Split(';');

        foreach (string Framework in Frameworks)
            Project.ParseTargetFramework(parsedFrameworkList, Framework);
    }

    private static void ParseTargetFramework(List<Framework> parsedFrameworkList, string frameworkName)
    {
        if (frameworkName.StartsWith(NetStandardPattern, StringComparison.InvariantCulture))
            Project.ParseTargetFrameworkNetStandard(parsedFrameworkList, frameworkName);
        else if (frameworkName.StartsWith(NetCorePattern, StringComparison.InvariantCulture))
            Project.ParseTargetFrameworkNetCore(parsedFrameworkList, frameworkName);
        else if (frameworkName.StartsWith(NetFrameworkPattern, StringComparison.InvariantCulture))
            Project.ParseTargetFrameworkNetFramework(parsedFrameworkList, frameworkName);
    }

    private static void ParseTargetFrameworkNetStandard(List<Framework> parsedFrameworkList, string frameworkName)
    {
        string VersionString = frameworkName.Substring(NetStandardPattern.Length);

        if (ParseNetVersion(VersionString, out int Major, out int Minor))
        {
            Framework NewFramework = new(frameworkName, FrameworkType.NetStandard, Major, Minor);
            parsedFrameworkList.Add(NewFramework);
        }
    }

    private static void ParseTargetFrameworkNetCore(List<Framework> parsedFrameworkList, string frameworkName)
    {
        string VersionString = frameworkName.Substring(NetCorePattern.Length);

        if (ParseNetVersion(VersionString, out int Major, out int Minor))
        {
            Framework NewFramework = new(frameworkName, FrameworkType.NetCore, Major, Minor);
            parsedFrameworkList.Add(NewFramework);
        }
    }

    private static void ParseTargetFrameworkNetFramework(List<Framework> parsedFrameworkList, string frameworkName)
    {
        string FrameworkString = frameworkName;
        string MonikerString = string.Empty;
        FrameworkMoniker Moniker = FrameworkMoniker.none;

        foreach (FrameworkMoniker? MonikerValue in typeof(FrameworkMoniker).GetEnumValues())
        {
            if (MonikerValue == FrameworkMoniker.none)
                continue;

            string MonikerName = MonikerValue.Value.ToString();

            int MonikerIndex = FrameworkString.IndexOf($"-{MonikerName}", StringComparison.InvariantCulture);
            if (MonikerIndex > 0)
            {
                Moniker = MonikerValue.Value;
                MonikerString = FrameworkString.Substring(MonikerIndex + 1);
                FrameworkString = FrameworkString.Substring(0, MonikerIndex);
                break;
            }
        }

        string FrameworkVersionString = FrameworkString.Substring(NetFrameworkPattern.Length);

        if (ParseNetVersion(FrameworkVersionString, out int Major, out int Minor))
        {
            if (Moniker != FrameworkMoniker.none)
            {
                string MonikerVersionString = MonikerString.Substring(Moniker.ToString().Length);
                if (ParseMonikerVersion(MonikerVersionString, out int MonikerMajor, out int MonikerMinor))
                {
                    Framework NewFramework = new(frameworkName, FrameworkType.NetFramework, Major, Minor, Moniker, MonikerMajor, MonikerMinor);
                    parsedFrameworkList.Add(NewFramework);
                }
                else
                {
                    Framework NewFramework = new(frameworkName, FrameworkType.NetFramework, Major, Minor, Moniker);
                    parsedFrameworkList.Add(NewFramework);
                }
            }
            else
            {
                Framework NewFramework = new(frameworkName, FrameworkType.NetFramework, Major, Minor);
                parsedFrameworkList.Add(NewFramework);
            }
        }
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

    private static bool ParseMonikerVersion(string text, out int major, out int minor)
    {
        major = -1;
        minor = -1;

        string[] Versions = text.Split('.');
        if (Versions.Length == 2)
        {
            if (int.TryParse(Versions[0], out major) && int.TryParse(Versions[1], out minor))
                return true;
        }

        return false;
    }

    private void ParseProjectItemGroup(List<PackageReference> packageReferenceList, List<string> projectReferenceList, XElement projectElement)
    {
        ParseProjectElementEditorConfigLink(projectElement);
        ParseProjectElementPackageReference(packageReferenceList, projectElement);
        Project.ParseProjectElementProjectReference(projectReferenceList, projectElement);
    }

    private void ParseProjectElementEditorConfigLink(XElement projectElement)
    {
        XElement? NoneElement = projectElement.Element("None");
        if (NoneElement is not null)
        {
            bool IncludeEditorConfig = false;
            bool LinkEditorConfig = false;

            foreach (XAttribute Attribute in NoneElement.Attributes())
            {
                if (Attribute.Name == "Include" && Attribute.Value.EndsWith(".editorconfig", StringComparison.InvariantCulture))
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
        foreach (XElement PackageReferenceElement in projectElement.Descendants("PackageReference"))
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

    private static void ParseProjectElementProjectReference(List<string> projectReferenceList, XElement projectElement)
    {
        foreach (XElement ProjectReferenceElement in projectElement.Descendants("ProjectReference"))
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

    private static bool IsVersionCompatible(string longVersion, string shortVersion)
    {
        string ComparedText = (longVersion.Length <= shortVersion.Length) ? shortVersion : $"{shortVersion}.";
        return longVersion.StartsWith(ComparedText, StringComparison.InvariantCulture);
    }

    private string TargetFrameworks = string.Empty;
}
