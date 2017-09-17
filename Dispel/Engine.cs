using System.IO;
using System.Threading.Tasks;

namespace Dispel
{
    public static class Engine
    {
        public static async Task ConvertAsync(Stream input, Stream output, OutputFormat format)
        {
            var parser = LogParser.Log;
            var generator = Formats.GetGenerator(format);

            using (var reader = new StreamReader(input))
            using (var writer = new StreamWriter(output))
            {
                var inputText = await reader.ReadToEndAsync();

                var result = parser(inputText);
                if (!result.IsSuccess)
                {
                    writer.WriteLine($"parse error! expected: {result.Expected}; found: '{result.Remainder}'");
                }
                else
                {
                    var outputText = generator(result.Tree);
                    await writer.WriteLineAsync(outputText);
                }

                await writer.FlushAsync();
            }
        }
    }
}
