using Nestor.Models;

namespace Nestor.Poetry
{
    public class RhymingPair
    {
        public WordForm FirstWord { get; set; }
        public WordForm SecondWord { get; set; }
        public double Score { get; set; }
    }
}