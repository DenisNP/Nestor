using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nestor.Models;

namespace Nestor.DictBuilder
{
    public static class ParadigmGenerator
    {
        /// <summary>
        /// Generate paradigm from dict lines
        /// </summary>
        /// <param name="lines">Dict lines</param>
        /// <param name="storage">Storage</param>
        /// <returns>Paradigm object</returns>
        public static Paradigm Generate(List<string> lines, HashedStorage storage, out HashSet<string> altForms)
        {
            var forms = ExtractForms(lines);
            var stem = FindStem(forms.Select(f => f.word).ToList());
            altForms = new HashSet<string>();

            // create new paradigm
            var paradigm = new Paradigm(storage) { Stem = stem };
            
            // fill paradigm rules with deconstructed values
            var rules = new List<MorphRule>();

            foreach (var (word, tags, accent, altForm) in forms)
            {
                var (prefix, suffix) = RemoveStem(word, stem);

                rules.Add(new MorphRule
                {
                    Prefix = (ushort)storage.AddPrefix(prefix),
                    Suffix = (ushort)storage.AddSuffix(suffix),
                    Accent = (byte)accent,
                    TagGroup = (ushort)storage.AddTagGroup(tags)
                });

                if (altForm != "")
                {
                    altForms.Add(altForm);
                }
            }

            // sort paradigm rules, exclude first
            var lemmaRule = rules.First();
            rules.RemoveAt(0);

            rules = rules
                .OrderBy(r => r.Prefix)
                .ThenBy(r => r.Suffix)
                .ThenBy(r => r.TagGroup)
                .ThenBy(r => r.Accent)
                .ToList();
            
            // return lemma
            rules.Insert(0, lemmaRule);
            
            // fill paradigm and return
            paradigm.Rules = rules.ToArray();
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
        private static (string word, string[] tags, int accent, string altForm)[] ExtractForms(List<string> lines)
        {
            var result = new List<(string word, string[] tags, int accent, string altForm)>();
            
            foreach (var line in lines)
            {
                var lineData = line.Split("|");

                // extract word form
                var firstWord = Regex.Replace(lineData[0], "[^а-яё\\-]+", "");
                var secondWord = Regex.Replace(lineData[2], "[^а-яё\\-']+", "");

                // accent
                var accent = FindAccent(secondWord);
                secondWord = secondWord.Replace("'", "");

                // tags
                var tags = lineData[1].Trim().Split(" ");

                // if second form is different from first one
                if (secondWord != firstWord)
                {
                    // if it is just 'ё' letter
                    if (secondWord.Replace("ё", "е") == firstWord)
                    {
                        // add to result list
                        result.Add((secondWord, tags, accent, firstWord));
                    }
                    else
                    {
                        // other types
                        result.Add((firstWord, tags, accent, secondWord));
                    }
                }
                else
                {
                    // just add result
                    result.Add((firstWord, tags, accent, secondWord));
                }
            }

            return result.ToArray();
        }

        private static int FindAccent(string word)
        {
            const string vowels = "аоуыэяёюие";
            var vCount = 0;
            for (var i = 0; i < word.Length; i++)
            {
                var chr = word.Substring(i, 1);
                if (vowels.Contains(chr))
                {
                    vCount++;
                } 
                else if (chr == "'")
                {
                    return vCount;
                }
            }

            if (vCount == 1)
            {
                return 1;
            }

            return 0; // no accent presented
        }
    }
}