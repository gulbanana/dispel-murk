using static Dispel.Parse.Combinators;
using Dispel.Parse;

namespace Dispel
{
    public static class DirectiveParser
    {
        public static readonly Parser MircDirective = Sequence(
            Skip(@"Session\s"),
            Term(@"Ident|Start|Close|Time"),
            Skip(@":\s"),
            Term(@".*"));

        public static readonly Parser DMDirective = Sequence(
            Skip(@"\#"),
            Term(@"\w+"),
            Optional(Skip(@"\s")),
            Term(@".*"));
    }
}
