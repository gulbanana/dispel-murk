using System.Collections.Generic;
using System.Linq;

namespace Dispel.AST
{
    public class Run
    {
        public readonly Attribute[] Attributes;
        public readonly string Text;

        public Run(IEnumerable<Attribute> attributes, string text)
        {
            Attributes = attributes.ToArray();
            Text = text;
        }
    }
}
