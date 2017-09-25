namespace Dispel.AST
{
    public class Session
    {
        public readonly string Ident;
        public readonly SessionBody Body;

        public Session(string startTime, string ident, SessionBody body, string closeTime)
        {
            Ident = ident;
            Body = body;
        }
    }
}
