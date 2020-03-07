using System.Collections.Generic;
using System.Linq;
using Nestor.Models;

namespace Nestor
{
    public class Storage
    {
        private readonly List<string> _prefixes = new List<string>();
        private readonly List<string> _suffixes = new List<string>();
        private readonly List<string> _tags = new List<string>();

        public string GetPrefix(int id)
        {
            return id == 0 ? "" : _prefixes[id - 1];
        }

        public string GetSuffix(int id)
        {
            return id == 0 ? "" : _suffixes[id - 1];
        }
        
        public int AddPrefix(string prefix)
        {
            if (prefix == "")
            {
                return 0;
            }

            return _prefixes.AddOrCheck(prefix) + 1;
        }

        public int AddSuffix(string suffix)
        {
            if (suffix == "")
            {
                return 0;
            }

            return _suffixes.AddOrCheck(suffix) + 1;
        }
        
        public int AddTag(string tag)
        {
            return _tags.AddOrCheck(tag) + 1;
        }

        public int[] AddTags(string[] tags)
        {
            return tags.Select(t => AddTag(t)).ToArray();
        }

        public List<string> GetPrefixes()
        {
            return _prefixes;
        }

        public List<string> GetSuffixes()
        {
            return _suffixes;
        }

        public List<string> GetTags()
        {
            return _tags;
        }
    }
}