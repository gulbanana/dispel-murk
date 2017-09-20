using System.Collections.Generic;
using System.Linq;

namespace Dispel.AST
{
    public class Log
    {
        public readonly Message[] Messages;

        public Log(IEnumerable<Message> messages)
        {
            Messages = messages.ToArray();
        }
    }
}
