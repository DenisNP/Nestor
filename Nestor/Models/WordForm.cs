using Nestor.Data;

namespace Nestor.Models
{
    public class WordForm
    {
        public string Word { get; }
        public int Stress { get; }

        public Tag Tag { get; }
        
        public string[] Grammemes { get; }
        
        internal WordForm(string word, int stress, string[] grammemes, Storage storage)
        {
            Word = word;
            Stress = stress;
            Grammemes = grammemes;

            Tag = new Tag(grammemes, storage);
        }
    }
}