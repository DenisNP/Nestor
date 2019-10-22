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

            var addresses = new string[]
            {
                @"C:\Users\Denis\Desktop\dict\0_20000_wiki_ruscorp.bin",
                @"C:\Users\Denis\Desktop\dict\20000_30000_wiki_ruscorp.bin",
                @"C:\Users\Denis\Desktop\dict\30000_40000_wiki_ruscorp.bin",
                @"C:\Users\Denis\Desktop\dict\40000_60000_wiki_ruscorp.bin",
                @"C:\Users\Denis\Desktop\dict\60000_90000_wiki_ruscorp.bin",
            };
            
            var db = new DawgBuilder<Record>();
            
            foreach (var address in addresses)
            {
                var sDawg = Dawg<Record>.Load(File.OpenRead(address), 
                    reader => new Record(reader.ReadString()));

                foreach (var (key, value) in sDawg)
                {
                    db.Insert(key, value);
                }
            }

            Console.Write("Building...");
            var d = db.BuildDawg();
            Console.WriteLine(d.GetNodeCount());
            Console.Write("Saving...");
            using (var create = File.Create("model_large.bin"))
            {
                d.SaveTo(create, Record.Write);
            }
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
                var n = GetNeighbours(s, depth);
                if (n != null)
                {
                    cloud.AddRange(n);
                }
            }

            return cloud;
        }

        private List<string> GetNeighbours(string word, int level)
        {
            var record = GetRecord(word);
            if (record == null) return null;

            var best = record.Best.Select(x => x.Value).ToList();
            var result = new List<string>(best);
            if (level > 1)
            {
                foreach (var n in best)
                {
                    var subN = GetNeighbours(n, level - 1);
                    if (subN != null)
                    {
                        result.AddRange(subN);
                    }
                }
            }

            return result;
        }
        
        private Stream LoadFile(string name)
        {
            try
            {
                var assembly = Assembly.GetCallingAssembly();
                var file = assembly.GetManifestResourceStream("Nestor." + name);
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

        public Record()
        {
            
        }

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