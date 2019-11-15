using Dispel.AST;
using System;
using System.Linq;

namespace Dispel
{
    /// <summary>output plaintext log files</summary>
    static class TextGenerator
    {
        public static OutputFile[] Format(Log log)
        {
            return log.Sessions.Select(Format).ToArray();
        }

        public static OutputFile Format(Session session, int index)
        {
            return new OutputFile(
                $"{session.Ident}-{index}.txt",
                string.Join("", session.Body.Select(Format))
            );
        }

        public static string Format(Line header)
        {
            return $"[{header.Timestamp}] <{header.Username}> {Format(header.Message)}";
        }

        public static string Format(Message body)
        {
            return $"{body.Flatten()}{Environment.NewLine}";
        }
    }
}
