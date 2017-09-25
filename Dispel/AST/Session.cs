using System.Collections.Generic;
using System.Linq;

namespace Dispel.AST
{
    public class Session
    {
        public readonly string Ident;
        public readonly Line[] Body;

        public Session(string ident, string startTime, string endTime, IEnumerable<Line> body)
        {
            Ident = ident;
            Body = body.ToArray();
        }
    }
}
