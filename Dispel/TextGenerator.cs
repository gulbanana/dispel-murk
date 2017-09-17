using System;
using System.Collections.Generic;
using System.Linq;

namespace Dispel
{
    /// <summary>output a plaintext log file</summary>
    static class TextGenerator
    {
        public static string Format(Node tree)
        {
            return string.Join("", Flatten(tree));
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
                        yield return " <";
                    }

                    foreach (var child in node.Children.SelectMany(Flatten))
                    {
                        yield return child;
                    }

                    if (node.Subtype == LogNode.SEQ_USER)
                    {
                        yield return "> ";
                    }
                    else if (node.Subtype == LogNode.SEQ_LINE)
                    {
                        yield return Environment.NewLine;
                    }

                    break;

                default:
                    throw new Exception($"Unknown node type {node.Type}");
            }
        }
    }
}
