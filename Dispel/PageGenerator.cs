using Dispel.AST;
using System;
using System.Linq;
using System.Text;

namespace Dispel
{
    /// <summary>output HTML</summary>
    static class PageGenerator
    {
        private const string template = @"<!DOCTYPE html>
<meta charset=""utf-8"">
<title>mIRC Logfile</title>
<style>
body {
    font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Oxygen-Sans, Ubuntu, Cantarell, ""Helvetica Neue"", sans-serif;
    color: rgb(15, 15, 15);
}
p.line {
    margin: 0;
    padding: 0;
}
span.user {
    font-weight: bold;
}
span.timestamp {
    color: rgb(128, 128, 128);
}
</style>
";

        public static string Format(Log log)
        {
            var content = string.Join("", log.Messages.Select(Format));
            return template + content;
        }

        public static string Format(Message message)
        {
            return $"<p class='line'>{Format(message.Header)} {Format(message.Body)}</p>{Environment.NewLine}";
        }

        public static string Format(MessageHeader header)
        {
            return $"<span class='timestamp'>{header.Timestamp}</span> <span class='user'>&lt;{header.Username}&gt;</span>";
        }

        public static string Format(MessageBody body)
        {
            var isBold = false;

            var builder = new StringBuilder();
            foreach (var run in body.Runs)
            {
                foreach (var attribute in run.Attributes)
                {
                    switch (attribute.Flag)
                    {
                        case AttributeType.Bold:
                            isBold = !isBold;
                            builder.Append(isBold ? "<b>" : "</b>");
                            break;

                        default:
                            break;
                    }

                    builder.Append(run.Text);
                }
            }
            return builder.ToString();
        }
    }
}
