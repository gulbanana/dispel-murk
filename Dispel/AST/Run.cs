namespace Dispel.AST
{
    public class Run
    {
        public readonly Attribute[] Attributes;
        public readonly string Text;

        public Run(Attribute[] attributes, string text)
        {
            Attributes = attributes;
            Text = text;
        }
    }
}
