using System.Collections.Generic;
using System.Linq;

namespace Dispel.AST
{
    public class Log
    {
        public readonly Session[] Sessions;

        public Log(IEnumerable<Session> sessions)
        {
            Sessions = sessions.ToArray();
        }
    }
}
