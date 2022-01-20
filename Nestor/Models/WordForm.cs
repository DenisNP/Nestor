using System;
using Nestor.Data;

namespace Nestor.Models
{
    public class WordForm
    {
        public string Word { get; }
        public int Stress { get; private set; }

        public Tag Tag { get; }
        
        public string[] Grammemes { get; }
        
        internal WordForm(string word, int stress, string[] grammemes, Storage storage)
        {
            Word = word;
            Stress = stress;
            Grammemes = grammemes;

            Tag = new Tag(grammemes, storage);
        }

        public void SetStress(int stress)
        {
            if (Stress != -1)
            {
                throw new InvalidOperationException("You can set only unknown stress");
            }

            int index = NestorMorph.GetStressIndex(Word, stress);
            if (index == -1)
            {
                throw new ArgumentException("Stress should be number of stressed vowel started from 1");
            }
            
            Stress = stress;
        }
    }
}