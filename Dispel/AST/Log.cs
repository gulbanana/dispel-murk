using System.Collections.Generic;

namespace Dispel.AST
{
    public class Log
    {
        public readonly string Filename;
        public readonly IReadOnlyList<Session> Sessions;

        public Log(string filename, IReadOnlyList<Session> sessions)
        {
            Filename = filename;
            Sessions = sessions;
        }
    }
}
