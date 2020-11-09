using Nestor.Data;

namespace Nestor.Models
{
    public class Grammatics
    {
        public Pos Pos { get; } = Pos.None;
        public Gender Gender { get; internal set; } = Gender.None;
        public Number Number { get; } = Number.None;
        public Case Case { get; } = Case.None;
        public Tense Tense { get; } = Tense.None;
        public Person Person { get; } = Person.None;

        public Grammatics(string[] tags, Storage storage)
        {
            if (tags.Length == 0) return;
            
            // try to determine pos
            Pos = storage.PosByTag(tags[0]);
            
            // other
            for (var i = 1; i < tags.Length; i++)
            {
                var tag = tags[i];
                if (Gender == Gender.None) Gender = storage.GenderByTag(tag);
                if (Number == Number.None) Number = storage.NumberByTag(tag);
                if (Case == Case.None) Case = storage.CaseByTag(tag);
                if (Tense == Tense.None) Tense = storage.TenseByTag(tag);
                if (Person == Person.None) Person = storage.PersonByTag(tag);
            }
        }

        public Grammatics()
        {
            
        }

        public int DifferenceFrom(Gender gender, Case @case, Number number, Tense tense, Person person, bool ignoreNotDefined = true)
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