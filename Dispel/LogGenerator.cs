using Dispel.AST;
using System;
using System.IO;
using System.Linq;

namespace Dispel
{
    /// <summary>output log files from log files, but with metadata stripped!</summary>
    static class LogGenerator
    {
        public static OutputFile[] Format(Log log)
        {
            var lines = log.Sessions.SelectMany(s => s.Body);
            var text = string.Join("", lines.Select(Format));
            return new OutputFile[]
            {
                new($"{Path.GetFileNameWithoutExtension(log.Filename)}-stripped.log", text)
            };
        }

        public static string Format(Line line)
        {
            return line.Original + Environment.NewLine;
        }
    }
}
