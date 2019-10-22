using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using DawgSharp;

namespace Nestor.Chronicles
{

    public class NestorChronicles
    {
        private Dawg<Record> _dawg;
        
        public NestorChronicles()
        {
            Console.Write("Nestor loading Chronicles...");

            _dawg = Dawg<Record>.Load(LoadFile("model_large.bin"), 
                reader => new Record(reader.ReadString()));
            
            Console.WriteLine("Ok");
        }

        public Record GetRecord(string w)
        {
            return _dawg[w];
        }

        public List<string> Cloud(IEnumerable<string> input, int depth)
        {
            var collection = input.ToList();
            var cloud = new List<string>(collection);
            foreach (var s in collection)
            {
                var n = Neighbours(s, depth);
                if (n != null)
                {
                    cloud.AddRange(n);
                }
            }

            return cloud;
        }

        public List<string> Neighbours(string word, int level)
        {
            var record = GetRecord(word);
            if (record == null) return null;

            var best = record.Best.Select(x => x.Value).ToList();
            var result = new List<string>(best);
            if (level > 1)
            {
                foreach (var n in best)
                {
                    var subN = Neighbours(n, level - 1);
                    if (subN != null)
                    {
                        result.AddRange(subN);
                    }
                }
            }

            return new List<string>(new HashSet<string>(result));
        }
        
        private Stream LoadFile(string name)
        {
            try
            {
                var assembly = Assembly.GetCallingAssembly();
                var file = assembly.GetManifestResourceStream("Nestor.Chronicles." + name);
                if (file != null)
                {
                    return file;
                }
            }
            catch (Exception _)
            {
                // ignored
            }

            return File.OpenRead(name);
        }
    }
    
    public class Record
    {
        public List<Word> Best { get; set; }
        public List<Word> Worst { get; set; }

        public Record() { }

        public Record(string raw)
        {
            var parts = raw.Split("!");
            Best = parts[0].Split("|").Select(x => new Word(x)).ToList();
            Worst = parts[1].Split("|").Select(x => new Word(x)).ToList();
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

        public Word() { }

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