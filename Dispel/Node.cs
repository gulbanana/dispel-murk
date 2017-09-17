using System.Collections.Generic;
using System.Linq;

namespace Dispel
{
    public class Node
    {
        public readonly NodeType Type;
        public readonly string Text;
        public readonly List<Node> Children;

        public Node(NodeType type, string text = null, IEnumerable<Node> children = null)
        {
            Type = type;
            Text = text;
            Children = (children ?? Enumerable.Empty<Node>()).ToList();
        }

        public override string ToString()
        {
            var formattedChildren = (Children.Any() ? "{" + string.Join(", ", Children.Select(c => c.ToString())) + "}" : "");
            var formattedText = (!string.IsNullOrWhiteSpace(Text) ? string.Format("'{0}' ", Text) : "");
            return string.Format($"{Type}: {formattedText}{formattedChildren}");
        }
    }
}