using System;
using Nestor.Data;

namespace Nestor.Models
{
    public class WordForm : IDisposable
    {
        public string Word { get; private set; }
        public int Stress { get; private set; }

        public Tag Tag { get; private set; }
        
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
            int index = NestorMorph.GetStressIndex(Word, stress);
            if (index == -1)
            {
                throw new ArgumentException("Stress should be number of stressed vowel started from 1");
            }
            
            Stress = stress;
        }

        public void Dispose()
        {
            Word = null;
            Tag = null;
        }
    }
}