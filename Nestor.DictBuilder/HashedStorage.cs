using System;
using System.Collections.Generic;
using System.Linq;
using Nestor.Data;
using Nestor.Models;

namespace Nestor.DictBuilder
{
    public class HashedStorage : Storage
    {
        private readonly Dictionary<string, int> _prefixesDict = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _suffixesDict = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _tagsDict = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _tagGroupsDict = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _wordsDict = new Dictionary<string, int>();

        public int AddPrefix(string prefix)
        {
            return Utils.ComplexAdd(_prefixesDict, Prefixes, prefix, x => x) + 1;
        }

        public int AddSuffix(string suffix)
        {
            return Utils.ComplexAdd(_suffixesDict, Suffixes, suffix, x => x) + 1;
        }
        
        public int AddTag(string tag)
        {
            return Utils.ComplexAdd(_tagsDict, Tags, tag, x => x) + 1;
        }

        public int[] AddTags(string[] tags)
        {
            return tags.Select(AddTag).OrderBy(t => t).ToArray();
        }

        public int AddTagGroup(string[] tags)
        {
            var tagIds = AddTags(tags);
            var tagGroup = string.Join("|", tagIds);
            return Utils.ComplexAdd(_tagGroupsDict, TagGroups, tagGroup, x => x) + 1;
        }

        public int AddWord(WordRaw w)
        {
            return Utils.ComplexAdd(_wordsDict, Words, w, x => x.ToString()) + 1;
        }
    }
}