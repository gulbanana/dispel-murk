using System.IO;

namespace Dispel
{
    public static class Engine
    {
        public static void Convert(Stream input, Stream output)
        {
            using (var reader = new StreamReader(input))
            using (var writer = new StreamWriter(output))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    writer.WriteLine(ConvertLine(line));
                }

                writer.Flush();
            }
        }

        static string ConvertLine(string input)
        {
            var parser = LogParser.Line;

            var r = parser(input);
            if (!r.IsSuccess) return $"parse error! expected: {r.Expected}; found: '{r.Remainder}'";

            return LogGenerator.Format(r.Tree);
        }
    }
}
