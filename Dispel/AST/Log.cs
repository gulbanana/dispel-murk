using System.Collections.Generic;

namespace Dispel.AST
{
    public class Log
    {
        public readonly IReadOnlyList<Session> Sessions;

        public Log(IReadOnlyList<Session> sessions)
        {
            Sessions = sessions;
        }
    }
}
