namespace Dispel.AST
{
    public class MessageHeader
    {
        public readonly string Timestamp;
        public readonly string Username;

        public MessageHeader(string timestamp, string username)
        {
            Timestamp = timestamp;
            Username = username;
        }
    }
}
