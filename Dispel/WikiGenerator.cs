using Dispel.Parse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dispel
{
    /// <summary>output special HTML for the Obsidian Portal wiki</summary>
    static class WikiGenerator
    {
        public static string Format(Node tree)
        {
            return string.Join("", Flatten(tree));
        }

        private static IEnumerable<string> Flatten(Node node)
        {
            switch (node.Type)
            {
                case NodeType.Terminal:
                    if (node.Subtype == LogNode.TERM_TIMESTAMP)
                    {
                        yield return $"<span style='font-family:monospace;'><span style='color:#a9a9a9;'>{node.Text}</span>";
                    }
                    else
                    {
                        yield return node.Text;
                    }

                    break;

                case NodeType.Production:
                case NodeType.Repetition:
                    if (node.Subtype == LogNode.SEQ_USER)
                    {
                        yield return "&nbsp;<b>&lt;";
                    }

                    foreach (var child in node.Children.SelectMany(Flatten))
                    {
                        yield return child;
                    }

                    if (node.Subtype == LogNode.SEQ_USER)
                    {
                        yield return "&gt;</b>";
                        yield return "".PadRight(9 - node.Children.Single(n => n.Type != NodeType.Empty).Text.Length).Replace(" ", "&nbsp;");
                        yield return "</span>";
                    }
                    else if (node.Subtype == LogNode.SEQ_LINE)
                    {
                        yield return "<br>" + Environment.NewLine;
                    }

                    break;
            }
        }
    }
}
