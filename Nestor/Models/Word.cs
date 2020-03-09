using System.Collections.Generic;
using Nestor.Data;

namespace Nestor.Models
{
    public class Word
    {
        public string Stem { get; }
        
        private readonly Storage _storage;
        private ushort[] _paradigm;
        private readonly short _paradigmId;

        public Word(WordRaw raw, Storage storage, List<ushort[]> paradigms)
        {
            Stem = raw.Stem;
            _storage = storage;
            _paradigmId = raw.ParadigmId;
            _paradigm = _paradigmId == -1 ? ParadigmHelper.Empty() : paradigms[_paradigmId];
        }
        
        public string[] GetAllForms()
        {
            return GetAllForms(_paradigm, Stem, _storage);
        }

        public string GetForm(int idx, Storage storage, List<ushort[]> paradigms)
        {
            return GetForm(idx, _paradigm, Stem, storage);
        }

        public string Lemma(Storage storage, List<ushort[]> paradigms)
        {
            return GetForm(0, storage, paradigms);
        }

        public static string[] GetAllForms(ushort[] paradigm, string stem, Storage storage)
        {
            var forms = new string[paradigm.Length / 4];
            for (var i = 0; i < forms.Length; i++)
            {
                forms[i] = GetForm(i, paradigm, stem, storage);
            }

            return forms;
        }

        public static string GetForm(int idx, ushort[] paradigm, string stem, Storage storage)
        {
            var formsCount = paradigm.Length / 4;
            return $"{storage.GetPrefix(paradigm[idx])}{stem}{storage.GetSuffix(paradigm[formsCount + idx])}";
        }
    }
}