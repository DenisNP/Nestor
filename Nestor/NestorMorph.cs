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
        private static readonly HashSet<string> Prepositions = new HashSet<string>();
        private static readonly Storage Storage = new Storage();
        private static readonly List<ushort[]> Paradigms = new List<ushort[]>();

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
            var wForm = options != MorphOption.None ? Clean(wordForm, options) : wordForm; 
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
                return new []{ new Word(raw, Storage, Paradigms) };
            }

            return wordIds.Select(WordById).ToArray();
        }

        /// <summary>
        /// Get exact word forms
        /// </summary>
        /// <param name="wordForm">Word form string</param>
        /// <param name="options">Additional options for operation</param>
        /// <returns>Word forms array</returns>
        public WordForm[] WordForms(string wordForm, MorphOption options = MorphOption.None)
        {
            var form = options != MorphOption.None ? Clean(wordForm, options) : wordForm; 
            var words = WordInfo(form);
            return words.SelectMany(w => w.Forms.Where(f => f.Word == form)).ToArray();
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
                .Where(t => !options.HasFlag(MorphOption.RemovePrepositions) || !Prepositions.Contains(t))
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
            var wordRaw = Storage.GetWord(id);
            return new Word(wordRaw, Storage, Paradigms);
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
}