using Dispel.AST;
using System;
using System.Linq;

namespace Dispel
{
    /// <summary>output special HTML for the Obsidian Portal wiki</summary>
    static class WikiGenerator
    {
        public static string Format(Log log)
        {
            return string.Join("", log.Messages.Select(Format));
        }

        public static string Format(Message message)
        {
            return $"{Format(message.Header)} {message.Body.Flatten()}<br>{Environment.NewLine}";
        }

        public static string Format(MessageHeader header)
        {
            var padding = "".PadRight(9 - header.Username.Length).Replace(" ", "&nbsp;");
            return $"<span style='font-family:monospace;'><span style='color:#a9a9a9;'>{header.Timestamp}</span>&nbsp;<b>&lt;{header.Username}&gt;</b>{padding}</span>";
        }
    }
}
