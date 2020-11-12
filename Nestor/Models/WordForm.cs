using Nestor.Data;

namespace Nestor.Models
{
    public class WordForm
    {
        public string Word { get; }
        public int Accent { get; }

        public Tag Tag { get; }
        
        public string[] Grammemes { get; }

        public WordForm(string word, int accent, string[] grammemes, Storage storage)
        {
            Word = word;
            Accent = accent;
            Grammemes = grammemes;

            Tag = new Tag(grammemes, storage);
        }
    }
}