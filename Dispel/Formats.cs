using Dispel.AST;
using Dispel.Parse;
using System;
using System.Collections.Generic;
using System.Text;

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

                case OutputFormat.HTML:
                    return "html";

                case OutputFormat.Wiki:
                    return "wiki.html";

                default:
                    throw new NotSupportedException();
            }
        }

        public static Func<SessionBody, string> GetGenerator(OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.Text:
                    return TextGenerator.Format;

                case OutputFormat.HTML:
                    return PageGenerator.Format;

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

                case "html":
                    return OutputFormat.HTML;

                case "wiki":
                    return OutputFormat.Wiki;

                default:
                    throw new NotSupportedException();
            }
        }

        public static readonly string[] Names = new[] { "text", "html", "wiki" };
    }
}
