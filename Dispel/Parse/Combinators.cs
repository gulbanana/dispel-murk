﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dispel.Parse
{
    public static class Combinators
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

        public static Parser Term(string pattern, Func<Match, string> extract)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");

            var regex = new Regex("^" + pattern, _RegexOpts);

            return text =>
            {
                var m = Consume(ref text, regex);
                if (m == null) return ParseResult.Failure(text, pattern);

                return ParseResult.Success(new Node(NodeType.Terminal, extract(m)), text);
            };
        }

        public static Parser Term(string pattern) => Term(pattern, m => m.Value);

        public static Parser Optional(Parser p)
        {
            return text =>
            {
                var r = p(text);
                if (r.IsSuccess)
                    return r;
                else
                    return ParseResult.None(r.Remainder);
            };
        }

        public static Parser Optional(Parser p, string @default)
        {
            return text =>
            {
                var r = p(text);
                if (r.IsSuccess)
                    return r;
                else
                    return ParseResult.Success(new Node(NodeType.Terminal, @default), r.Remainder);
            };
        }

        public static Parser Sequence(params Parser[] parsers)
        {
            return text =>
            {
                var nodes = new List<Node>();

                foreach (var p in parsers)
                {
                    var r = p(text);
                    if (!r.IsSuccess) return r;
                    if (r.Tree.Type != NodeType.Empty) nodes.Add(r.Tree);
                    text = r.Remainder;
                }

                if (!nodes.Any())
                {
                    return ParseResult.None(text);
                }
                else if (nodes.Count() > 1)
                {
                    return ParseResult.Success(new Node(NodeType.Production, null, nodes), text);
                }
                else
                {
                    return ParseResult.Success(nodes.Single(), text);
                }
            };
        }

        public static Parser Decorated(int extractAt, params Parser[] parsers)
        {
            return text =>
            {
                var nodes = new List<Node>();

                foreach (var p in parsers)
                {
                    var r = p(text);
                    if (!r.IsSuccess) return r;
                    if (r.Tree.Type != NodeType.Empty) nodes.Add(r.Tree);
                    text = r.Remainder;
                }

                if (!nodes.Any())
                {
                    return ParseResult.None(text);
                }
                else
                {
                    return ParseResult.Success(nodes.ElementAt(extractAt), text);
                }
            };
        }

        /// <summary>0 or more</summary>
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

                return ParseResult.Success(new Node(NodeType.Repetition, null, nodes), text);
            };
        }

        /// <summary>1 or more</summary>
        public static Parser RequiredSet(Parser parser)
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
                {
                    return ParseResult.Failure(text, $"SET OF ({parser("").Expected})");
                }
                else
                {
                    return ParseResult.Success(new Node(NodeType.Repetition, null, nodes), text);
                }
            };
        }

        public static Parser Any(params Parser[] parsers)
        {
            return text =>
            {
                ParseResult r;
                var failures = new List<string>();
                for (var i = 0; i < parsers.Length; i++ )
                {
                    var p = parsers[i];
                    r = p(text);
                    if (r.IsSuccess)
                    {
                        return ParseResult.Tagged(r.Tree, r.Remainder, i+1);
                    }
                    else
                    {
                        failures.Add(r.Expected);
                    }
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
                {
                    return r;
                }
                else
                {
                    return ParseResult.None(r.Remainder);
                }
            };
        }

        public static Parser Skip(string pattern)
        {
            return Skip(Term(pattern));
        }
    }
}