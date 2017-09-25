using System;
using System.IO;
using System.Linq;
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
                    writer.WriteLine($"parse error! expected: {Environment.NewLine}{result.Expected}{Environment.NewLine}found:{Environment.NewLine}{result.Remainder}");
                }
                else if (!string.IsNullOrEmpty(result.Remainder))
                {
                    writer.WriteLine($"parse error! found input, but also extra data:{Environment.NewLine}{result.Remainder}");
                }
                else if (result.Tree.Type == Parse.NodeType.Empty)
                {
                    writer.WriteLine($"parse succeeded, but no AST produced");
                }
                else
                {
                    var ast = result.Tree.Build<AST.Log>();
                    var outputText = generator(new AST.SessionBody(ast.Sessions.SelectMany(s => s.Body.Messages)));
                    await writer.WriteLineAsync(outputText);
                }

                await writer.FlushAsync();
            }
        }
    }
}
