using static Dispel.Parse.Combinators;
using Dispel.Parse;

namespace Dispel
{
    public static class LineParser
    {
        public static readonly Parser ToggleBold = Term(@"\x02");
        public static readonly Parser ToggleItalic = Term(@"\x1D");
        public static readonly Parser ToggleUnderline = Term(@"\x1F");
        public static readonly Parser SetColor = Sequence(Term(@"\x03"), Term(@"(\d\d?,\d\d?|\d\d?)"));
        public static readonly Parser ResetColor = Term(@"\x03");
        public static readonly Parser FullReset = Term(@"\x0F");
        public static readonly Parser Attribute = Any(ToggleBold, ToggleItalic, ToggleUnderline, SetColor, ResetColor, FullReset);

        public static readonly Parser Text = Term(@"[^\x02\x03\x0F\x1D\x1F\r\n]+");
        public static readonly Parser Attributes = Set(Attribute);
        public static readonly Parser Run = Sequence(Attributes, Text);
        public static readonly Parser AttributedText = Sequence(Set(Run), Skip(Optional(Attributes)));

        public static readonly Parser Timestamp = Sequence(Skip(@"\["), Term(@"\d\d:\d\d"), Skip(@"\]"));
        public static readonly Parser Username = Sequence(Skip(@"<"), Skip(SetColor), Optional(Skip(@"@")), Term(@"[\w|\[\]]+"), Skip(FullReset), Skip(@">"));

        public static readonly Parser Line = Sequence(
            Optional(Skip(SetColor)), // occasionally you get a malformed color reset without the ^K
            Optional(Skip("03")), // occasionally you get a malformed color reset without the ^K
            Timestamp, 
            Skip(@"\s"), 
            Any(Username, Term(@"\*")), 
            Skip(@"\s"), 
            AttributedText);
    }
}
