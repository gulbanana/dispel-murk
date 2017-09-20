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
            return string.Join("", log.Messages.Select(Format));
        }

        public static string Format(Message message)
        {
            return $"{Format(message.Header)} {message.Body.Text}{Environment.NewLine}";
        }

        public static string Format(MessageHeader header)
        {
            return $"[{header.Timestamp}] <{header.Username}>";
        }
    }
}
