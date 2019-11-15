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
            var formatName = "site";
            var quiet = false;
            var single = false;

            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("o|outputFormat", ref formatName, "output format - (page|site|text|wiki|all)");
                syntax.DefineOption("q|quiet", ref quiet, "suppress output");
                syntax.DefineOption("s|single-file", ref single, "generate combined single-file output");
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
                foreach (var format in new[] { OutputFormat.WebPage, OutputFormat.Text, OutputFormat.Wiki })
                {
                    await ConvertAsync(format, logPaths, quiet, single);
                }
            }
            else
            {
                var format = Formats.Parse(formatName);
                await ConvertAsync(format, logPaths, quiet, single);
            }
        }

        private static async Task ConvertAsync(OutputFormat format, IReadOnlyList<string> logPaths, bool quiet, bool single)
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

                if (!single)
                {
                    using (var inputStream = File.OpenRead(inputFile))
                    {
                        var logs = await Engine.ConvertAsync(inputStream, format);
                        foreach (var log in logs)
                        {
                            await File.WriteAllTextAsync(log.Filename, log.Content);
                            if (!quiet) Console.WriteLine($"Created {log.Filename}.");
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
                            await Engine.ConvertSingleAsync(inputStream, outputStream, format);
                        }
                    }
                    if (!quiet) Console.WriteLine($"Created {outputFile}.");
                }
            }
        }
    }
}
