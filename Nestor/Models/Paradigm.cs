namespace Nestor.Models
{
    public class Paradigm
    {
        public string Stem { get; set; }
        public MorphRule[] Rules { get; set; }
        public bool TwoLemmas { get; set; }
        
        private readonly Storage _storage;
        
        public Paradigm(Storage storage)
        {
            _storage = storage;
        }

        public bool IsEqualTo(Paradigm other)
        {
            return ToString() == other.ToString();
        }

        public string[] GetAllForms()
        {
            var forms = new string[Rules.Length];
            for (var i = 0; i < Rules.Length; i++)
            {
                forms[i] = GetForm(i);
            }

            return forms;
        }

        public string GetForm(int idx)
        {
            return $"{_storage.GetPrefix(Rules[idx].Prefix)}{Stem}{_storage.GetSuffix(Rules[idx].Suffix)}";
        }

        public string[] Lemmas()
        {
            return TwoLemmas ? new[] {GetForm(0), GetForm(1)} : new[] {GetForm(0)};
        }

        public override string ToString()
        {
            return $"{Stem}" +
                   $"!{Rules.Join("|")}";
        }
    }

    public struct MorphRule
    {
        public int Prefix;
        public int Suffix;
        public int Accent;
        public int[] Tags;

        public override string ToString()
        {
            return $"{Prefix};{Suffix};{Accent};{Tags.Join(",")}";
        }
    }
}