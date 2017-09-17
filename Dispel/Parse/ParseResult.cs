namespace Dispel.Parse
{
    public class ParseResult
    {
        public static ParseResult Success(Node n, string t) => new ParseResult(true, n, t, null);

        public static ParseResult Failure(string remainder, string expected) => new ParseResult(false, null, remainder, expected);

        public static ParseResult None(string t) => new ParseResult(true, new Node(NodeType.Empty, 0, ""), t, null);

        public readonly Node Tree;
        public readonly string Remainder;
        public readonly bool IsSuccess;
        public readonly string Expected;

        private ParseResult(bool isSuccess, Node tree, string remainder, string expected)
        {
            Tree = tree;
            Remainder = remainder;
            IsSuccess = isSuccess;
            Expected = expected;
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