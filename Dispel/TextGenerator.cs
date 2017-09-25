using Dispel.AST;
using System;
using System.Linq;

namespace Dispel
{
    /// <summary>output a plaintext log file</summary>
    static class TextGenerator
    {
        public static string Format(Log log)
        {
            return string.Join("", log.Sessions.SelectMany(s => s.Body).Select(Format));
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
