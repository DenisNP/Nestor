namespace Nestor.Poetry
{
    public class PoeticSyllable
    {
        public PoeticSyllableType Type { get; set; }
        public string Text { get; set; }
    }
    
    public enum PoeticSyllableType
    {
        Consonant,
        Stressed,
        Unstressed
    }

    public enum StressType
    {
        StrictlyUnstressed,
        CanBeStressed,
        StrictlyStressed,
    }
}