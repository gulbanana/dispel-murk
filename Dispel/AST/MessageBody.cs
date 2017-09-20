using System.Collections.Generic;
using System.Linq;

namespace Dispel.AST
{
    public class MessageBody
    {
        public readonly string Text;

        public MessageBody(IEnumerable<Run> runs)
        {
            Text = string.Join("", runs.Select(r => r.Text));
        }
    }
}
