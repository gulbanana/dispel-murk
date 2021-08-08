namespace Dispel.Parse
{
    public class ParseResult
    {
        public static ParseResult Success(Node n, string t) => new ParseResult(true, n, t, null, 0);
        
        public static ParseResult Tagged(Node n, string t, int i) => new ParseResult(true, n, t, null, i);

        public static ParseResult Failure(string remainder, string expected) => new ParseResult(false, null, remainder, expected, 0);

        public static ParseResult None(string t) => new ParseResult(true, new Node(NodeType.Empty, ""), t, null, 0);

        public readonly Node Tree;
        public readonly string Remainder;
        public readonly bool IsSuccess;
        public readonly string Expected;
        public readonly int Tag;

        private ParseResult(bool isSuccess, Node tree, string remainder, string expected, int tag)
        {
            Tree = tree;
            Remainder = remainder;
            IsSuccess = isSuccess;
            Expected = expected;
            Tag = tag;
        }

        public override string ToString()
        {
            if (IsSuccess)
            {
                if (!string.IsNullOrEmpty(Remainder))
                {
                    return $"Success: {Tree}";
                }
                else
                { 
                    return $"Success: {Tree}, {Remainder}";
                }
            }
            else
            {
                return $"Failure: expected '{Expected}' but found '{Remainder}'";
            }
        }
    }
}