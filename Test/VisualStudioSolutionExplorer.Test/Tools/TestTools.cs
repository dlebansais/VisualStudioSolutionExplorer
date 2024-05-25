namespace VisualStudioSolutionExplorer.Test;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public static class TestTools
{
    public static string GetExecutingProjectRootPath()
    {
        string? CurrentDirectory = Environment.CurrentDirectory;
        bool Continue = true;

        while (Continue)
        {
            string? ParentFolder = System.IO.Path.GetDirectoryName(CurrentDirectory);
            string? FileName = System.IO.Path.GetFileName(CurrentDirectory);

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
