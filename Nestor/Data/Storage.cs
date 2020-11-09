using System;
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
        
        protected readonly Dictionary<string, Pos> PartsOfSpeech = new Dictionary<string, Pos>();
        protected readonly Dictionary<string, Gender> Genders = new Dictionary<string, Gender>();
        protected readonly Dictionary<string, Number> Numbers = new Dictionary<string, Number>();
        protected readonly Dictionary<string, Case> Cases = new Dictionary<string, Case>();
        protected readonly Dictionary<string, Tense> Tenses = new Dictionary<string, Tense>();
        protected readonly Dictionary<string, Person> Persons = new Dictionary<string, Person>();
        private readonly TagMapper _tagMapper = new TagMapper();

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

        public void ParseTags()
        {
            foreach (var tag in Tags)
            {
                AddToDictionary(tag, _tagMapper.GetPos(tag), PartsOfSpeech);
                AddToDictionary(tag, _tagMapper.GetGender(tag), Genders);
                AddToDictionary(tag, _tagMapper.GetNumber(tag), Numbers);
                AddToDictionary(tag, _tagMapper.GetCase(tag), Cases);
                AddToDictionary(tag, _tagMapper.GetTense(tag), Tenses);
                AddToDictionary(tag, _tagMapper.GetPerson(tag), Persons);
            }
        }

        public Pos PosByTag(string tag)
        {
            return PartsOfSpeech.GetValueOrDefault(tag, Pos.None);
        }

        public Gender GenderByTag(string tag)
        {
            return Genders.GetValueOrDefault(tag, Gender.None);
        }

        public Number NumberByTag(string tag)
        {
            return Numbers.GetValueOrDefault(tag, Number.None);
        }

        public Case CaseByTag(string tag)
        {
            return Cases.GetValueOrDefault(tag, Case.None);
        }

        public Tense TenseByTag(string tag)
        {
            return Tenses.GetValueOrDefault(tag, Tense.None);
        }

        public Person PersonByTag(string tag)
        {
            return Persons.GetValueOrDefault(tag, Person.None);
        }

        private void AddToDictionary<T>(
            string tag,
            T currentValue,
            Dictionary<string, T> dictionary,
            T defaultValue = default
        ) where T : Enum
        {
            if (Equals(currentValue, defaultValue)) return;
            
            if (dictionary.ContainsKey(tag))
                throw new InvalidOperationException($"Tag {tag} is already in {nameof(dictionary)} dictionary");
                
            dictionary.Add(tag, currentValue);
        }
    }
}