using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DawgSharp;
using Nestor.Data;
using Nestor.Models;

namespace Nestor
{
    public partial class NestorMorph
    {
        private Dawg<int> _dawgSingle;
        private Dawg<int[]> _dawgMulti;
        private readonly HashSet<string> _prepositions = new HashSet<string>();
        private readonly Storage _storage = new Storage();
        private readonly List<ushort[]> _paradigms = new List<ushort[]>();

        public NestorMorph()
        {
            LoadAdditional();
            LoadParadigms();
            LoadWords();
            LoadMorphology();
            GC.Collect();
        }

        /// <summary>
        /// Get info about entire word by its single form
        /// </summary>
        /// <param name="wordForm">Word form</param>
        /// <param name="options">Additional options for operation</param>
        /// <returns>List of all words from its form</returns>
        public Word[] WordInfo(string wordForm, MorphOption options = MorphOption.None)
        {
            return WordInfo(wordForm, out _, options);
        }

        /// <summary>
        /// Get info about entire word by its single form
        /// </summary>
        /// <param name="wordForm">Word form</param>
        /// <param name="cleanWord">Input word after cleaning</param>
        /// <param name="options">Additional options for operation</param>
        /// <returns>List of all words from its form</returns>
        public Word[] WordInfo(string wordForm, out string cleanWord, MorphOption options = MorphOption.None)
        {
            var wForm = options != MorphOption.None ? Clean(wordForm, options) : wordForm;
            cleanWord = wordForm;
            int[] wordIds = null;
            var single = _dawgSingle[wForm];
            if (single == 0)
            {
                var multiple = _dawgMulti[wForm];
                if (multiple != null)
                {
                    wordIds = multiple;
                }
            }
            else
            {
                wordIds = new[] {single};
            }

            // word not found, return default with its initial form
            if (wordIds == null)
            {
                var raw = new WordRaw
                {
                    Stem = wordForm,
                    ParadigmId = 0
                };
                return new []{ new Word(raw, _storage, _paradigms) };
            }

            return wordIds.Select(WordById).ToArray();
        }

        /// <summary>
        /// Tokenize input string to cyrillic words lowercased
        /// </summary>
        /// <param name="s">Input string</param>
        /// <param name="options">Additional options for operation</param>
        /// <returns>Array of tokens</returns>
        public string[] Tokenize(string s, MorphOption options = MorphOption.None)
        {
            var regex = "[^а-яё";
            if (options.HasFlag(MorphOption.KeepNumbers))
            {
                regex += "0-9";
            }
            if (options.HasFlag(MorphOption.KeepLatin))
            {
                regex += "a-z";
            }
            if (!options.HasFlag(MorphOption.RemoveHyphen))
            {
                regex += "\\-";
            }
            regex += "]+";
            
            var tokens = Regex.Split(s.ToLower(), regex)
                .Select(t => t.Trim().Trim('-'))
                .Where(t => t != "")
                .Where(t => !options.HasFlag(MorphOption.RemovePrepositions) || !_prepositions.Contains(t))
                .Where(t => !options.HasFlag(MorphOption.RemoveNonExistent) || WordExists(t));

            return options.HasFlag(MorphOption.Distinct)
                ? tokens.Distinct().ToArray()
                : tokens.ToArray();
        }

        /// <summary>
        /// Remove all non-cyrillic symbols and turn string to lowercase
        /// </summary>
        /// <param name="s">Input string</param>
        /// <param name="options">Additional options for operation</param>
        /// <returns>Cleaned string</returns>
        public string Clean(string s, MorphOption options = MorphOption.None)
        {
            return Tokenize(s, options).Join(" ");
        }

        /// <summary>
        /// Convert all words in string to its lemmas
        /// </summary>
        /// <param name="s">Input string</param>
        /// <param name="options">Additional options for operation</param>
        /// <returns>Array of lemmas</returns>
        public string[] Lemmatize(string s, MorphOption options = MorphOption.None)
        {
            var tokens = Tokenize(s, options);
            var words = tokens.Select(t => WordInfo(t));

            var selectedWords = words
                .SelectMany(w => options.HasFlag(MorphOption.InsertAllLemmas) ? w : new[] {w[0]})
                .Select(w => w.Lemma.Word);

            return options.HasFlag(MorphOption.Distinct) 
                ? selectedWords.Distinct().ToArray() 
                : selectedWords.ToArray();
        }

        /// <summary>
        /// Check if word form exists in dictionary
        /// </summary>
        /// <param name="wordForm">Word form to check</param>
        /// <returns>True if exists</returns>
        public bool WordExists(string wordForm)
        {
            return _dawgSingle[wordForm] != 0 || _dawgMulti[wordForm] != null;
        }
        
        /// <summary>
        /// Get word by id
        /// </summary>
        /// <param name="id">Word id</param>
        /// <returns>Word object</returns>
        private Word WordById(int id)
        {
            var wordRaw = _storage.GetWord(id);
            return new Word(wordRaw, _storage, _paradigms);
        }
    }

    [Flags]
    public enum MorphOption
    {
        None = 0,
        RemovePrepositions = 1,
        RemoveNonExistent = 2,
        KeepNumbers = 4,
        KeepLatin = 8,
        InsertAllLemmas = 16,
        Distinct = 32,
        RemoveHyphen = 64
    }

    public enum Pos
    {
        None,
        Noun,
        Verb,
        Adjective,
        Adverb,
        Numeral,
        Participle,
        Transgressive,
        Pronoun,
        Preposition,
        Conjunction,
        Particle,
        Interjection,
        Predicative,
        Parenthesis
    }

    public enum Gender
    {
        None,
        Masculine,
        Feminine,
        Neuter,
        Common,
    }

    public enum Number
    {
        None,
        Singular,
        Plural,
    }

    public enum Case
    {
        None,
        Nominative,
        Genitive,
        Dative,
        Accusative,
        Instrumental,
        Prepositional,
        Locative,
        Partitive,
        Vocative,
    }

    public enum Tense
    {
        None,
        Past,
        Present,
        Future,
        Infinitive,
    }

    public enum Person
    {
        None,
        First,
        Second,
        Third,
    }
}