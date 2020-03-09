using System.Collections.Generic;
using Nestor.Models;

namespace Nestor.Data
{
    public class Storage
    {
        protected readonly List<string> Prefixes = new List<string>();
        protected readonly List<string> Suffixes = new List<string>();
        protected readonly List<string> Tags = new List<string>();
        protected readonly List<byte[]> TagGroups = new List<byte[]>();
        protected readonly List<WordRaw> Words = new List<WordRaw>();

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

        public byte[] GetTagGroup(int id)
        {
            return id == 0 ? new byte[0] : TagGroups[id - 1];
        }

        public WordRaw GetWord(int id)
        {
            return id == 0 ? default : Words[id - 1];
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

        public List<byte[]> GetTagGroups()
        {
            return TagGroups;
        }

        public List<WordRaw> GetWords()
        {
            return Words;
        }
    }
}