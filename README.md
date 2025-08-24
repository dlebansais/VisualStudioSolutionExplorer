# VisualStudioSolutionExplorer

Explore .sln files and their projects in a tree view.

## Usage

To start, create a `Solution` object providing the path to the solution file:

```csharp
using SlnExplorer;

string SolutionFileName = @"C:\Path\To\YourSolution.sln";
Solution NewSolution = new(SolutionFileName);
```

The constructor also accepts a name and `StreamReader`:

```csharp
Solution NewSolution = new("MySolution", new StreamReader(SolutionFileName));
```

This loads and parses the file. You can then access the projects within the solution:

```csharp
foreach (Project Item in NewSolution.ProjectList)
{
    /// ...
}
```

## Features

### `Solution` class

| Property           | Comment                                 |
|--------------------|-----------------------------------------|
| `Name`             | The solution name.                      |
| `SolutionFileName` | The full path to the solution file.     |
| `ProjectList`      | A list of all projects in the solution. |

### `Project` class

| Property                   | Comment                                                          |
|----------------------------|------------------------------------------------------------------|
| `ParentSolution`           | The parent solution.                                             |
| `ProjectName`              | The project name.                                                |
| `RelativePath`             | The project path, relative to the solution path.                 |
| `ProjectGuid`              | The project GUID.                                                |
| `ProjectType`              | The project type (Library, Console...)                           |
| `Extension`                | The extension (typically, csproj).                               |
| `Dependencies`             | The dependencies, as strings.                                    |
| `ProjectReferences`        | The project references, as strings.                              |
| `ParentProjectGuid`        | The parent project GUID.                                         |
| `ProjectConfigurations`    | The project configurations (Debug, Release...)                   |
| `DependencyLevel`          | The dependency level, `-1` if none.                              |
| `IsStaticLibrary`          | Is the project a static library?                                 |
| `SdkType`                  | The Sdk type (Windows Desktop Sdk...)                            |
| `OutputType`               | The project output type (Exe...)                                 |
| `UseWpf`                   | Does the projcet uses WPF?                                       |
| `UseWindowsForms`          | Does the project uses Windows Forms?                             |
| `HasVersion`               | Does the project have a version?                                 |
| `Version`                  | The project version.                                             |
| `IsAssemblyVersionValid`   | Does the project have a valid assembly version?                  |
| `AssemblyVersion`          | The assembly version.                                            |
| `IsFileVersionValid `      | Does the project have a valid file version?                      |
| `FileVersion`              | The file version.                                                |
| `Author`                   | The project author(s).                                           |
| `Description`              | The project description.                                         |
| `Copyright`                | The project copyright text.                                      |
| `HasRepositoryUrl`         | Does the project have a repository URL?                          |
| `RepositoryUrl`            | The project repository URL (`null` if none).                     |
| `ApplicationIcon`          | The project application icon.                                    |
| `FrameworkList`            | The list of parsed project frameworks (net48, net8.0, ...)       |
| `HasTargetFrameworks`      | Does the project have one or more target frameworks.             |
| `LanguageVersion`          | The language version.                                            |
| `Nullable`                 | The project nullable values setting.                             |
| `NeutralLanguage`          | The neutral langauge.                                            |
| `IsTreatWarningsAsErrors`  | Are project warnings treated as errors?                          |
| `IsTestProject`            | Is the project a test project?                                   |
| `IsNotPackable`            | Is the project not packable?                                     |
| `IsEditorConfigLinked`     | Does the the project have links to the solution's editor config. |
| `PackageReferenceList`     | The list of package references.                                  |
| `IsProjectWithOutput`      | Does the project produces an output?                             |
| `PackageIcon`              | The project package icon.                                        |
| `PackageLicenseExpression` | The project package license.                                     |
| `PackageReadmeFile`        | The project package readme.                                      |

### `Configuration` class

| Property            | Comment                                 |
|---------------------|-----------------------------------------|
| `Project`           | The parent project.                     |
| `ConfigurationName` | The configuration name.                 |
| `PlatformName`      | The platform name.                      |
| `IncludeInBuild`    | Is the configuration included in build? |

### `Framework` class

| Property       | Comment                                          |
|----------------|--------------------------------------------------|
| `Name`         | The framework name.                              |
| `Type`         | The framework type (.NET Standard, .NET Core...) |
| `Major `       | The framework major version.                     |
| `Minor`        | The framework minor version.                     |
| `Moniker`      | The target framework moniker (TFM).              |
| `MonikerMajor` | The moniker major version.                       |
| `MonikerMinor` | The moniker minor version.                       |

### `PackageReference` class

| Property             | Comment                              |
|----------------------|--------------------------------------|
| `Project`            | The parent project.                  |
| `Name`               | The package name.                    |
| `Version`            | The package version.                 |
| `Condition`          | The package condition, in plain text |
| `IsAllPrivateAssets` | Are all assets private?              |
