namespace Dispel.AST
{
    public class Message
    {
        public MessageHeader Header;
        public MessageBody Body;

        public Message(MessageHeader header, MessageBody body)
        {
            Header = header;
            Body = body;
        }
    }
}
