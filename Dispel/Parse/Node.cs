using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dispel.Parse
{
    //[DebuggerDisplay("{ToString()}")]
    public class Node
    {
        public readonly NodeType Type;
        public readonly int Subtype;
        public readonly string Text;
        public readonly IReadOnlyList<Node> Children;

        public Node(NodeType type, int subtype, string text = null, IEnumerable<Node> children = null)
        {
            Type = type;
            Subtype = subtype;
            Text = text;
            Children = (children ?? Enumerable.Empty<Node>()).Where(n => n.Type != NodeType.Empty).ToList();
        }

        public T Build<T>()
        {
            return (T)Build(typeof(T));
        }

        private object Build(Type t)
        {
            switch (Type)
            {
                // try string, <string>
                case NodeType.Terminal:
                    if (t == typeof(string))
                    {
                        return Text;
                    }
                    else
                    {
                        return Activator.CreateInstance(t, Text);
                    }

                // try <T,U>, <_, _>
                case NodeType.Production:
                    var pConstructor = t.GetConstructors().SingleOrDefault(c => c.GetParameters().Count() == Children.Count);
                    var pinfos = pConstructor.GetParameters();
                    var ps = new object[pinfos.Length];

                    if (pinfos.All(pinfo => pinfo.ParameterType == typeof(Node)))
                    {
                        for (var i = 0; i < Children.Count; i++)
                        {
                            ps[i] = Children[i];
                        }
                    }
                    else
                    {
                        for (var i = 0; i < Children.Count; i++)
                        {
                            ps[i] = Children[i].Build(pinfos[i].ParameterType);
                        }
                    }

                    return pConstructor.Invoke(ps);

                // try [T], [_]
                case NodeType.Repetition:
                    var rConstructor = t.GetConstructors().SingleOrDefault(c => c.GetParameters().Count() == 1 && typeof(IEnumerable).IsAssignableFrom(c.GetParameters().Single().ParameterType));
                    if (rConstructor != null)
                    {
                        var pinfo = rConstructor.GetParameters().Single();
                        var pt = pinfo.ParameterType.GetGenericArguments().Single();
                        var rps = Array.CreateInstance(pt, Children.Count); 
                        for (var i = 0; i < Children.Count; i++)
                        {
                            rps.SetValue(Children[i].Build(pt), i);
                        }
                        return rConstructor.Invoke(new[] { rps });
                    }

                    rConstructor = t.GetConstructor(new[] { typeof(IEnumerable<Node>) });
                    return rConstructor.Invoke(new[] { Children });

                case NodeType.Empty: 
                    throw new NotSupportedException("cannot build AST from empty node");

                default:
                    throw new NotImplementedException($"unexpected node type {Type}");
            }

        }

        public override string ToString()
        {
            var formattedSubtype = Subtype == 0 ? Type.ToString() : Type.ToString() + "(" + Subtype.ToString() + ")";
            var formattedChildren = (Children.Any() ? "{" + string.Join(", ", Children.Select(c => c.ToString())) + "}" : "");
            var formattedText = (!string.IsNullOrWhiteSpace(Text) ? string.Format("'{0}' ", Text) : "");
            return $"{formattedSubtype}: {formattedText}{formattedChildren}";
        }
    }
}