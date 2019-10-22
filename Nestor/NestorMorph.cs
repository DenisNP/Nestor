using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DawgSharp;

namespace Nestor
{
    public class NestorMorph
    {
        private Dawg<string[]> _dawg;
        private static readonly string[] Prepositions =
        {
            "или", 
            "иль",
            "о",
            "в", 
            "у", 
            "к", 
            "а", 
            "и", 
            "с", 
            "на", 
            "по", 
            "за", 
            "ко", 
            "из", 
            "об", 
            "от", 
            "во", 
            "то",
            "до",
            "без", 
            "про", 
            "вне", 
            "для", 
            "изо", 
            "меж", 
            "над", 
            "под", 
            "обо", 
            "ото", 
            "при"
        };

        public NestorMorph()
        {
            Console.Write("Nestor loading data...");

            _dawg = Dawg<string[]>.Load(LoadFile("dict.bin"),
                reader =>
                {
                    var str = reader.ReadString();
                    var lemmas = str.Split("|").Where(l => l != "");
                    return lemmas.ToArray();
                });
            
            Console.WriteLine("Ok");
        }

        private Stream LoadFile(string name)
        {
            try
            {
                var assembly = Assembly.GetCallingAssembly();
                var file = assembly.GetManifestResourceStream("Nestor." + name);
                if (file != null)
                {
                    return file;
                }
            }
            catch (Exception _)
            {
                // ignored
            }

            return File.OpenRead(name);
        }

        public bool CheckPhrase(string inputPhrase, bool removePrepositions, params string[] expected)
        {
            return CheckPhrase(inputPhrase, -1, removePrepositions, expected);
        }

        public bool CheckPhrase(
            string inputPhrase,
            int maxDifference,
            bool removePrepositions,
            params string[] expected
        )
        {
            var inputTokens = CleanString(inputPhrase, removePrepositions);
            return expected.Any(
                expectedPhrase => CheckPhraseCleared(
                    inputTokens,
                    maxDifference,
                    expectedPhrase
                )
            );
        }

        public bool CheckPhrase(
            string inputPhrase,
            int maxDifference,
            bool removePrepositions,
            string expected
        )
        {
            return CheckPhraseCleared(
                CleanString(
                    inputPhrase,
                    removePrepositions
                ),
                maxDifference,
                expected
            );
        }

        private bool CheckPhraseCleared(IEnumerable<string> input, int maxDifference, string expected)
        {
            var expectedTokens = new HashSet<string>(expected.Split(" "));
            var matches = 0;
            var inputList = input.ToList();
            if (maxDifference == -1)
            {
                maxDifference = DefaultParameters(inputList.Count, expectedTokens.Count);
            }
            
            foreach (var s in inputList)
            {
                var expectedMatch = expectedTokens.FirstOrDefault(exp => HasLemma(s, exp));
                if (string.IsNullOrEmpty(expectedMatch)) continue;
                
                matches++;
                expectedTokens.Remove(expectedMatch);
            }

            return expectedTokens.Count == 0 && inputList.Count - matches <= maxDifference;
        }
        
        public IEnumerable<string> Lemmatize(string phrase, bool removeUndictionaried = false, bool allLemmas = false)
        {
            var tokens = Regex.Split(phrase.ToLower().Trim(), "[^\\w\\-]").Where(x => x != "");
            return Lemmatize(tokens, removeUndictionaried);
        }

        public IEnumerable<string> Lemmatize(IEnumerable<string> phrase, bool removeUndictionaried = false, bool allLemmas = false)
        {
            var list = new List<string>();
            
            foreach (var p in phrase)
            {
                var l = GetLemmas(p);
                if (l == null)
                {
                     if (!removeUndictionaried) list.Add(p);
                     continue;
                }

                if (l.Length == 0)
                {
                    list.Add(p);
                    continue;
                }

                if (allLemmas)
                {
                    list.AddRange(l);
                }
                else
                {
                    list.Add(l.First());
                }
            }
            
            return list;
        }

        private IEnumerable<string> CleanString(string s, bool removePrepositions)
        {
            return Regex.Split(s.ToLower().Trim(), "[^а-яё\\-]+")
                .Where(t => !removePrepositions || !Prepositions.Contains(t));
        }

        public bool HasOneOfLemmas(string inputWord, params string[] lemmas)
        {
            return lemmas.Any(l => HasLemma(inputWord, l));
        }

        public bool HasLemma(string inputWord, string lemma)
        {
            var word = inputWord.ToLower().Trim();
            if (word == lemma) return true;
            var lemmas = GetLemmas(word);

            return lemmas != null && lemmas.Contains(lemma);
        }

        public string[] GetLemmas(string inputWord)
        {
            var word = inputWord.ToLower().Trim();
            var lemmas = _dawg[word];
            if (lemmas != null && lemmas.Length == 0) return new[] {word};
            return lemmas;
        }

        private static int DefaultParameters(int inputLength, int expectedLength)
        {
            switch (expectedLength)
            {
                case 1:
                case 2:
                    return 1;
                case 3:
                    return 2;
                case 4:
                    return 3;    
            }

            return 5;
        }
    }
}