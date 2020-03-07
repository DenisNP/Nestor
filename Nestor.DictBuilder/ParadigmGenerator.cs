using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nestor.Models;

namespace Nestor.DictBuilder
{
    public static class ParadigmGenerator
    {
        public static Paradigm Generate(List<string> lines, Storage storage)
        {
            var forms = ExtractForms(lines);
            var stem = FindStem(forms.Select(f => f.word).ToList());

            // create new paradigm
            var paradigm = new Paradigm(storage)
            {
                Stem = stem, 
                Prefixes = new int[forms.Length], 
                Suffixes = new int[forms.Length],
                Accents = new int[forms.Length],
                Tags = new int[forms.Length][]
            };
            
            // fill paradigm with deconstructed values
            for (var i = 0; i < forms.Length; i++)
            {
                var (word, tags, accent) = forms[i];
                var (prefix, suffix) = RemoveStem(word, stem);

                paradigm.Prefixes[i] = storage.AddPrefix(prefix);
                paradigm.Suffixes[i] = storage.AddSuffix(suffix);
                paradigm.Accents[i] = accent;
                paradigm.Tags[i] = storage.AddTags(tags);
            }

            return paradigm;
        }

        /// <summary>
        /// Remove stem from word and return prefix and suffix
        /// </summary>
        /// <param name="word">Word</param>
        /// <param name="stem">Stem</param>
        /// <returns>Tuple with prefix and suffix after removing</returns>
        private static (string prefix, string suffix) RemoveStem(string word, string stem)
        {
            if (stem == "")
            {
                return (word, "");
            }
            
            var startIndex = word.IndexOf(stem, StringComparison.Ordinal);
            var prefix = word.Substring(0, startIndex);
            var suffix = word.Substring(startIndex + stem.Length);

            return (prefix, suffix);
        }
        
        /// <summary>
        /// Find stem from list of words
        /// </summary>
        /// <param name="words">List of words</param>
        /// <returns>Stem string</returns>
        private static string FindStem(List<string> words)
        {
            var first = words.First();
            if (words.Count == 1)
            {
                return first;
            }

            var lastCandidate = first;
            var lastStartIndex = 0;
            var lastLength = first.Length;
            while (lastCandidate.Length > 0)
            {
                if (words.All(w => w.Contains(lastCandidate)))
                {
                    // all words contains candidate, it is a stem
                    return lastCandidate;
                }
                
                // move start index step forward
                lastStartIndex++;
                if (lastStartIndex + lastLength > first.Length)
                {
                    // shrink candidate length
                    lastLength--;
                    lastStartIndex = 0;
                }

                lastCandidate = first.Substring(lastStartIndex, lastLength);
            }

            return lastCandidate;
        }

        /// <summary>
        /// Extract words data from dictionary lines
        /// </summary>
        /// <param name="lines">Dict lines</param>
        /// <returns>Tuple list with words data</returns>
        private static (string word, string[] tags, int accent)[] ExtractForms(List<string> lines)
        {
            var result = new List<(string word, string[] tags, int accent)>();

            foreach (var line in lines)
            {
                var lineData = line.Split("|");
                
                // extract word form
                var firstWord = Regex.Replace(lineData[0], "[^а-яё\\-]+", "");
                var secondWord = Regex.Replace(lineData[2], "[^а-яё\\-']+", "");
                
                // accent
                var accent = secondWord.IndexOf("'", StringComparison.Ordinal) - 1;
                secondWord = secondWord.Replace("'", "");

                // tags
                var tags = lineData[1].Trim().Split(" ");

                // add to result list
                result.Add((firstWord, tags, accent));

                // if second form is different from first one, add it too
                if (secondWord != firstWord)
                {
                    result.Add((secondWord, tags, accent));
                }
            }
            
            return result.ToArray();
        }
    }
}