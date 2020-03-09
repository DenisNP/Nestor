using Nestor.Data;

namespace Nestor.Models
{
    public struct Word
    {
        public string Stem { get; set; }
        public ushort ParadigmId { get; set; }

        public Word(string rawString)
        {
            var data = rawString.Split("|");
            Stem = data[0];
            ParadigmId = ushort.Parse(data[1]);
        }

        public string[] GetAllForms(ushort[] paradigm, Storage storage)
        {
            var forms = new string[paradigm.Length / 4];
            for (var i = 0; i < forms.Length; i++)
            {
                forms[i] = GetForm(i, paradigm, storage);
            }

            return forms;
        }

        public string GetForm(int idx, ushort[] paradigm, Storage storage)
        {
            var formsCount = paradigm.Length / 4;
            return $"{storage.GetPrefix(paradigm[idx])}{Stem}{storage.GetSuffix(paradigm[formsCount + idx])}";
        }

        public string Lemma(ushort[] paradigm, Storage storage)
        {
            return GetForm(0, paradigm, storage);
        }

        public override string ToString()
        {
            return $"{Stem}|{ParadigmId}";
        }
    }
}