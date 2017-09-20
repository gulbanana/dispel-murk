namespace Dispel.AST
{
    public class Message
    {
        public MessageHeader Header;
        public string Body;

        public Message(MessageHeader header, string body)
        {
            Header = header;
            Body = body;
        }
    }
}
