using static Dispel.Parse;

namespace Dispel
{
    public static class LogParser
    {
        public static readonly Parser Run = Term(@".+");

        public static readonly Parser Timestamp = Term(@"\[(\d\d:\d\d)\]");

        public static readonly Parser Color = Term(@"\x03\d\d");

        public static readonly Parser FullReset = Term(@"\x0F");

        public static readonly Parser Username = Sequence(Term(@"\s<"), Skip(Color), Term(@"\w+"), Skip(FullReset), Term(@">\s"));

        public static readonly Parser Line = Sequence(Optional(Skip(Color)), Timestamp, Username, Run);
    }
}
