using System.Collections.Generic;
using System.Linq;

namespace Dispel
{
    public class Node
    {
        public readonly NodeType Type;
        public readonly int Subtype;
        public readonly string Text;
        public readonly List<Node> Children;

        public Node(NodeType type, int subtype, string text = null, IEnumerable<Node> children = null)
        {
            Type = type;
            Subtype = subtype;
            Text = text;
            Children = (children ?? Enumerable.Empty<Node>()).ToList();
        }

        public override string ToString()
        {
            var formattedSubtype = Subtype == 0 ? Type.ToString() : Type.ToString() + "(" + Subtype.ToString() + ")";
            var formattedChildren = (Children.Any() ? "{" + string.Join(", ", Children.Select(c => c.ToString())) + "}" : "");
            var formattedText = (!string.IsNullOrWhiteSpace(Text) ? string.Format("'{0}' ", Text) : "");
            return string.Format($"{formattedSubtype}: {formattedText}{formattedChildren}");
        }
    }
}