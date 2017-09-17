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

                case NodeType.Literal:
                    yield return node.Text;
                    break;

                case NodeType.Sequence:
                    foreach (var child in node.Children.SelectMany(Flatten))
                        yield return child;
                    break;
            }
        }
    }
}
