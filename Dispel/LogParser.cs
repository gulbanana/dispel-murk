using static Dispel.Parse.Combinators;
using static Dispel.LogNode;
using Dispel.Parse;

namespace Dispel
{
    public static class LogParser
    {
        public static readonly Parser ToggleBold = Skip(Term(@"\x02"));
        public static readonly Parser ToggleItalic = Skip(Term(@"\x1D"));
        public static readonly Parser ToggleUnderline = Skip(Term(@"\x1F"));
        public static readonly Parser SetColor = Skip(Term(@"\x03(\d\d?,\d\d?|\d\d?|)"));
        public static readonly Parser FullReset = Skip(Term(@"\x0F"));
        public static readonly Parser Attribute = Any(ToggleBold, ToggleItalic, ToggleUnderline, SetColor, FullReset);

        public static readonly Parser Text = Term(@"[^\x02\x03\x0F\x1D\x1F\r\n]+");
        public static readonly Parser Attributes = Set(Attribute);
        public static readonly Parser Run = Sequence(Attributes, Text);
        public static readonly Parser AttributedText = Sequence(Set(Run), Attributes);

        public static readonly Parser Timestamp = Term(@"\[(\d\d:\d\d)\]");
        public static readonly Parser Username = Sequence(SEQ_USER, Skip(Term(@"\s<")), SetColor, Optional(Skip(Term(@"@"))), Term(@"\w+"), FullReset, Skip(Term(@">\s")));
        public static readonly Parser Header = Sequence(Optional(SetColor), Timestamp, Username);

        public static readonly Parser MessageLine = Sequence(SEQ_LINE, Header, AttributedText);

        public static readonly Parser Newline = Term(@"\r?\n");
        public static readonly Parser Log = RequiredSet(Sequence(MessageLine, Skip(Newline)));
    }
}
