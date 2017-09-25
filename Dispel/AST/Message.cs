using System.Linq;

namespace Dispel.AST
{
    public class Message
    {
        public readonly Run[] Runs;

        public Message(Run[] runs)
        {
            Runs = runs;
        }

        public string Flatten() => string.Join("", Runs.Select(r => r.Text));
    }
}
