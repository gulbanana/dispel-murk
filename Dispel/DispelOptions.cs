using System;
using System.Collections.Generic;

namespace Dispel
{
    public class DispelOptions
    {
        public string Title { get; set; }
        public string GM { get;set; }
        public Dictionary<string, string> Aliases { get; set; }
        public Dictionary<string, int[]> Groups { get; set; }
        public Dictionary<string, string> Notes { get; set; }
        public int BlankLinesThreshhold { get; set; } = 3;
        public TimeSpan MaxLengthThreshhold { get; set; } = TimeSpan.FromHours(24);
        public string[] Ignored { get; set; } = new string[] { "*", "->" };  // control node, private message
        public bool Verbose { get; set; }
        public FilterOptions Filters { get; set; }
        public bool WordCount { get; set; }
        public bool NoIndex { get; set; }
    }
}