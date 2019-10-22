using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Nestor.Chronicles
{
    public class NestorChronicles
    {
        
    }

    public class Record
    {
        public List<Word> Best { get; set; }
        public List<Word> Worst { get; set; }

        public Record()
        {
            
        }

        public Record(string raw)
        {
            var parts = raw.Split("!");
            Best = parts[0].Split("|").Select(x => new Word(x)).ToList();
            Best = parts[1].Split("|").Select(x => new Word(x)).ToList();
        }
        
        public static void Write(BinaryWriter writer, Record record)
        {
            writer.Write(record.ToString());
        }

        public override string ToString()
        {
            return string.Join("|", Best.Select(x => x.ToString())) + "!" +
                   string.Join("|", Worst.Select(x => x.ToString()));
        }
    }

    public class Word
    {
        public string Value { get; set; }
        public double Distance { get; set; }

        public Word()
        {
            
        }

        public Word(string raw)
        {
            var data = raw.Split(";");
            Value = data[0];
            Distance = double.Parse(data[1], CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return Value + ";" + Distance.ToString("0.00", CultureInfo.InvariantCulture);
        }
    }
}