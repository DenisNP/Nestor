using System.Collections.Generic;
using System.Linq;
using Nestor.Data;

namespace Nestor.Models
{
    public class Word
    {
        public string Stem { get; }
        public WordForm[] Forms { get; }
        public WordForm Lemma => Forms[0];

        public Word(WordRaw raw, Storage storage, List<ushort[]> paradigms)
        {
            Stem = raw.Stem;
            var paradigm = raw.ParadigmId == 0 ? ParadigmHelper.Empty() : paradigms[raw.ParadigmId - 1];

            Forms = new WordForm[paradigm.Length / 4];
            for (var i = 0; i < Forms.Length; i++)
            {
                var tagGroup = storage.GetTagGroup(paradigm[Forms.Length * 3 + i]);
                Forms[i] = new WordForm
                {
                    Word = GetForm(i, paradigm, Stem, storage),
                    Accent = paradigm[Forms.Length * 2 + i],
                    Tags = tagGroup.Select(tag => storage.GetTag(tag)).ToHashSet()
                };
            }
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