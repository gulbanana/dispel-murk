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
                    writer.WriteLine(line);
                }

                writer.Flush();
            }
        }
    }
}
