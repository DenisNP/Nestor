using Nestor.Data;

namespace Nestor.Models
{
    public class Tag
    {
        public Pos Pos { get; } = Pos.None;
        public Gender Gender { get; internal set; } = Gender.None;
        public Number Number { get; } = Number.None;
        public Case Case { get; } = Case.None;
        public Tense Tense { get; } = Tense.None;
        public Person Person { get; } = Person.None;

        internal Tag(string[] grammemes, Storage storage)
        {
            if (grammemes.Length == 0) return;
            
            // try to determine pos
            Pos = storage.PosByGrammeme(grammemes[0]);
            
            // other
            for (var i = 1; i < grammemes.Length; i++)
            {
                string tag = grammemes[i];
                if (Gender == Gender.None) Gender = storage.GenderByGrammeme(tag);
                if (Number == Number.None) Number = storage.NumberByGrammeme(tag);
                if (Case == Case.None) Case = storage.CaseByGrammeme(tag);
                if (Tense == Tense.None) Tense = storage.TenseByGrammeme(tag);
                if (Person == Person.None) Person = storage.PersonByGrammeme(tag);
            }
        }

        public Tag()
        {
            
        }

        public int DifferenceFrom(Gender gender, Case @case, Number number, Tense tense, Person person, bool ignoreNotDefined = false)
        {
            var dist = 0;
            
            if (gender != Gender.None && (Gender != Gender.None || !ignoreNotDefined) && gender != Gender) dist++;
            if (@case != Case.None && (Case != Case.None || !ignoreNotDefined) && @case != Case) dist++;
            if (number != Number.None && (Number != Number.None || !ignoreNotDefined) && number != Number) dist++;
            if (tense != Tense.None && (Tense != Tense.None || !ignoreNotDefined) && tense != Tense) dist++;
            if (person != Person.None && (Person != Person.None || !ignoreNotDefined) && person != Person) dist++;
            
            return dist;
        }
    }
}