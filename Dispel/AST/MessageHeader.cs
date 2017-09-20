namespace Dispel.AST
{
    public class MessageHeader
    {
        public string Timestamp;
        public string Username;

        public MessageHeader(string timestamp, string username)
        {
            Timestamp = timestamp;
            Username = username;
        }
    }
}
