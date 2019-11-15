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
            var formatName = "html";
            var quiet = false;
            var multi = false;

            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("o|outputFormat", ref formatName, "output format - (html|text|wiki|all)");
                syntax.DefineOption("q|quiet", ref quiet, "suppress output");
                syntax.DefineOption("m|multi-session", ref multi, "generate multiple outputs");
                syntax.DefineParameterList("logs", ref logPaths, "log files to convert");
            });

            if (!logPaths.Any())
            {
                logPaths = Directory.EnumerateFiles(".", "*.log").ToArray();
            }

            if (!logPaths.Any())
            {
                if (!quiet) Console.WriteLine("No logs to process!");
                return;
            }

            if (formatName != "all" && !Formats.Names.Contains(formatName))
            {
                if (!quiet) Console.WriteLine($"Unrecognised format {formatName}!");
                return;
            }

            if (formatName == "all")
            {
                foreach (var format in new[] { OutputFormat.HTML, OutputFormat.Text, OutputFormat.Wiki })
                {
                    await ConvertAsync(format, logPaths, quiet, multi);
                }
            }
            else
            {
                var format = Formats.Parse(formatName);
                await ConvertAsync(format, logPaths, quiet, multi);
            }
        }

        private static async Task ConvertAsync(OutputFormat format, IReadOnlyList<string> logPaths, bool quiet, bool multi)
        {
            foreach (var logPath in logPaths)
            {
                if (!File.Exists(logPath))
                {
                    if (!quiet) Console.WriteLine($"File not found: {logPath}!");
                    continue;
                }

                var inputFile = Path.GetFullPath(logPath);
                if (!quiet) Console.WriteLine($"Processing {inputFile}...");

                if (multi)
                {
                    using (var inputStream = File.OpenRead(inputFile))
                    {
                        var logs = await Engine.ConvertMultisessionAsync(inputStream, format);
                        int idx = 0;
                        foreach (var log in logs)
                        {
                            var outputFile = $"{Path.GetFileNameWithoutExtension(inputFile)}-{idx++}.{Formats.GetFileExtension(format)}";
                            await File.WriteAllTextAsync(outputFile, log);
                            if (!quiet) Console.WriteLine($"Created {outputFile}.");
                        }
                    }
                }
                else
                {
                    var outputFile = Path.ChangeExtension(inputFile, Formats.GetFileExtension(format));
                    using (var inputStream = File.OpenRead(inputFile))
                    {
                        using (var outputStream = File.OpenWrite(outputFile))
                        {
                            await Engine.ConvertAsync(inputStream, outputStream, format);
                        }
                    }
                    if (!quiet) Console.WriteLine($"Created {outputFile}.");
                }
            }
        }
    }
}
