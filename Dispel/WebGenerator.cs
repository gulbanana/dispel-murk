using Dispel.AST;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Dispel
{
    /// <summary>output HTML</summary>
    static class WebGenerator
    {
        private const string prefix = @"<!DOCTYPE html>
<meta charset=""utf-8"">
<title>mIRC Logfile</title>
";

        private const string stylesheet = @"<link href=""style.css"" rel=""stylesheet"">
";

        public static OutputFile[] FormatSite(Log log)
        {
            return log.Sessions.Select(Format).Append(new OutputFile(
                "style.css",
                ReadStylesheet()
            )).Append(new OutputFile(
                "index.html",
                CreateIndex(log)
            )).ToArray();
        }

        public static OutputFile Format(Session session, int index)
        {
            var bodyContent = string.Join("", session.Body.Select(Format));

            return new OutputFile(
                $"{session.Ident}-{index}.html",
                prefix + stylesheet + bodyContent
            );
        }

        public static OutputFile[] FormatPage(Log log)
        {
            var firstIdent = log.Sessions.First().Ident;
            var bodyContent = string.Join("", log.Sessions.SelectMany(s => s.Body).Select(Format));
            var stylesheet = @"<style type=""text/css"">
" + ReadStylesheet() + @"
</style>
";

            return new[] {
                new OutputFile(
                    $"{firstIdent}.html",
                    prefix + stylesheet + bodyContent
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

        private static string ReadStylesheet()
        {
            return new StreamReader(typeof(WebGenerator).Assembly.GetManifestResourceStream("Dispel.style.css")).ReadToEnd();
        }

        private static string CreateIndex(Log log)
        {
            var title = log.Sessions.First().Ident;
            var links = log.Sessions
                .Select((s, ix) => {
                    var name = $"{s.Ident}-{ix}.html";
                    var url = Uri.EscapeDataString(name);
                    var date = s.StartTime;
                    var participants = s.Body
                        .Select(l => l.Username)
                        .Distinct()
                        .OrderBy(u => !u.Equals("Crion", StringComparison.OrdinalIgnoreCase))
                        .ThenBy(u => u);

                    return $@"<a href=""{url}"">{name}</a>
    <span>{date}</span>
    <span>{string.Join(", ", participants)}</span>";});

            return $@"{prefix}{stylesheet}<h2>{title}</h2>
<div class=""session-grid"">
    <span class=""th"">Session</span>
    <span class=""th"">Date</span>
    <span class=""th"">Participants</span>
    {string.Join("\n\t", links)}
</div>";
        }
    }
}
