using System;
using System.Collections.Generic;
using System.Linq;
using Nestor.Data;

namespace Nestor.DictBuilder
{
    public class HashedStorage : IStorage
    {
        private readonly Dictionary<string, int> _prefixesDict = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _suffixesDict = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _tagsDict = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _tagGroupsDict = new Dictionary<string, int>();
        
        private readonly List<string> _prefixes = new List<string>();
        private readonly List<string> _suffixes = new List<string>();
        private readonly List<string> _tags = new List<string>();
        private readonly List<string> _tagGroups = new List<string>();

        public int AddPrefix(string prefix)
        {
            return ComplexAdd(_prefixesDict, _prefixes, prefix);
        }

        public int AddSuffix(string suffix)
        {
            return ComplexAdd(_suffixesDict, _suffixes, suffix);
        }
        
        public int AddTag(string tag)
        {
            return ComplexAdd(_tagsDict, _tags, tag);
        }

        public int[] AddTags(string[] tags)
        {
            return tags.Select(AddTag).OrderBy(t => t).ToArray();
        }

        public int AddTagGroup(string[] tags)
        {
            var tagIds = AddTags(tags);
            var tagGroup = string.Join("|", tagIds);
            return ComplexAdd(_tagGroupsDict, _tagGroups, tagGroup);
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

        public string GetPrefix(int id)
        {
            return _prefixes[id];
        }

        public string GetSuffix(int id)
        {
            return _suffixes[id];
        }

        private int ComplexAdd<T>(Dictionary<T, int> dict, List<T> list, T value)
        {
            if (value == null || (value is string s && s == ""))
            {
                return 0;
            }
            
            var index = dict.AddOrCheck(value);
            if (index == list.Count)
            {
                list.Add(value);
            } 
            else if (index > list.Count)
            {
                throw new ArgumentException(
                    "Index in dictionary is greater than list size, something went wrong"
                );    
            }

            return index + 1;
        }
    }
}