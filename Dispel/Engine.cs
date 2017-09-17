using System.IO;

namespace Dispel
{
    public static class Engine
    {
        public static void Convert(Stream input, Stream output)
        {
            input.CopyTo(output);
        }
    }
}
