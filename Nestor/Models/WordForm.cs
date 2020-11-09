using Nestor.Data;

namespace Nestor.Models
{
    public class WordForm
    {
        public string Word { get; }
        public int Accent { get; }

        public Grammatics Grammatics { get; }
        
        public string[] Tags { get; }

        public WordForm(string word, int accent, string[] tags, Storage storage)
        {
            Word = word;
            Accent = accent;
            Tags = tags;

            Grammatics = new Grammatics(tags, storage);
        }
    }
}