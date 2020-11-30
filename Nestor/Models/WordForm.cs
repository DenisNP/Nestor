using Nestor.Data;

namespace Nestor.Models
{
    public class WordForm
    {
        private const string Vowels = "аоуыэяёюие";
        
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

        public int GetAccentIndex()
        {
            if (Accent <= 0) return -1;
            var k = 0;
            
            for (var i = 0; i < Word.Length; i++)
            {
                var chr = Word[i];
                if (Vowels.Contains(chr)) k++;

                if (k == Accent) return i;
            }

            return -1;
        }
    }
}