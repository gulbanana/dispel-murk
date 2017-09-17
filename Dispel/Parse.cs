using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dispel
{
    public delegate ParseResult Parser(string input);

    static class Parse
    {
        const RegexOptions _RegexOpts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace;

        private static Match Consume(ref string text, Regex pattern)
        {
            var m = pattern.Match(text);

            if (m.Success)
            {
                text = text.Substring(m.Value.Length);
                return m;
            }
            else
            {
                return null;
            }
        }

        public static Parser Term(string pattern, Func<Match, string> extract, NodeType type = NodeType.Literal)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");

            var regex = new Regex("^" + pattern, _RegexOpts);

            return text =>
            {
                var m = Consume(ref text, regex);
                if (m == null) return ParseResult.Failure(text, pattern);

                return ParseResult.Success(new Node(type, extract(m)), text);
            };
        }

        public static Parser Term(string pattern, NodeType type = NodeType.Literal)
        {
            return Term(pattern, m => m.Value, type);
        }

        public static Parser Optional(Parser p)
        {
            return text =>
            {
                var r = p(text);
                if (r.IsSuccess)
                    return r;
                else
                    return ParseResult.Empty(r.Remainder);
            };
        }

        public static Parser Sequence(Func<IEnumerable<Node>, Node> reduce, params Parser[] parsers)
        {
            return text =>
            {
                var nodes = new List<Node>();

                foreach (var p in parsers)
                {
                    var r = p(text);
                    if (!r.IsSuccess) return r;
                    nodes.Add(r.Tree);
                    text = r.Remainder;
                }

                return ParseResult.Success(reduce(nodes), text);
            };
        }

        public static Parser Sequence(params Parser[] parsers)
        {
            return Sequence(nodes => new Node(NodeType.Sequence, null, nodes), parsers);
        }

        public static Parser Set(Parser parser)
        {
            return text =>
            {
                var nodes = new List<Node>();

                ParseResult r;
                do
                {
                    r = parser(text);
                    if (r.IsSuccess)
                    {
                        nodes.Add(r.Tree);
                        text = r.Remainder;
                    }
                } while (r.IsSuccess);

                if (nodes.Count == 0)
                    return ParseResult.Failure(text, $"SET OF ({parser("").Expected})");
                else if (nodes.Count == 1)
                    return ParseResult.Success(nodes.Single(), text);
                else
                    return ParseResult.Success(new Node(NodeType.Sequence, null, nodes), text);
            };
        }

        public static Parser Any(params Parser[] parsers)
        {
            return text =>
            {
                ParseResult r;
                var failures = new List<string>();
                foreach (var p in parsers)
                {
                    r = p(text);
                    if (r.IsSuccess)
                        return r;
                    else
                        failures.Add(r.Expected);
                }

                return ParseResult.Failure(text, $"ANY OF ({string.Join(",", failures)})");
            };
        }

        public static Parser Skip(Parser p)
        {
            return text =>
            {
                var r = p(text);
                if (!r.IsSuccess)
                    return r;
                else
                    return ParseResult.Empty(r.Remainder);
            };
        }
    }
}