using Nestor.Data;

namespace Nestor.Models
{
    public class Grammatics
    {
        public Pos Pos { get; } = Pos.None;
        public Gender Gender { get; internal set; } = Gender.None;
        public Number Number { get; } = Number.None;
        public Case Case { get; } = Case.None;

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
            }
        }

        public Grammatics()
        {
            
        }

        public int DifferenceFrom(Gender gender, Case @case, Number number, bool ignoreNotDefined = true)
        {
            var dist = 0;
            if ((gender != Gender.None || !ignoreNotDefined) && gender != Gender)
                dist++;

            if ((@case != Case.None || !ignoreNotDefined) && @case != Case)
                dist++;

            if ((number != Number.None || !ignoreNotDefined) && number != Number)
                dist++;

            return dist;
        }
    }
}