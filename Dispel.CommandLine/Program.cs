using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dispel.CommandLine
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IReadOnlyList<string> logPaths = Array.Empty<string>();
            var format = "html";

            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("f|format", ref format, "output format - 'html' or 'text'");
                syntax.DefineParameterList("logs", ref logPaths, "log files to convert");
            });

            if (format != "html" && format != "text")
            {
                Console.WriteLine($"Unrecognised format {format}!");
                return;
            }

            if (!logPaths.Any())
            {
                logPaths = Directory.EnumerateFiles(".", "*.log").ToArray();
            }

            if (!logPaths.Any())
            {
                Console.WriteLine("No logs to process!");
                return;
            }

            foreach (var logPath in logPaths)
            {
                if (!File.Exists(logPath))
                {
                    Console.WriteLine($"File not found: {logPath}!");
                    continue;
                }

                var inputFile = Path.GetFullPath(logPath);
                Console.WriteLine($"Processing {inputFile}...");

                var outputFile = Path.ChangeExtension(inputFile, format == "html" ? "html" : "txt");
                using (var inputStream = File.OpenRead(inputFile))
                {
                    using (var outputStream = File.OpenWrite(outputFile))
                    {
                        await Engine.ConvertAsync(inputStream, outputStream, format == "html" ? OutputFormat.HTML : OutputFormat.Text);
                    }
                }

                Console.WriteLine($"Created {outputFile}.");
            }
        }
    }
}
