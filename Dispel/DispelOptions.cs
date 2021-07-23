using System.Collections.Generic;

namespace Dispel
{
    public class DispelOptions
    {
        public string Title { get; set; }
        public string GM { get;set; }
        public Dictionary<string, string> Aliases { get; set; }
        public Dictionary<string, int[]> Groups { get; set; }
        public int[] Vignettes { get; set; }
    }
}