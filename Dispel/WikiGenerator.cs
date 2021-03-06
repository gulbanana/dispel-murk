﻿using Dispel.AST;
using System;
using System.Linq;

namespace Dispel
{
    /// <summary>output special HTML for the Obsidian Portal wiki</summary>
    static class WikiGenerator
    {
        public static OutputFile[] Format(Log log)
        {
            return log.Sessions.Select(Format).ToArray();
        }

        public static OutputFile Format(Session session, int index)
        {
            var maxUsername = session.Body.Select(line => line.Username.Length).Max();

            return new OutputFile(
                $"{session.Ident}-{index}.wiki.html",
                string.Join("", session.Body.Select(l => Format(l, maxUsername)))
            );
        }

        public static string Format(Line line, int maxUsername)
        {
            var padding = "".PadRight(maxUsername + 1 - line.Username.Length).Replace(" ", "&nbsp;");
            return $"<span style='font-family:monospace;'><span style='color:#a9a9a9;'>{line.Timestamp}</span>&nbsp;<b>&lt;{line.Username}&gt;</b>{padding}</span> {Format(line.Message)}";
        }

        public static string Format(Message message)
        {
            return $"{message.Flatten()}<br>{Environment.NewLine}";
        }
    }
}
