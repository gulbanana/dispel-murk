using Dispel.Parse;
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
{0}";

        public static string Format(Node tree)
        {
            var content = string.Join("", Flatten(tree));
            return string.Format(template, content);
        }

        private static IEnumerable<string> Flatten(Node node)
        {
            switch (node.Type)
            {
                case NodeType.Empty:
                    break;

                case NodeType.Term:
                    yield return node.Text;
                    break;

                case NodeType.Sequence:
                    if (node.Subtype == LogNode.SEQ_USER)
                    {
                        yield return " &lt;";
                    }
                    else if (node.Subtype == LogNode.SEQ_LINE)
                    {
                        yield return "<p>";
                    }

                    foreach (var child in node.Children.SelectMany(Flatten))
                    {
                        yield return child;
                    }

                    if (node.Subtype == LogNode.SEQ_USER)
                    {
                        yield return "&gt; ";
                    }
                    else if (node.Subtype == LogNode.SEQ_LINE)
                    {
                        yield return "</p>";
                    }

                    break;
            }
        }
    }
}
