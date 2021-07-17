using Dispel.AST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dispel
{
    public class Engine
    {
        private readonly List<Session> sessions;
        private readonly List<Line> sessionLines;
        private readonly DispelOptions options;
        private string sessionIdent;
        private string sessionStart;
        private string sessionEnd;
        private bool inSession;

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
            if (sessionLines.Count > 0)
            {
                sessions.Add(new Session(sessionIdent, sessionStart, sessionEnd, sessionLines));
                sessionLines.Clear();
            }

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
                    else if (line.StartsWith("Session "))
                    {
                        var sessionCommand = line.Substring(8, 5);
                        switch (sessionCommand)
                        {
                            case "Ident":
                                sessionIdent = line.Substring(15);
                                break;

                            case "Start":
                                if (inSession) FlushSession();
                                inSession = true;
                                sessionStart = line.Substring(15);
                                break;

                            case "Close":
                                sessionEnd = line.Substring(15);
                                FlushSession();
                                inSession = false;
                                break;
                        }
                    }
                    else
                    {
                        ProcessMessage(line);
                    }
                }

                FlushSession();

                return sessions;
            }
        }

        private void ProcessMessage(string line)
        {
            var result = LineParser.Line(line);
            if (!result.IsSuccess) throw new Exception("parse error: expected " + result.Expected + Environment.NewLine + result.Remainder);
            if (!string.IsNullOrEmpty(result.Remainder)) throw new Exception("parse error: junk after log" + result.Expected + Environment.NewLine + result.Remainder);

            var username = result.Tree.Children[1].Text;
            if (username != "*" && username != "->") // control node, private message
            {
                var runNodes = result.Tree.Children[2].Children;

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
                    result.Tree.Children[0].Text,
                    GetAlias(username),
                    new Message(runs)
                );

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
