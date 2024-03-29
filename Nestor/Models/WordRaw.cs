namespace Nestor.Models
{
    public struct WordRaw
    {
        public string Stem { get; set; }
        public short ParadigmId { get; set; }

        internal WordRaw(string rawString)
        {
            string[] data = rawString.Split("|");
            Stem = data[0];
            ParadigmId = short.Parse(data[1]);
        }

        public override string ToString()
        {
            return $"{Stem}|{ParadigmId}";
        }
    }
}