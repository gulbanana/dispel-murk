﻿using Dispel.AST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dispel
{
    public class Engine
    {
        private readonly List<Session> sessions;
        private readonly List<Line> sessionLines;
        private readonly DispelOptions options;
        private string previousSessionIdent;
        private string sessionIdent;
        private string sessionStart;
        private string sessionEnd;
        private string sessionTime;
        private bool inSession;
        private int blankLines;

        public Engine(DispelOptions options)
        {
            sessions = new List<Session>();
            sessionLines = new List<Line>();
            inSession = true;
            this.options = options;
        }

        // XXX only works for single-file formats
        public async Task ConvertSingleAsync(Stream input, Stream output, OutputFormat format)
        {
            var generator = Formats.GetGenerator(format, options);
            var sessions = await GetSessions(input);
            var log = new Log(sessions);
            var outputFiles = generator(log);
            var outputText = outputFiles.Single().Content;

            using (var writer = new StreamWriter(output))
            {
                await writer.WriteLineAsync(outputText);
                await writer.FlushAsync();
            }
        }

        public async Task<OutputFile[]> ConvertAsync(Stream input, OutputFormat format)
        {
            var generator = Formats.GetGenerator(format, options);
            var sessions = await GetSessions(input);
            var log = new Log(sessions);
            return generator(log);
        }

        private void FlushSession()
        {
            blankLines = 0;
            if (sessionLines.Count > 0)
            {
                sessions.Add(new Session(sessionIdent ?? previousSessionIdent, sessionStart ?? sessionTime, sessionEnd, sessionLines));
                sessionLines.Clear();
            }

            previousSessionIdent = sessionIdent;
            sessionIdent = null;
            sessionStart = null;
            sessionEnd = null;
        }
        
        private async Task<List<Session>> GetSessions(Stream input)
        { 
            using (var reader = new StreamReader(input))            
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();

                    if (line.Length == 0)
                    {
                        continue;
                    }
                    else if (line.StartsWith("Start of "))
                    {
                        var match = Regex.Match(line, "Start of (.+) buffer: (.+)");
                        if (match.Success)
                        {
                            sessionIdent = match.Groups[1].Value;
                            sessionStart = match.Groups[2].Value;
                            sessionTime = sessionStart;
                        }
                    }
                    else if (line.StartsWith("End of "))
                    {
                        var match = Regex.Match(line, "End of (.+) buffer: (.+)");
                        if (match.Success)
                        {
                            sessionIdent = match.Groups[1].Value;
                            sessionEnd = match.Groups[2].Value;
                            sessionTime = sessionEnd;
                        }
                    }
                    else
                    {
                        var parser = Parse.Combinators.Any(LineParser.Line, DirectiveParser.MircDirective, DirectiveParser.DMDirective);
                        var result = parser(line);

                        if (!result.IsSuccess) throw new Exception("parse error: expected " + result.Expected + Environment.NewLine + result.Remainder);
                        if (!string.IsNullOrEmpty(result.Remainder)) throw new Exception("parse error: junk after log" + result.Expected + Environment.NewLine + result.Remainder);

                        switch(result.Tag)
                        {
                            case 1:
                                ProcessMessage(result.Tree);
                                break;

                            case 2:
                                ProcessSessionDirective(result.Tree);
                                break;

                            case 3:
                                ProcessEngineDirective(result.Tree);
                                break;
                        }
                    }
                }

                FlushSession();

                return sessions;
            }
        }

        private void ProcessEngineDirective(Parse.Node tree)
        {
            var command = tree.Children[0].Text;
            var args = tree.Children[1].Text;
            switch (command)
            {
                case "img":
                    sessionLines.Add(new Line(args));
                    break;
            }
        }

        private void ProcessSessionDirective(Parse.Node tree)
        {
            var command = tree.Children[0].Text;
            var args = tree.Children[1].Text;
            switch (command)
            {
                case "Ident":
                    sessionIdent = args;
                    break;

                case "Start":
                    if (inSession) FlushSession();
                    inSession = true;
                    sessionStart = args;
                    sessionTime = sessionStart;
                    break;

                case "Close":
                    sessionEnd = args;
                    FlushSession();
                    inSession = false;
                    break;

                case "Time":
                    sessionTime = args;
                    break;
            }

        }

        private void ProcessMessage(Parse.Node tree)
        {
            var username = tree.Children[1].Text;
            if (!options.Ignored.Contains(username))
            {
                var runNodes = tree.Children[2].Children;

                var runs = new Run[runNodes.Length];
                for (var i = 0; i < runs.Length; i++)
                {
                    var attributeNodes = runNodes[i].Children[0].Children;
                    var textNode = runNodes[i].Children[1];

                    var attributes = new AST.Attribute[attributeNodes.Length];
                    for (var j = 0; j < attributes.Length; j++)
                    {
                        var t2 = attributeNodes[j];
                        if (t2.Children.Length == 2)
                        {
                            attributes[j] = new AST.Attribute(t2.Children[0].Text, t2.Children[1].Text);
                        }
                        else
                        {
                            attributes[j] = new AST.Attribute(t2.Text);
                        }
                    }

                    runs[i] = new Run(attributes, textNode.Text);
                }

                var ast = new Line(
                    tree.Children[0].Text,
                    GetAlias(username),
                    new Message(runs)
                );

                if (runs.All(r => string.IsNullOrWhiteSpace(r.Text)))
                {
                    blankLines++;
                }
                else
                {
                    if (blankLines >= options.BlankLinesThreshhold)
                    {
                        sessionLines.RemoveRange(sessionLines.Count - blankLines, blankLines);

                        if (sessionLines.Any())
                        {
                            FlushSession();
                            sessionIdent = previousSessionIdent;
                            sessionStart = sessionTime;
                        }
                    }
                    blankLines = 0;
                }
                sessionLines.Add(ast);             
            }
        }

        private string GetAlias(string username)
        {
            if (options.Aliases != null && options.Aliases.ContainsKey(username))
            {
                return options.Aliases[username];
            }
            else
            {
                return username;
            }
        }
    }
}
