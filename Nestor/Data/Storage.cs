using System.Collections.Generic;

namespace Nestor.Data
{
    public class Storage : IStorage
    {
        private readonly List<string> _prefixes = new List<string>();
        private readonly List<string> _suffixes = new List<string>();
        private readonly List<string> _tags = new List<string>();
        private readonly List<string> _tagGroups = new List<string>();

        public string GetPrefix(int id)
        {
            return id == 0 ? "" : _prefixes[id - 1];
        }

        public string GetSuffix(int id)
        {
            return id == 0 ? "" : _suffixes[id - 1];
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

        public List<string> GetTagGroups()
        {
            return _tagGroups;
        }
    }
}