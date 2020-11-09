namespace Nestor.Data
{
    public class TagMapper
    {
        public Pos GetPos(string tag)
        {
            return tag switch
            {
                "сущ" => Pos.Noun,
                "прл" => Pos.Adjective,
                "гл" => Pos.Verb,
                "нар" => Pos.Adverb,
                "прч" => Pos.Participle,
                "дееп" => Pos.Transgressive,
                "мест" => Pos.Pronoun,
                "предл" => Pos.Preposition,
                "союз" => Pos.Conjunction,
                "част" => Pos.Particle,
                "межд" => Pos.Interjection,
                "предик" => Pos.Predicative,
                _ => Pos.None
            };
        }

        public Gender GetGender(string tag)
        {
            return tag switch
            {
                "муж" => Gender.Masculine,
                "жен" => Gender.Feminine,
                "ср" => Gender.Neuter,
                "общ" => Gender.Common,
                _ => Gender.None
            };
        }

        public Number GetNumber(string tag)
        {
            return tag switch
            {
                "ед" => Number.Singular,
                "мн" => Number.Plural,
                _ => Number.None
            };
        }

        public Case GetCase(string tag)
        {
            return tag switch
            {
                "им" => Case.Nominative,
                "род" => Case.Genitive,
                "дат" => Case.Dative,
                "вин" => Case.Accusative,
                "тв" => Case.Instrumental,
                "пр" => Case.Prepositional,
                "мест" => Case.Locative,
                "парт" => Case.Partitive,
                "зват" => Case.Vocative,
                _ => Case.None
            };
        }

        public Tense GetTense(string tag)
        {
            return tag switch
            {
                "прош" => Tense.Past,
                "наст" => Tense.Present,
                "буд" => Tense.Future,
                "инф" => Tense.Infinitive,
                _ => Tense.None
            };
        }

        public Person GetPerson(string tag)
        {
            return tag switch
            {
                "1-е" => Person.First,
                "2-е" => Person.Second,
                "3-е" => Person.Third,
                _ => Person.None
            };
        }
    }
}