using Nestor.Data;

namespace Nestor.Models
{
    public struct Word
    {
        public string Stem { get; set; }
        public ushort ParadigmId { get; set; }

        public Word(string rawString)
        {
            var data = rawString.Split(";");
            Stem = data[0];
            ParadigmId = ushort.Parse(data[1]);
        }

        public string[] GetAllForms(Paradigm paradigm, IStorage storage)
        {
            var forms = new string[paradigm.Rules.Length];
            for (var i = 0; i < paradigm.Rules.Length; i++)
            {
                forms[i] = GetForm(i, paradigm, storage);
            }

            return forms;
        }

        public string GetForm(int idx, Paradigm paradigm, IStorage storage)
        {
            return $"{storage.GetPrefix(paradigm.Rules[idx].Prefix)}{Stem}{storage.GetSuffix(paradigm.Rules[idx].Suffix)}";
        }

        public string Lemma(Paradigm paradigm, Storage storage)
        {
            return GetForm(0, paradigm, storage);
        }

        public override string ToString()
        {
            return $"{Stem};{ParadigmId}";
        }
    }
}