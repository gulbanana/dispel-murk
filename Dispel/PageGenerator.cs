using Dispel.Parse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dispel
{
    /// <summary>output HTML</summary>
    static class PageGenerator
    {
        private const string template = @"<!DOCTYPE html>
<meta charset=""utf-8"">
<title>Test Page</title>
<style>
body {
    font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Oxygen-Sans, Ubuntu, Cantarell, ""Helvetica Neue"", sans-serif;
    color: rgb(15, 15, 15);
}
p.line {
    margin: 0;
    padding: 0;
}
span.user {
    font-weight: bold;
}
span.timestamp {
    color: rgb(128, 128, 128);
}
</style>
";

        public static string Format(Node tree)
        {
            var content = string.Join("", Flatten(tree));
            return template + content;
        }

        private static IEnumerable<string> Flatten(Node node)
        {
            switch (node.Type)
            {
                case NodeType.Empty:
                    break;

                case NodeType.Term:
                    if (node.Subtype == LogNode.TERM_TIMESTAMP)
                    {
                        yield return $"<span class='timestamp'>{node.Text}</span>";
                    }
                    else
                    {
                        yield return node.Text;
                    }

                    break;

                case NodeType.Sequence:
                    if (node.Subtype == LogNode.SEQ_USER)
                    {
                        yield return " <span class='user'>&lt;";
                    }
                    else if (node.Subtype == LogNode.SEQ_LINE)
                    {
                        yield return "<p class='line'>";
                    }

                    foreach (var child in node.Children.SelectMany(Flatten))
                    {
                        yield return child;
                    }

                    if (node.Subtype == LogNode.SEQ_USER)
                    {
                        yield return "&gt;</span> ";
                    }
                    else if (node.Subtype == LogNode.SEQ_LINE)
                    {
                        yield return "</p>" + Environment.NewLine;
                    }

                    break;
            }
        }
    }
}
