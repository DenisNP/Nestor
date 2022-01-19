using System;

namespace Nestor.Poetry
{
    public record WordWithStress
    {
        public string Word { get; }
        public byte Stress { get; }
        public int StressIndex { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="word">String contains word</param>
        /// <param name="stress">Number of stressed vowel starting from 1</param>
        public WordWithStress(string word, byte stress)
        {
            Word = word.ToLower().Trim();
            Stress = stress;
            StressIndex = NestorMorph.GetStressIndex(word, stress);

            if (stress < 1)
            {
                throw new ArgumentException("Stress is a number of stressed vowel starting from 1");
            }
            
            if (string.IsNullOrEmpty(Word))
            {
                throw new ArgumentException("Word cannot be null or empty");
            }
            
            if (StressIndex < 0)
            {
                throw new ArgumentException(
                    "Stress is a number of stressed vowel. Probably stress is larger than number of vowels in word."
                );
            }
        }

        public string GetTailAfterStress(bool addCharBeforeStress = false)
        {
            int startIndex = StressIndex;
            if (startIndex > 0 && addCharBeforeStress)
            {
                var prevLetter = Word[startIndex - 1].ToString();
                if (!NestorMorph.IsVowel(prevLetter))
                {
                    startIndex--;
                }
            }

            return Word[startIndex..];
        }
    }
}