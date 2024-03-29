﻿using Dispel.AST;
using System;

namespace Dispel
{
    public static class Formats
    {
        public static string GetFileExtension(OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.Text:
                    return "txt";

                case OutputFormat.WebPage:
                    return "html";

                case OutputFormat.WebSite:
                    return "html";

                case OutputFormat.Wiki:
                    return "wiki.html";

                case OutputFormat.StrippedLog:
                    return "log";

                default:
                    throw new NotSupportedException();
            }
        }

        public static Func<Log, OutputFile[]> GetGenerator(OutputFormat format, DispelOptions options)
        {
            switch (format)
            {
                case OutputFormat.Text:
                    return TextGenerator.Format;

                case OutputFormat.WebPage:
                    return new WebGenerator(options).FormatPage;

                case OutputFormat.WebSite:
                    return new WebGenerator(options).FormatSite;

                case OutputFormat.Wiki:
                    return WikiGenerator.Format;

                case OutputFormat.StrippedLog:
                    return LogGenerator.Format;

                default:
                    throw new NotSupportedException();
            }
        }

        public static OutputFormat Parse(string name)
        {
            switch (name)
            {
                case "text":
                    return OutputFormat.Text;

                case "page":
                    return OutputFormat.WebPage;

                case "site":
                    return OutputFormat.WebSite;

                case "wiki":
                    return OutputFormat.Wiki;

                case "stripped":
                    return OutputFormat.StrippedLog;

                default:
                    throw new NotSupportedException();
            }
        }

        public static readonly string[] Names = new[] { "text", "page", "site", "wiki", "stripped" };
    }
}
