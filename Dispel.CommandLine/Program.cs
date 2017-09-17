using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;

namespace Dispel.CommandLine
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            IReadOnlyList<string> logPaths = Array.Empty<string>();

            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineParameterList("logs", ref logPaths, "log files or directories to convert");
            });

            if (!logPaths.Any())
            {
                logPaths = Directory.EnumerateFiles(".", "*.log").ToArray();
            }

            foreach (var logPath in logPaths)
            {
                if (!File.Exists(logPath))
                {
                    Console.WriteLine($"File not found: {logPath}");
                    continue;
                }

                var logFile = Path.GetFullPath(logPath);
                Console.WriteLine($"Processing {logFile}...");

                var htmlFile = Path.ChangeExtension(logFile, "html");
                using (var inputStream = File.OpenRead(logFile))
                {
                    using (var outputStream = File.OpenWrite(htmlFile))
                    {
                        Engine.Convert(inputStream, outputStream);
                    }
                }
            }
        }
    }
}
