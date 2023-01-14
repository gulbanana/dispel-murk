namespace Dispel.AST
{
    public class Line
    {
        public readonly string Original;
        public readonly string Timestamp;
        public readonly string Username;
        public readonly Message Message;
        public readonly string Media;

        public Line(string original, string timestamp, string username, Message message)
        {
            Original = original;
            Timestamp = timestamp;
            Username = username;
            Message = message;
        }

        public Line(string media)
        {
            Media = media;
        }
    }
}
