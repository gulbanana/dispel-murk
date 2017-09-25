using System.Collections.Generic;
using System.Linq;

namespace Dispel.AST
{
    public class SessionBody
    {
        public readonly Message[] Messages;

        public SessionBody(IEnumerable<Message> messages)
        {
            Messages = messages.ToArray();
        }
    }
}
