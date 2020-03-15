using System.Collections.Generic;

namespace Nestor.Models
{
    public class WordForm
    {
        public string Word { get; set; }
        public int Accent { get; set; }
        public HashSet<string> Tags { get; set; }
        
        
    }
}