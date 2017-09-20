using System.Collections.Generic;
using System.Linq;

namespace Dispel.AST
{
    public class MessageBody
    {
        public string Text;

        public MessageBody(IEnumerable<Run> runs)
        {
            Text = string.Join("", runs.Select(r => r.Text));
        }
    }

    public class Attributes
    {
        public Attributes(IEnumerable<Parse.Node> _) { }
    }

    public class Run
    {
        public string Text;

        public Run(Attributes _, string text)
        {
            Text = text;
        }
    }
}
