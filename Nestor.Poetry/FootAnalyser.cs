using System;
using System.Collections.Generic;
using System.Linq;
using Nestor.Models;

namespace Nestor.Poetry
{ 
    public class FootAnalyser
    {
        private readonly NestorMorph _nestor;
        private const int WordToMaskMismatchPenalty = 5;
        private const int MaskToWordMismatchPenalty = 2;

        public FootAnalyser(NestorMorph nestor = null)
        {
            _nestor = nestor ?? new NestorMorph();
        }
        
        /// <summary>
        /// Find best matching foot for entire poem
        /// </summary>
        /// <param name="poem">String contains lines of poem in Russian, separated by newline character</param>
        /// <returns>Best foot matched</returns>
        public Foot FindBestFootByPoem(string poem)
        {
            IEnumerable<string> lines = poem.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Where(l => !string.IsNullOrEmpty(l.Trim()));
            
            var distances = new Dictionary<Foot, int>();
            foreach (string line in lines)
            {
                (Foot foot, int distance)[] result = ScoreAllFootsByLine(line);
                foreach ((Foot foot, int distance) in result)
                {
                    if (!distances.ContainsKey(foot))
                    {
                        distances.Add(foot, distance);
                    }
                    else
                    {
                        distances[foot] += distance;
                    }
                }
            }
            
            // sort result and return
            KeyValuePair<Foot, int> best = distances.OrderBy(d => d.Value).First();
            return best.Key;
        }

        /// <summary>
        /// Find best matching foot for current line
        /// </summary>
        /// <param name="line">String contains single poem line in Russian</param>
        /// <param name="distance">Distance to foot found, less is better</param>
        /// <returns>Best foot matched</returns>
        public Foot FindBestFootByLine(string line, out int distance)
        {
            (Foot foot, int distance)[] allFoots = ScoreAllFootsByLine(line);
            distance = allFoots.First().distance;
            return allFoots.First().foot;
        }

        /// <summary>
        /// Score all basic foots by distance to current line, less distance is better
        /// </summary>
        /// <param name="line">String contains single poem line</param>
        /// <returns>Array af all foots with distances</returns>
        public (Foot foot, int distance)[] ScoreAllFootsByLine(string line)
        {
            string[] tokens = _nestor.Tokenize(line, MorphOption.RemoveHyphen);
            StressType[] lineStresses = tokens.SelectMany(GetPoeticStresses).ToArray();
            
            // score all foots foot
            var result = new List<(Foot foot, int distance)>();
            foreach (FootType footType in Enum.GetValues<FootType>())
            {
                if (footType == FootType.Unknown) continue;
                var foot = new Foot(footType);
                result.Add((foot, DistanceToFoot(foot, lineStresses)));
            }

            return result.OrderBy(r => r.distance).ToArray();
        }

        /// <summary>
        /// Calculate distance between line stresses and foot stresses
        /// </summary>
        /// <param name="foot">Current foot</param>
        /// <param name="lineStresses">Current line stresses</param>
        /// <returns>Distance, less is better</returns>
        public int DistanceToFoot(Foot foot, IList<StressType> lineStresses)
        {
            StressType[] mask = foot.GetMaskOfLength(lineStresses.Count);
            var dist = 0;
            for (var i = 0; i < mask.Length; i++)
            {
                StressType lineStress = lineStresses[i];
                StressType maskStress = mask[i];
                
                // if line vowel can be stressed or not, this is zero distance to mask
                // otherwise:
                if (lineStress != StressType.CanBeStressed)
                {
                    switch (lineStress)
                    {
                        // mask is strictly unstressed when line vowel is not, this is a big penalty
                        case StressType.StrictlyStressed when maskStress == StressType.StrictlyUnstressed:
                            dist += WordToMaskMismatchPenalty;
                            break;
                        // mask can be stressed when line vowel in unstressed, light penalty
                        case StressType.StrictlyUnstressed when maskStress == StressType.CanBeStressed:
                            dist += MaskToWordMismatchPenalty;
                            break;
                    }
                    
                    // when line vowel is strictly stressed and mask can be stressed, there is no penalty
                    // when line vowel and mask both strictly unstressed, there is no penalty too
                }
            }

            return dist;
        }
        
        /// <summary>
        /// Get all possible word stresses accounding to possible poetic foot
        /// </summary>
        /// <param name="word">Input word</param>
        /// <returns>Array of stresses, each index is a vowel number in the word starting from zero</returns>
        public StressType[] GetPoeticStresses(string word)
        {
            int vCount = word.Count(NestorMorph.IsVowel);
            
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