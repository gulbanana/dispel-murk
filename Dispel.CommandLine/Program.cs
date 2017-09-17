using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;

namespace Dispel.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            IReadOnlyList<string> logFiles = Array.Empty<string>();

            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineParameterList("logs", ref logFiles, "log files to convert");
            });

            if (!logFiles.Any())
            {
                logFiles = Directory.EnumerateFiles(".", "*.log").ToArray();
            }

            foreach (var logFile in logFiles)
            {
                var fullPath = Path.GetFullPath(logFile);
                Console.WriteLine($"Processing {fullPath}...");
            }
        }
    }
}
