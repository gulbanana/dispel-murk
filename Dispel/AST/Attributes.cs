namespace Dispel.AST
{
    public class Attribute
    {
        public readonly string Flag;
        public readonly string Options;

        public Attribute(string flag)
        {
            Flag = flag;
        }

        public Attribute(string flag, string options)
        {
            Flag = flag;
            Options = options;
        }
    }
}
