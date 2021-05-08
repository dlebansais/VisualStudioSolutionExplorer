namespace TestEaslyNumber
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using SlnExplorer;

    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 1)
                return -1;

            string SolutionNameArg = args[0];

            Debug.WriteLine($"Current directory: {Environment.CurrentDirectory}");

            string[] Directories = Directory.GetDirectories(@"C:\Projects");
            foreach (string Directory in Directories)
            {
                if (!Directory.EndsWith(SolutionNameArg))
                    continue;

                string RootPath = @$"{Directory}\";
                string SolutionName = Path.GetFileName(Directory) + ".sln";
                if (File.Exists($"{RootPath}{SolutionName}"))
                {
                    Solution NewSolution = new Solution(Path.Combine(RootPath, SolutionName));
                    foreach (Project Project in NewSolution.ProjectList)
                    {
                        if (Project.ProjectType == ProjectType.Unknown)
                        {
                            string ProjectPath = Path.Combine(RootPath, Project.RelativePath);
                            Project.LoadDetails(ProjectPath);
                        }

                        Project.CheckVersionConsistency(out _);
                    }
                }
            }

            return 0;
        }
    }
}
