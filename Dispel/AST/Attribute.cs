namespace Dispel.AST
{
    public class Attribute
    {
        public readonly AttributeType Flag;
        public readonly string Options;

        public Attribute(string flag)
        {
            switch (flag)
            {
                case "\x02":
                    Flag = AttributeType.Bold;
                    break;

                case "\x1D":
                    Flag = AttributeType.Italic;
                    break;

                case "\x1F":
                    Flag = AttributeType.Underline;
                    break;

                case "\x03":
                    Flag = AttributeType.Color;
                    break;

                case "\x0F":
                    Flag = AttributeType.Reset;
                    break;
            }
        }

        public Attribute(string flag, string options) : this(flag)
        {
            Options = options;
        }
    }
}
