using System.Collections.Generic;
using Nestor.Models;

namespace Nestor.Data
{
    public class Storage
    {
        protected readonly List<string> Prefixes = new List<string>();
        protected readonly List<string> Suffixes = new List<string>();
        protected readonly List<string> Tags = new List<string>();
        protected readonly List<string> TagGroups = new List<string>();
        protected readonly List<Word> Words = new List<Word>();

        public string GetPrefix(int id)
        {
            return Prefixes.GetOrEmpty(id);
        }

        public string GetSuffix(int id)
        {
            return Suffixes.GetOrEmpty(id);
        }

        public string GetTag(int id)
        {
            return Tags.GetOrEmpty(id);
        }

        public string GetTagGroup(int id)
        {
            return TagGroups.GetOrEmpty(id);
        }

        public Word GetWord(int id)
        {
            return Words[id];
        }

        public List<string> GetPrefixes()
        {
            return Prefixes;
        }

        public List<string> GetSuffixes()
        {
            return Suffixes;
        }

        public List<string> GetTags()
        {
            return Tags;
        }

        public List<string> GetTagGroups()
        {
            return TagGroups;
        }

        public List<Word> GetWords()
        {
            return Words;
        }
    }
}