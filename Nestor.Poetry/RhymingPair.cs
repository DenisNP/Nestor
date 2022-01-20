using System.Globalization;

namespace Nestor.Poetry
{
    public class RhymingPair
    {
        public WordWithStress FirstWord { get; }
        public WordWithStress SecondWord { get; }
        public double Score { get; }

        public RhymingPair(WordWithStress firstWord, WordWithStress secondWord, double score)
        {
            FirstWord = firstWord;
            SecondWord = secondWord;
            Score = score;
        }

        public override string ToString()
        {
            return $"{FirstWord}-{SecondWord} = {Score.ToString("0.000", CultureInfo.InvariantCulture)}";
        }
    }
}