﻿namespace SlnExplorer;

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

        List<Framework> ParsedFrameworkList = [];

        if (TargetFrameworks.Length > 0)
            ParseTargetFrameworks(ParsedFrameworkList);

        FrameworkList = ParsedFrameworkList.AsReadOnly();

        List<PackageReference> ParsedPackageReferenceList = [];
        List<string> ParsedProjectReferenceList = [];
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
        _ = SetPropertyFromElement(projectElement, "OutputType", value => OutputType = value);
        _ = SetPropertyFromElement(projectElement, "UseWPF", value => UseWpf = IsStringTrue(value));
        _ = SetPropertyFromElement(projectElement, "UseWindowsForms", value => UseWindowsForms = IsStringTrue(value));

        bool IsProjectWithOutput = false;
        IsProjectWithOutput |= ProjectType == ProjectType.Unknown;
        IsProjectWithOutput |= ProjectType == ProjectType.KnownToBeMSBuildFormat;

        if (IsProjectWithOutput)
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
        _ = SetPropertyFromElement(projectElement, "LangVersion", value => LanguageVersion = value);

        XElement? NullableElement = projectElement.Element("Nullable");
        if (NullableElement is not null)
        {
            Nullable = NullableElement.Value.ToUpper(CultureInfo.InvariantCulture) switch
            {
                "ENABLE" => NullableAnnotation.Enable,
                "WARNINGS" => NullableAnnotation.Warnings,
                "ANNOTATIONS" => NullableAnnotation.Annotations,
                "DISABLE" => NullableAnnotation.Disable,
                _ => NullableAnnotation.None,
            };
        }

        _ = SetPropertyFromElement(projectElement, "NeutralLanguage", value => NeutralLanguage = value);
        _ = SetPropertyFromElement(projectElement, "TreatWarningsAsErrors", value => IsTreatWarningsAsErrors = IsStringTrue(value));
        _ = SetPropertyFromElement(projectElement, "IsTestProject", value => IsTestProject = IsStringTrue(value));
        _ = SetPropertyFromElement(projectElement, "IsPackable", value => IsNotPackable = IsStringFalse(value));
    }

    private void ParseProjectElementVersion(XElement projectElement)
    {
        _ = SetPropertyFromElement(projectElement, "Version", value => Version = value);
        _ = SetPropertyFromElement(projectElement, "AssemblyVersion", value => AssemblyVersion = value);
        _ = SetPropertyFromElement(projectElement, "FileVersion", value => FileVersion = value);
    }

    private void ParseProjectElementInfo(XElement projectElement)
    {
        _ = SetPropertyFromElement(projectElement, "Authors", value => Author = value);
        _ = SetPropertyFromElement(projectElement, "Description", value => Description = value);
        _ = SetPropertyFromElement(projectElement, "Copyright", value => Copyright = value);
        _ = SetPropertyFromElement(projectElement, "RepositoryUrl", value => RepositoryUrl = new Uri(value));
        _ = SetPropertyFromElement(projectElement, "ApplicationIcon", value => ApplicationIcon = value);
        _ = SetPropertyFromElement(projectElement, "PackageIcon", value => PackageIcon = value);
        _ = SetPropertyFromElement(projectElement, "PackageLicenseExpression", value => PackageLicenseExpression = value);
        _ = SetPropertyFromElement(projectElement, "PackageReadmeFile", value => PackageReadmeFile = value);
    }

    private void ParseProjectElementFrameworks(XElement projectElement)
    {
        if (!SetPropertyFromElement(projectElement, "TargetFramework", value => TargetFrameworks = value))
            _ = SetPropertyFromElement(projectElement, "TargetFrameworks", value => TargetFrameworks = value);
    }

    private static bool SetPropertyFromElement(XElement element, string key, Action<string> setter)
    {
        XElement? Item = element.Element(key);
        if (Item is not null)
        {
            setter(Item.Value);
            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool IsStringTrue(string value)
        => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

    private static bool IsStringFalse(string value)
        => string.Equals(value, "false", StringComparison.OrdinalIgnoreCase);

    private void ParseTargetFrameworks(List<Framework> parsedFrameworkList)
    {
        string[] Frameworks = TargetFrameworks.Split(';');

        foreach (string Framework in Frameworks)
            ParseTargetFramework(parsedFrameworkList, Framework);
    }

    private static void ParseTargetFramework(List<Framework> parsedFrameworkList, string frameworkName)
    {
        if (frameworkName.StartsWith(NetStandardPattern, StringComparison.InvariantCulture))
            ParseTargetFrameworkNetStandard(parsedFrameworkList, frameworkName);
        else if (frameworkName.StartsWith(NetCorePattern, StringComparison.InvariantCulture))
            ParseTargetFrameworkNetCore(parsedFrameworkList, frameworkName);
        else if (frameworkName.StartsWith(NetFrameworkPattern, StringComparison.InvariantCulture))
            ParseTargetFrameworkNetFramework(parsedFrameworkList, frameworkName);
    }

    private static void ParseTargetFrameworkNetStandard(List<Framework> parsedFrameworkList, string frameworkName)
    {
        string VersionString = frameworkName[NetStandardPattern.Length..];

        if (ParseNetVersion(VersionString, out int Major, out int Minor))
        {
            Framework NewFramework = new(frameworkName, FrameworkType.NetStandard, Major, Minor);
            parsedFrameworkList.Add(NewFramework);
        }
    }

    private static void ParseTargetFrameworkNetCore(List<Framework> parsedFrameworkList, string frameworkName)
    {
        string VersionString = frameworkName[NetCorePattern.Length..];

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
                MonikerString = FrameworkString[(MonikerIndex + 1)..];
                FrameworkString = FrameworkString[..MonikerIndex];
                break;
            }
        }

        string FrameworkVersionString = FrameworkString[NetFrameworkPattern.Length..];

        if (ParseNetVersion(FrameworkVersionString, out int Major, out int Minor))
        {
            if (Moniker != FrameworkMoniker.none)
            {
                string MonikerVersionString = MonikerString[Moniker.ToString().Length..];
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
            if (Version.Length > 1 && int.TryParse(Version[..1], out major) && int.TryParse(Version[1..], out minor))
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
        ParseProjectElementProjectReference(projectReferenceList, projectElement);
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
            bool IsAllPrivateAssets = false;

            foreach (XAttribute Attribute in PackageReferenceElement.Attributes())
            {
                if (Attribute.Name == "Include")
                    Name = Attribute.Value;

                if (Attribute.Name == "Version")
                    Version = Attribute.Value;

                if (Attribute.Name == "Condition")
                    Condition = Attribute.Value;

                if (Attribute.Name == "PrivateAssets")
                    IsAllPrivateAssets = IsAll(Attribute.Value);
            }

            foreach (XElement PackageReferenceItemElement in PackageReferenceElement.Elements())
                if (PackageReferenceItemElement.Name == "PrivateAssets")
                    IsAllPrivateAssets = IsAll(PackageReferenceItemElement.Value);

            if (Name.Length > 0 && Version.Length > 0)
            {
                PackageReference NewPackageReference = new(this, Name, Version, Condition, IsAllPrivateAssets);
                packageReferenceList.Add(NewPackageReference);
            }
        }

        static bool IsAll(string value)
        {
            return value.Equals("All", StringComparison.OrdinalIgnoreCase);
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
