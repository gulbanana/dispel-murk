using System;
using System.Collections.Generic;
using System.Linq;

namespace Dispel.Parse
{
    public class Node
    {
        public readonly NodeType Type;
        public readonly string Text;
        public readonly Node[] Children;

        public Node(NodeType type, string text = null, IEnumerable<Node> children = null)
        {
            Type = type;
            Text = text;
            Children = children == null ? Array.Empty<Node>() : children.ToArray();
        }

        public override string ToString()
        {
            var formattedSubtype = Type.ToString();
            var formattedChildren = (Children.Any() ? "{" + string.Join(", ", Children.Select(c => c.ToString())) + "}" : "");
            var formattedText = (!string.IsNullOrWhiteSpace(Text) ? string.Format("'{0}' ", Text) : "");
            return $"{formattedSubtype}: {formattedText}{formattedChildren}";
        }
    }
}