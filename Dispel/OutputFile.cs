namespace Dispel
{
    public class OutputFile
    {
        public readonly string Filename;
        public readonly string Content;

        public OutputFile(string filename, string content)
        {
            Filename = filename;
            Content = content;
        }
    }
}
