using System.Collections.Generic;
using System.Linq;

namespace Dispel.AST
{
    public class Message
    {
        public readonly Run[] Runs;

        public Message(IEnumerable<Run> runs)
        {
            Runs = runs.ToArray();
        }

        public string Flatten() => string.Join("", Runs.Select(r => r.Text));
    }
}
