namespace Nestor.Poetry
{
    public record Trigram(string LeftConsonant, string Vowel, string RightConsonant)
    {
        public bool IsNull => string.IsNullOrEmpty(LeftConsonant)
                              && string.IsNullOrEmpty(Vowel)
                              && string.IsNullOrEmpty(RightConsonant);

        public override string ToString()
        {
            return $"Trigram[{LeftConsonant}-{Vowel}-{RightConsonant}]";
        }
    }
}