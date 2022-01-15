using System;
using System.Collections.Generic;
using System.Linq;
using Nestor.Models;

namespace Nestor.Poetry
{ 
    public class FootAnalyser
    {
        private readonly NestorMorph _nestor;

        public FootAnalyser(NestorMorph nestor = null)
        {
            _nestor = nestor ?? new NestorMorph();
        }
        
        /// <summary>
        /// Get all possible word stresses accounding to possible poetic foot
        /// </summary>
        /// <param name="word">Input word</param>
        /// <returns>Array of stresses, each index is a vowel number in the word starting from zero</returns>
        public StressType[] GetPoeticStresses(string word)
        {
            int vCount = word.Count(l => _nestor.IsVowel(l));
            
            // if there are no vowels, return empty array
            if (vCount == 0)
            {
                return Array.Empty<StressType>();
            }

                // with one vowel return single fuzzy stress
            // in poetry word with one syllable can be both stressed and unstressed
            if (vCount == 1)
            {
                return new[] { StressType.CanBeStressed };
            }

            var knownStressedVowelNumbers = new HashSet<int>();
            
            Word[] wordInfos = _nestor.WordInfo(word);
            foreach (Word wordInfo in wordInfos)
            {
                WordForm[] exactForms = wordInfo.ExactForms(word);
                foreach (WordForm form in exactForms)
                {
                    if (form.Stress > 0)
                    {
                        knownStressedVowelNumbers.Add(form.Stress);
                    }
                }
            }

            // there are no known stresses, every vowel can be stressed
            if (knownStressedVowelNumbers.Count == 0)
            {
                return Enumerable.Repeat(StressType.CanBeStressed, vCount).ToArray();
            }
            
            // only certain vowels are stresses, any other are not
            StressType[] finalStresses = Enumerable.Repeat(StressType.StrictlyUnstressed, vCount).ToArray();
            if (knownStressedVowelNumbers.Count == 1)
            {
                // strong single stress
                finalStresses[knownStressedVowelNumbers.First() - 1] = StressType.StrictlyStressed;
            }
            else
            {
                // various stresses
                foreach (int knownStressNumber in knownStressedVowelNumbers)
                {
                    finalStresses[knownStressNumber - 1] = StressType.CanBeStressed;
                }
            }

            return finalStresses;
        }
    }
}