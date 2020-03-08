using Nestor.Data;

namespace Nestor.Models
{
    public class Paradigm
    {
        public MorphRule[] Rules { get; set; }
        
        private readonly IStorage _storage;

        public Paradigm(IStorage storage, string rawLine)
        {
            _storage = storage;
            var rawData = rawLine.Split("!");

            var rulesData = rawData[1].Split("|");
            Rules = new MorphRule[rulesData.Length];

            for (var i = 0; i < rulesData.Length; i++)
            {
                Rules[i] = (new MorphRule()).FromString(rulesData[i]);
            }
        }
        
        public Paradigm(IStorage storage)
        {
            _storage = storage;
        }
        
        public override string ToString()
        {
            return Rules.Join("|");
        }
    }

    public struct MorphRule
    {
        public ushort Prefix;
        public ushort Suffix;
        public byte Accent;
        public ushort TagGroup;

        public override string ToString()
        {
            return $"{Prefix};{Suffix};{Accent}{TagGroup}";
        }

        public MorphRule FromString(string s)
        {
            var data = s.Split(";");
            
            Prefix = ushort.Parse(data[0]);
            Suffix = ushort.Parse(data[1]);
            Accent = byte.Parse(data[2]);
            TagGroup = ushort.Parse(data[3]);
            
            return this;
        }
    }
}