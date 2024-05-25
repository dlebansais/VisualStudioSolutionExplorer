namespace VisualStudioSolutionExplorer.Test;

using System.Diagnostics;
using System.IO;
using System.Reflection;

public static class TestTools
{
    public static string GetExecutingProjectRootPath()
    {
        Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        string? CurrentDirectory = Path.GetDirectoryName(ExecutingAssembly.Location);
        bool Continue = true;

        while (Continue)
        {
            string? ParentFolder = Path.GetDirectoryName(CurrentDirectory);
            string FileName = Path.GetFileName(CurrentDirectory)!;

            switch (FileName)
            {
                case "net481":
                case "net7.0":
                case "net7.0-windows7.0":
                case "net8.0":
                case "net8.0-windows7.0":
                case "Debug":
                case "Release":
                case "x64":
                case "bin":
                    CurrentDirectory = ParentFolder;
                    continue;
                default:
                    Continue = false;
                    break;
            }
        }

        Debug.Assert(CurrentDirectory is not null);

        return CurrentDirectory!;
    }
}
