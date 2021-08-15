using static Dispel.Parse.Combinators;
using Dispel.Parse;

namespace Dispel
{
    public static class LineParser
    {
        public static readonly Parser SetColor = Sequence(Term(@"\x03"), Term(@"(\d\d?,\d\d?|\d\d?)"));
        public static readonly Parser Toggle = Term(@"[\x03\x02\x1D\x1F\x0F]");
        public static readonly Parser Attribute = Any(SetColor, Toggle);

        public static readonly Parser Text = Term(@"[^\x02\x03\x0F\x1D\x1F\r\n]+");
        public static readonly Parser Attributes = Set(Attribute);
        public static readonly Parser Run = Sequence(Attributes, Text);
        public static readonly Parser AttributedText = Sequence(Set(Run), Skip(Optional(Attributes)));

        public static readonly Parser Timestamp = Term(@"\[(\d\d:\d\d)\]", m => m.Groups[1].Value);
        public static readonly Parser ColoredUsername = Term(@"<\x03(\d\d?,\d\d?|\d\d?)[@+]?([\w|\[\]]+)\x0F>", m => m.Groups[2].Value);
        public static readonly Parser UncoloredUsername = Term(@"<[@+]?([\w|\[\]]+)>", m => m.Groups[1].Value);
        public static readonly Parser Username = Any(UncoloredUsername, ColoredUsername);

        public static readonly Parser Line = Sequence(
            Optional(Skip(@"\x03?\d\d?")), // occasionally you get a malformed color reset without the ^K
            Timestamp, 
            Skip(@"\s"), 
            Any(
                Username, 
                Term(@"\*"), 
                Term(@"->")
            ), 
            Skip(@"\s"), 
            AttributedText);

        public static readonly Parser Directive = Sequence(
            Optional(Skip(Timestamp)),
            Skip(@"Session\s"),
            Term(@"Ident|Start|Close|Time"),
            Skip(@":\s"),
            Term(@".*"));

        public static readonly Parser Pragma = Sequence(
            Skip(@"\#"),
            Term(@"\w+"),
            Optional(Skip(@"\s")),
            Term(@".*"));

        public static readonly Parser Server = Skip(@"-");
    }
}
