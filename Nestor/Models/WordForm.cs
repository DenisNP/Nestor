using System.Collections.Generic;
using Nestor.Data;

namespace Nestor.Models
{
    public class WordForm
    {
        public string Word { get; }
        public int Stress { get; }

        public Tag Tag { get; }
        
        public string[] Grammemes { get; }
        
        private readonly HashSet<string> _vowels;

        internal WordForm(string word, int stress, string[] grammemes, Storage storage, HashSet<string> vowels)
        {
            _vowels = vowels;
            Word = word;
            Stress = stress;
            Grammemes = grammemes;

            Tag = new Tag(grammemes, storage);
        }

        public int GetStressIndex()
        {
            if (Stress <= 0) return -1;
            var k = 0;
            
            for (var i = 0; i < Word.Length; i++)
            {
                char chr = Word[i];
                if (_vowels.Contains(chr.ToString())) k++;

                if (k == Stress) return i;
            }

            return -1;
        }
    }
}