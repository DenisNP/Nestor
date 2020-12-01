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
        public Tag Tag => Lemma.Tag;

        public Word(WordRaw raw, Storage storage, List<ushort[]> paradigms)
        {
            Stem = raw.Stem;
            var paradigm = raw.ParadigmId == 0 ? ParadigmHelper.Empty() : paradigms[raw.ParadigmId - 1];

            Forms = new WordForm[paradigm.Length / 4];
            for (var i = 0; i < Forms.Length; i++)
            {
                var tag = storage.GetTag(paradigm[Forms.Length * 3 + i]);
                Forms[i] = new WordForm
                (
                    GetForm(i, paradigm, Stem, storage),
                    paradigm[Forms.Length * 2 + i],
                    tag.Select(g => storage.GetGrammeme(g)).ToArray(),
                    storage
                );
            }
          
            // there is unknown GENDER for plural nouns in dictionary, fix that
            if (Tag.Pos == Pos.Noun)
            {
                foreach (var form in Forms)
                {
                    form.Tag.Gender = Tag.Gender;
                }
            }

            var lemmaTag = Lemma.Tag;

            Forms = Forms
                .Select((f, idx) => (f, idx))
                .OrderBy(x =>
                {
                    var (f, idx) = x;
                    
                    if (idx == 0) return int.MinValue;

                    var c = (int) f.Tag.Case;
                    if (c == 0) c = 100;
                    
                    if (f.Tag.Pos != lemmaTag.Pos) return c * 1000;

                    return c;
                })
                .Select(x => x.f)
                .ToArray();
        }

        public WordForm[] ExactForms(string word)
        {
            return Forms.Where(f => f.Word == word).ToArray();
        }

        public WordForm ClosestForm(
            Gender gender = Gender.None,
            Case @case = Case.None,
            Number number = Number.None,
            Tense tense = Tense.None,
            Person person = Person.None,
            bool exactMatch = true,
            bool ignoreNotDefined = false
        )
        {
            var (form, score) = Forms
                .Select(f => (f, f.Tag.DifferenceFrom(gender, @case, number, tense, person, ignoreNotDefined) + (f.Tag.Pos != Tag.Pos ? 10 : 0)))
                .MinBy(x => x.Item2);

            if (exactMatch && score > 0) return null;
            return form;
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