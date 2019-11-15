using Dispel.AST;
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

                default:
                    throw new NotSupportedException();
            }
        }

        public static Func<Log, OutputFile[]> GetGenerator(OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.Text:
                    return TextGenerator.Format;

                case OutputFormat.WebPage:
                    return WebGenerator.FormatPage;

                case OutputFormat.WebSite:
                    return WebGenerator.FormatSite;

                case OutputFormat.Wiki:
                    return WikiGenerator.Format;

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

                default:
                    throw new NotSupportedException();
            }
        }

        public static readonly string[] Names = new[] { "text", "page", "site", "wiki" };
    }
}
