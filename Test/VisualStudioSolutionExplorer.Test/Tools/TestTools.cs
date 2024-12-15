namespace VisualStudioSolutionExplorer.Test;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

internal static class TestTools
{
    public static string GetExecutingProjectRootPath()
    {
        Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        string? CurrentDirectory = Path.GetDirectoryName(ExecutingAssembly.Location);

        while (true)
        {
            string? ParentFolder = Path.GetDirectoryName(CurrentDirectory);
            string FileName = Path.GetFileName(CurrentDirectory)!;

            List<string> KnownFolderNames =
            [
                "net481",
                "net7.0",
                "net7.0-windows7.0",
                "net8.0",
                "net8.0-windows7.0",
                "net9.0",
                "net9.0-windows7.0",
                "Debug",
                "Release",
                "x64",
                "bin",
            ];

            if (KnownFolderNames.Contains(FileName))
                CurrentDirectory = ParentFolder;
            else
                break;
        }

        Debug.Assert(CurrentDirectory is not null);

        return CurrentDirectory!;
    }
}
