namespace Dispel.AST
{
    public class Message
    {
        public readonly MessageHeader Header;
        public readonly MessageBody Body;

        public Message(MessageHeader header, MessageBody body)
        {
            Header = header;
            Body = body;
        }
    }
}
