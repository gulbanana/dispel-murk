using Dispel.AST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dispel
{
    /// <summary>output HTML</summary>
    class WebGenerator
    {
        private const string stylesheet = @"<link href=""style.css"" rel=""stylesheet"">
";
        private readonly DispelOptions options;
        private readonly string prefix;

        public WebGenerator(DispelOptions options)
        {
            this.options = options;
            this.prefix = @$"<!DOCTYPE html>
<meta charset=""utf-8"">
<title>{options.Title ?? "mIRC Logfile"}</title>
";
        }

        public OutputFile[] FormatSite(Log log)
        {
            return log.Sessions
                .Select((s, i) => (s, i)).Where(t => Filter(t.s))
                .Select(t => Format(t.s, t.i)).Append(new OutputFile(
                    "style.css",
                    ReadStylesheet()
                )).Append(new OutputFile(
                    "index.html",
                    CreateIndex(log)
                )).ToArray();
        }

        public OutputFile[] FormatPage(Log log)
        {
            var logName = Path.GetFileNameWithoutExtension(log.Filename);

            var bodyContent = "<div class='log-grid'>" + Environment.NewLine
                            + string.Join(Environment.NewLine, log.Sessions.Where(Filter).SelectMany(s => s.Body).Select(Format)) + Environment.NewLine
                            + "</div>" + Environment.NewLine;

            var stylesheet = @"<style type=""text/css"">
" + ReadStylesheet() + @"
</style>
";

            return new[] {
                new OutputFile(
                    $"{logName}.html",
                    prefix + stylesheet + bodyContent
                )
            };
        }

        private bool Filter(Session session)
        {
            if (options.Filters == null)
            {
                return true;
            }
            
            if (options.Filters.IncludeOneOf != null && options.Filters.IncludeOneOf.Any())
            {
                if (session.Participants.All(u => !options.Filters.IncludeOneOf.Contains(u, StringComparer.OrdinalIgnoreCase)))
                {
                    if (options.Verbose)
                    {
                        Console.WriteLine($"IncludeOneOf: Skipping {session.Ident}");
                    }
                    return false;
                }
            }

            if (options.Filters.ExcludeAllOf != null && options.Filters.ExcludeAllOf.Any())
            {
                if (session.Participants.Any(u => options.Filters.ExcludeAllOf.Contains(u, StringComparer.OrdinalIgnoreCase)))
                {
                    if (options.Verbose)
                    {
                        Console.WriteLine($"ExcludeAllOf: Skipping {session.Ident} due to {session.Participants.Where(u => options.Filters.ExcludeAllOf.Contains(u, StringComparer.OrdinalIgnoreCase)).First()}");
                    }
                    return false;
                }
            }

            return true;
        }

        private OutputFile Format(Session session, int index)
        {
            var bodyContent = "<div class='log-grid'>" + Environment.NewLine
                            + string.Join(Environment.NewLine, session.Body.Select(Format)) + Environment.NewLine
                            + "</div>" + Environment.NewLine;

            return new OutputFile(
                $"{session.Ident}-{index}.html",
                prefix + stylesheet + bodyContent
            );
        }

        private static string Format(Line header)
        {
            if (header.Media == null)
            {
                return $"<span class='timestamp'>{header.Timestamp}</span> <span class='user'>&lt;{header.Username}&gt;</span> <span>{Format(header.Message)}</span>";
            }
            else
            {
                return $"<span class='timestamp'></span> <span class='user'></span> <img src=\"{header.Media}\">";
            }
        }

        private static string Format(Message body)
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

        private string CreateIndex(Log log)
        {
            var mainTitle = options.Title ?? Path.GetFileNameWithoutExtension(log.Filename);
            string title(int ix)
            {
                foreach (var kvp in options.Groups ?? Enumerable.Empty<KeyValuePair<string, int[]>>())
                {
                    if (ix >= kvp.Value[0] && (kvp.Value.Length == 1 || ix <= kvp.Value[1]))
                    {
                        return kvp.Key;
                    }
                }

                return mainTitle;
            }

            var groups = log.Sessions.Select((s, ix) => (s, ix)).GroupBy(t => title(t.ix));          

            return $@"{prefix}{stylesheet}{string.Join(Environment.NewLine, groups.Select(g => CreateIndex(g.Key, g)))}";
        }

        private string CreateIndex(string title, IEnumerable<(Session, int)> sessions)
        {
            var links = sessions
                .Select(t => {
                    var (s, ix) = t;
                    var name = $"{s.Ident}-{ix}.html";
                    var url = Uri.EscapeDataString(name);
                    var date = s.StartTime;
                    var participants = s.Body
                        .Select(l => l.Username)
                        .Where(u => u != null)
                        .Distinct()
                        .OrderBy(u => !u.Equals(options.GM ?? "banana", StringComparison.OrdinalIgnoreCase))
                        .ThenBy(u => u);
                    
                    var notes = options.Notes != null && options.Notes.ContainsKey(ix.ToString()) ? options.Notes[ix.ToString()] : string.Empty;

                    return $@"<a href=""{url}"">{name}</a>
    <span>{date}</span>
    <span>{string.Join(", ", participants)}</span>
    <span class=""notes"">{notes}</span>";
            });

            return $@"<h2>{title}</h2>
<div class=""session-grid"">
    <span class=""th"">Session</span>
    <span class=""th"">Date</span>
    <span class=""th"">Participants</span>
    <span class=""th"">Notes</span>
    {string.Join("\n\t", links)}
</div>
";
        }
    }
}
