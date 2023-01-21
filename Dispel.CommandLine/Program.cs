using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dispel.CommandLine
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var command = new RootCommand
            {                
                new Option<bool>(new[]{"q", "quiet"}, "suppress output"),
                new Option<bool>(new[]{"s", "single-file"}, "generate combined single-file output"),
                new Option<bool>(new[]{"m", "multiple-file"}, "combine all files into a single input"),
                new Option<string>(new[]{"o", "output-format"}, () => "site", "(page|site|text|wiki|stripped|all)"),
                new Argument<string[]>("logs", "log files to convert") { Arity = ArgumentArity.ZeroOrMore }
            };
            
            command.Handler = CommandHandler.Create<bool, bool, bool, string, string[]>(async (quiet, single, multiple, outputFormat, logs) =>
            {
                if (!logs.Any())
                {
                    logs = Directory.EnumerateFiles(".", "*.log").ToArray();
                }

                if (!logs.Any())
                {
                    if (!quiet) Console.WriteLine("No logs to process!");
                    return;
                }

                if (outputFormat != "all" && !Formats.Names.Contains(outputFormat))
                {
                    if (!quiet) Console.WriteLine($"Unrecognised format {outputFormat}!");
                    return;
                }

                if (outputFormat == "all")
                {
                    foreach (var format in Enum.GetValues(typeof(OutputFormat)))
                    {
                        await ConvertAsync((OutputFormat)format, logs, quiet, single, multiple);
                    }
                }
                else
                {
                    var format = Formats.Parse(outputFormat);
                    await ConvertAsync(format, logs, quiet, single, multiple);
                }
            });

            await command.InvokeAsync(args);
        }

        private static async Task ConvertAsync(OutputFormat format, IReadOnlyList<string> logPaths, bool quiet, bool single, bool multiple)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("config.json", optional: true)
                .Build();

            var options = config.Get<DispelOptions>() ?? new DispelOptions();

            if (multiple)
            {
                var inputFiles = new List<string>();
                foreach (var logPath in logPaths)
                {
                    if (!File.Exists(logPath))
                    {
                        if (!quiet) Console.WriteLine($"File not found: {logPath}!");
                        continue;
                    }

                    var inputFile = Path.GetFullPath(logPath);
                    inputFiles.Add(inputFile);
                    if (!quiet) Console.WriteLine($"Processing {inputFile}...");
                }

                var inputStreams = inputFiles.Select(f => (name: Path.GetFileName(f), file: File.Open(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) as Stream)).ToList();

                var engine = new Engine(options);
                if (!single)
                {
                    var logs = await engine.ConvertAsync(format, inputStreams.ToArray());
                    foreach (var log in logs)
                    {
                        var existingContent = File.Exists(log.Filename) ? await File.ReadAllTextAsync(log.Filename) : null;
                        if (log.Content == null)
                        {
                            if (existingContent != null)
                            {
                                File.Delete(log.Filename);
                                if (!quiet) Console.WriteLine($"Removed {log.Filename}.");
                            }
                        }
                        else if (!log.Content.Equals(existingContent))
                        {
                            await File.WriteAllTextAsync(log.Filename, log.Content);
                            if (!quiet) Console.WriteLine($"{(existingContent == null ? "Created" : "Updated")} {log.Filename}.");
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }

                foreach (var stream in inputStreams)
                {
                    stream.file.Dispose();
                }
            }
            else
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

                    var engine = new Engine(options);
                    if (!single)
                    {
                        using (var inputStream = File.Open(inputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            var logs = await engine.ConvertAsync(format, Path.GetFileName(inputFile), inputStream);
                            foreach (var log in logs)
                            {
                                var existingContent = File.Exists(log.Filename) ? await File.ReadAllTextAsync(log.Filename) : null;
                                if (log.Content == null)
                                {
                                    if (existingContent != null)
                                    {
                                        File.Delete(log.Filename);
                                        if (!quiet) Console.WriteLine($"Removed {log.Filename}.");
                                    }
                                }
                                else if (!log.Content.Equals(existingContent))
                                {
                                    await File.WriteAllTextAsync(log.Filename, log.Content);
                                    if (!quiet) Console.WriteLine($"{(existingContent == null ? "Created" : "Updated")} {log.Filename}.");
                                }
                            }
                        }
                    }
                    else
                    {
                        var outputFile = Path.ChangeExtension(inputFile, Formats.GetFileExtension(format));
                        using (var inputStream = File.Open(inputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (var outputStream = File.OpenWrite(outputFile))
                            {
                                await engine.ConvertSingleAsync(Path.GetFileName(inputFile), inputStream, outputStream, format);
                            }
                        }
                        if (!quiet) Console.WriteLine($"Created {outputFile}.");
                    }
                }
            }
        }
    }
}
