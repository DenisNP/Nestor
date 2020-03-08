using System.Linq;

namespace Nestor.Models
{
    public class Paradigm
    {
        public string Stem { get; set; }
        public MorphRule[] Rules { get; set; }
        
        private readonly Storage _storage;

        public Paradigm(Storage storage, string rawLine)
        {
            _storage = storage;
            var rawData = rawLine.Split("!");
            Stem = rawData[0];

            var rulesData = rawData[1].Split("|");
            Rules = new MorphRule[rulesData.Length];

            for (var i = 0; i < rulesData.Length; i++)
            {
                Rules[i] = (new MorphRule()).FromString(rulesData[i]);
            }
        }
        
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

        public string Lemma()
        {
            return GetForm(0);
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

        public MorphRule FromString(string s)
        {
            var data = s.Split(";");
            Prefix = int.Parse(data[0]);
            Suffix = int.Parse(data[1]);
            Accent = int.Parse(data[2]);
            Tags = data[3].Split(",").Select(int.Parse).ToArray();
            
            return this;
        }
    }
}