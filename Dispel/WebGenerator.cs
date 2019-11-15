using Dispel.AST;
using System;
using System.Linq;
using System.Text;

namespace Dispel
{
    /// <summary>output HTML</summary>
    static class WebGenerator
    {
        private const string prefix = @"<!DOCTYPE html>
<head>
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
    span.b { font-weight: bold; }
    span.u { text-decoration: underline; }
    span.i { font-style: italic; }
    span.c0 { color: rgb(255,255,255); }
    span.c1 { color: rgb(0,0,0); }
    span.c2 { color: rgb(0,0,127); }
    span.c3 { color: rgb(0,147,0); }
    span.c4 { color: rgb(255,0,0); }
    span.c5 { color: rgb(136,72,72); }
    span.c6 { color: rgb(156,0,156); }
    span.c7 { color: rgb(252,127,0); }
    span.c8 { color: rgb(255,255,0); }
    span.c9 { color: rgb(0,252,0); }
    span.c10 { color: rgb(0,147,147); }
    span.c11 { color: rgb(0,255,255); }
    span.c12 { color: rgb(0,0,252); }
    span.c13 { color: rgb(255,0,255); }
    span.c14 { color: rgb(127,127,127); }
    span.c15 { color: rgb(210,210,210); }
    </style>
</head>
<body>";

        private const string suffix = @"</body>
</html>";

        public static OutputFile[] FormatSite(Log log)
        {
            return log.Sessions.Select(Format).ToArray();
        }

        public static OutputFile Format(Session session, int index)
        {
            var bodyContent = string.Join("", session.Body.Select(Format));
            return new OutputFile(
                $"{session.Ident}-{index}.html",
                prefix + bodyContent
            );
        }

        public static OutputFile[] FormatPage(Log log)
        {
            var firstIdent = log.Sessions.First().Ident;
            var bodyContent = string.Join("", log.Sessions.SelectMany(s => s.Body).Select(Format));
            return new[] {
                new OutputFile(
                    $"{firstIdent}.html",
                    prefix + bodyContent
                )
            };
        }

        public static string Format(Line header)
        {
            return $"<p class='line'><span class='timestamp'>{header.Timestamp}</span> <span class='user'>&lt;{header.Username}&gt;</span> {Format(header.Message)}</p>{Environment.NewLine}";
        }

        public static string Format(Message body)
        {
            var isBold = false;
            var isItalic = false;
            var isUnderlined = false;
            var color = default(string);

            var builder = new StringBuilder();
            foreach (var run in body.Runs)
            {
                foreach (var attribute in run.Attributes)
                {
                    switch (attribute.Flag)
                    {
                        case AttributeType.Bold:
                            isBold = !isBold;
                            break;

                        case AttributeType.Italic:
                            isItalic = !isItalic;
                            break;

                        case AttributeType.Underline:
                            isUnderlined = !isUnderlined;
                            break;

                        case AttributeType.Color:
                            if (attribute.Options == null)
                            {
                                color = null;
                            }
                            else
                            {
                                color = int.Parse(attribute.Options.Split(',').First()).ToString();
                            }
                            break;

                        case AttributeType.Reset:
                            isBold = false;
                            isItalic = false;
                            isUnderlined = false;
                            color = null;
                            break;

                        default:
                            break;
                    }
                }

                var textClass = "";

                if (isBold) textClass += "b ";
                if (isItalic) textClass += "i ";
                if (isUnderlined) textClass += "u ";
                if (color != null) textClass += "c" + color;

                if (textClass != "") builder.Append($"<span class='{textClass}'>");
                builder.Append(run.Text.Replace("<", "&lt;").Replace(">", "&gt;"));
                if (textClass != "") builder.Append("</span>");
            }
            return builder.ToString();
        }
    }
}
