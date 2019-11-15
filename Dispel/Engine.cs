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
        public static async Task ConvertAsync(Stream input, Stream output, OutputFormat format)
        {
            var generator = Formats.GetGenerator(format);
            var engine = new Engine();
            var sessions = await engine.GetSessions(input);
            var log = new Log(sessions);
            var outputText = generator(log);

            using (var writer = new StreamWriter(output))
            {
                await writer.WriteLineAsync(outputText);
                await writer.FlushAsync();
            }
        }

        public static async Task<List<string>> ConvertMultisessionAsync(Stream input, OutputFormat format)
        {
            var generator = Formats.GetGenerator(format);
            var engine = new Engine();
            var sessions = await engine.GetSessions(input);
            return sessions.Select(s => new Log(new[]{s})).Select(generator).ToList();
        }

        private readonly List<Session> sessions;
        private readonly List<Line> sessionLines;
        private string sessionIdent;
        private string sessionStart;
        private string sessionEnd;
        private bool inSession;

        private Engine()
        {
            sessions = new List<Session>();
            sessionLines = new List<Line>();
            inSession = true;
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
            if (username != "*") // control node
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
                    username,
                    new Message(runs)
                );

                sessionLines.Add(ast);
            }
        }
    }
}
