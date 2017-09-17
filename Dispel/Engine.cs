using System;
using System.IO;

namespace Dispel
{
    public static class Engine
    {
        public static void Convert(Stream input, Stream output, OutputFormat format)
        {
            var parser = LogParser.Line;
            var generator = format == OutputFormat.Text ? (Func<Node, string>)TextGenerator.Format : PageGenerator.Format;

            using (var reader = new StreamReader(input))
            using (var writer = new StreamWriter(output))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var result = parser(line);
                    if (!result.IsSuccess)
                    {
                        writer.WriteLine($"parse error! expected: {result.Expected}; found: '{result.Remainder}'");
                    }
                    else
                    {
                        writer.WriteLine(generator(result.Tree));
                    }
                }

                writer.Flush();
            }
        }
    }
}
