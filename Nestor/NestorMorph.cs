using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            _dawg = Dawg<string[]>.Load(File.OpenRead("dict.bin"),
                reader =>
                {
                    var str = reader.ReadString();
                    return str.Split("|");
                });
            Console.WriteLine("Ok");
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

            return inputList.Count() - matches <= maxDifference;
        }

        private IEnumerable<string> CleanString(string s, bool removePrepositions)
        {
            return Regex.Split(s.ToLower().Trim(), "[^а-яё\\-]+")
                .Where(t => !removePrepositions && !Prepositions.Contains(t));
        }

        public bool HasOneOfLemmas(string inputWord, params string[] lemmas)
        {
            return lemmas.Any(l => HasLemma(inputWord, l));
        }

        public bool HasLemma(string inputWord, string lemma)
        {
            var word = inputWord.ToLower().Trim();
            if (word == lemma) return true;
            var lemmas = _dawg[word];

            return lemmas != null && lemmas.Contains(lemma);
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