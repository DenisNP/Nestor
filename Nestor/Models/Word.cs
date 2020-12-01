using System;
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

            var forms = new WordForm[paradigm.Length / 4];
            for (var i = 0; i < forms.Length; i++)
            {
                var tag = storage.GetTag(paradigm[forms.Length * 3 + i]);
                forms[i] = new WordForm
                (
                    GetForm(i, paradigm, Stem, storage),
                    paradigm[forms.Length * 2 + i],
                    tag.Select(g => storage.GetGrammeme(g)).ToArray(),
                    storage
                );
            }
          
            // there is unknown GENDER for plural nouns in dictionary, fix that
            if (forms[0].Tag.Pos == Pos.Noun)
            {
                foreach (var form in forms)
                {
                    form.Tag.Gender = forms[0].Tag.Gender;
                }
            }

            if (forms.Length > 1)
            {
                var lemma = forms[0];
                var formsList = forms.ToList();
                formsList.RemoveAt(0);
                
                // order
                int PosDifference(WordForm f)
                {
                    return lemma.Tag.Pos == f.Tag.Pos ? 0 : 1;
                }

                int EnumValue<T>(T @enum, T defaultValue = default) where T : Enum
                {
                    if (Equals(@enum, defaultValue)) return 100;
                    return (int)(object)@enum;
                }

                formsList = formsList
                    .OrderBy(PosDifference)
                    .ThenBy(f => EnumValue(f.Tag.Number))
                    .ThenBy(f => EnumValue(f.Tag.Gender))
                    .ThenBy(f => EnumValue(f.Tag.Person))
                    .ThenBy(f => EnumValue(f.Tag.Tense))
                    .ThenBy(f => EnumValue(f.Tag.Case))
                    .ToList();

                forms = formsList.Prepend(lemma).ToArray();
            }

            Forms = forms;
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