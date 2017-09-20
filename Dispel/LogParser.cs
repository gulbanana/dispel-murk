﻿using static Dispel.Parse.Combinators;
using Dispel.Parse;

namespace Dispel
{
    public static class LogParser
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
        public static readonly Parser Username = Sequence(Skip(@"\s<"), Skip(SetColor), Optional(Skip(@"@")), Term(@"\w+"), Skip(FullReset), Skip(@">\s"));
        public static readonly Parser Header = Sequence(Skip(Optional(SetColor)), Timestamp, Username);

        public static readonly Parser Message = Sequence(Header, AttributedText);
        public static readonly Parser Newline = Term(@"\r?\n");
        public static readonly Parser Log = RequiredSet(Sequence(Message, Skip(Newline)));
    }
}
