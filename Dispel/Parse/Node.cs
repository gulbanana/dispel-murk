using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dispel.Parse
{
    public class Node
    {
        public readonly NodeType Type;
        public readonly string Text;
        public readonly IReadOnlyList<Node> Children;

        public Node(NodeType type, string text = null, IEnumerable<Node> children = null)
        {
            Type = type;
            Text = text;
            if (children.Any(n => n.Type == NodeType.Empty)) throw new Exception();
            Children = (children ?? Enumerable.Empty<Node>()).Where(n => n.Type != NodeType.Empty).ToList();
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