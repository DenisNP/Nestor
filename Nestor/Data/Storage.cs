using System;
using System.Collections.Generic;
using Nestor.Models;

namespace Nestor.Data
{
    public class Storage
    {
        protected readonly List<string> Prefixes = new List<string>();
        protected readonly List<string> Suffixes = new List<string>();
        protected readonly List<string> Grammemes = new List<string>();
        protected readonly List<byte[]> Tags = new List<byte[]>();
        protected readonly List<WordRaw> Words = new List<WordRaw>();
        
        protected readonly Dictionary<string, Pos> PartsOfSpeech = new Dictionary<string, Pos>();
        protected readonly Dictionary<string, Gender> Genders = new Dictionary<string, Gender>();
        protected readonly Dictionary<string, Number> Numbers = new Dictionary<string, Number>();
        protected readonly Dictionary<string, Case> Cases = new Dictionary<string, Case>();
        protected readonly Dictionary<string, Tense> Tenses = new Dictionary<string, Tense>();
        protected readonly Dictionary<string, Person> Persons = new Dictionary<string, Person>();

        public string GetPrefix(int id)
        {
            return Prefixes.GetOrEmpty(id);
        }

        public string GetSuffix(int id)
        {
            return Suffixes.GetOrEmpty(id);
        }

        public string GetGrammeme(int id)
        {
            return Grammemes.GetOrEmpty(id);
        }

        public byte[] GetTag(int id)
        {
            return id == 0 ? Array.Empty<byte>() : Tags[id - 1];
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

        public List<string> GetGrammemes()
        {
            return Grammemes;
        }

        public List<byte[]> GetTags()
        {
            return Tags;
        }

        public List<WordRaw> GetWords()
        {
            return Words;
        }

        public void ParseGrammemes()
        {
            foreach (string grammeme in Grammemes)
            {
                AddToDictionary(grammeme, GrammemeMapper.GetPos(grammeme), PartsOfSpeech);
                AddToDictionary(grammeme, GrammemeMapper.GetGender(grammeme), Genders);
                AddToDictionary(grammeme, GrammemeMapper.GetNumber(grammeme), Numbers);
                AddToDictionary(grammeme, GrammemeMapper.GetCase(grammeme), Cases);
                AddToDictionary(grammeme, GrammemeMapper.GetTense(grammeme), Tenses);
                AddToDictionary(grammeme, GrammemeMapper.GetPerson(grammeme), Persons);
            }
        }

        public Pos PosByGrammeme(string grammeme)
        {
            return PartsOfSpeech.GetValueOrDefault(grammeme, Pos.None);
        }

        public Gender GenderByGrammeme(string grammeme)
        {
            return Genders.GetValueOrDefault(grammeme, Gender.None);
        }

        public Number NumberByGrammeme(string grammeme)
        {
            return Numbers.GetValueOrDefault(grammeme, Number.None);
        }

        public Case CaseByGrammeme(string grammeme)
        {
            return Cases.GetValueOrDefault(grammeme, Case.None);
        }

        public Tense TenseByGrammeme(string grammeme)
        {
            return Tenses.GetValueOrDefault(grammeme, Tense.None);
        }

        public Person PersonByGrammeme(string grammeme)
        {
            return Persons.GetValueOrDefault(grammeme, Person.None);
        }

        private void AddToDictionary<T>(
            string grammeme,
            T currentValue,
            Dictionary<string, T> dictionary,
            T defaultValue = default
        ) where T : Enum
        {
            if (Equals(currentValue, defaultValue)) return;
            
            if (dictionary.ContainsKey(grammeme))
                throw new InvalidOperationException($"Grammeme {grammeme} is already in {typeof(T)} dictionary");
                
            dictionary.Add(grammeme, currentValue);
        }
    }
}