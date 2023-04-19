using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nestor.Data;

namespace Nestor.DictBuilder
{
    internal static class ParadigmGenerator
    {
        /// <summary>
        /// Generate paradigm from dict lines
        /// </summary>
        /// <param name="lines">Dict lines</param>
        /// <param name="storage">Storage</param>
        /// <param name="altForms">Word alternative forms</param>
        /// <returns>Paradigm and stem for word</returns>
        public static (ushort[] paradigm, string Stem) Generate(List<string> lines, HashedStorage storage, out HashSet<string> altForms)
        {
            var forms = ExtractForms(lines);
            if (forms.Length == 0)
            {
                altForms = null;
                return (new ushort[0], null);
            }
            
            var stem = FindStem(forms.Select(f => f.word).ToList());
            altForms = new HashSet<string>();
            
            // fill paradigm rules with deconstructed values
            var rules = new List<MorphRule>();

            foreach (var (word, tags, accent, altForm) in forms)
            {
                var (prefix, suffix) = RemoveStem(word, stem);

                rules.Add(new MorphRule
                {
                    Prefix = (ushort)storage.AddPrefix(prefix),
                    Suffix = (ushort)storage.AddSuffix(suffix),
                    Stress = (byte)accent,
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
                .ThenBy(r => r.Stress)
                .ToList();
            
            // return lemma
            rules.Insert(0, lemmaRule);
            
            // fill paradigm and return
            return (ParadigmHelper.FromRules(rules), stem);
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

                // skip forms with spaces
                if (lineData[0].Trim().Contains(" ")) 
                    continue;

                // extract word form
                var firstWord = Regex.Replace(lineData[0], "[^а-яё\\-]+", "");
                var secondWord = Regex.Replace(lineData[2], "[^а-яё\\-']+", "");
                var secondWordClean = Regex.Replace(secondWord, "[^а-яё\\-]+", "");
                var lengthDiffers = firstWord.Length != secondWordClean.Length;

                // multiple accents
                int[] accentIndexes = secondWord
                    .Select((c, i) => (c, i))
                    .Where(x => x.c.ToString() == "'")
                    .Select(x => x.i)
                    .ToArray();

                if (accentIndexes.Length == 0 || lengthDiffers)
                    accentIndexes = new[] {-1};

                foreach (int accentIndex in accentIndexes)
                {
                    // create word with only one current accent
                    var secondSubword = accentIndex == -1
                        ? secondWord
                        : secondWord[..accentIndex].Replace("'", "") + "'" + secondWord[(accentIndex + 1)..].Replace("'", "");
                    
                    // accent
                    var accent = lengthDiffers ? -1 : FindAccent(secondSubword);
                    secondSubword = secondSubword.Replace("'", "");

                    // tags
                    var tags = lineData[1].Trim().Split(" ");

                    // if second form is different from first one
                    if (secondSubword != firstWord)
                    {
                        // if it is just 'ё' letter
                        if (secondSubword.Replace("ё", "е") == firstWord)
                        {
                            // add to result list
                            result.Add((secondSubword, tags, accent, firstWord));
                        }
                        else
                        {
                            // other types
                            result.Add((firstWord, tags, accent, secondSubword));
                        }
                    }
                    else
                    {
                        // just add result
                        result.Add((firstWord, tags, accent, secondSubword));
                    }
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