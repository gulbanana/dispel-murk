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

                default:
                    Flag = AttributeType.Unknown;
                    break;
            }
        }

        public Attribute(string flag, string options) : this(flag)
        {
            Options = options;
        }
    }
}
