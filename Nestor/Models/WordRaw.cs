using System.Collections.Generic;
using Nestor.Data;

namespace Nestor.Models
{
    public struct WordRaw
    {
        public string Stem { get; set; }
        public short ParadigmId { get; set; }

        public WordRaw(string rawString)
        {
            var data = rawString.Split("|");
            Stem = data[0];
            ParadigmId = short.Parse(data[1]);
        }

        public override string ToString()
        {
            return $"{Stem}|{ParadigmId}";
        }
    }
}