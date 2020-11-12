using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nestor.Nyms
{
    public class NestorNyms
    {
        private readonly Dictionary<string, HashSet<string>> _synonyms = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _antonyms = new Dictionary<string, HashSet<string>>();
        
        public NestorNyms()
        {
            LoadFileTo("Dict/synonyms.txt", _synonyms);
            LoadFileTo("Dict/antonyms.txt", _antonyms);
            GC.Collect();
        }

        private void LoadFileTo(string fileName, Dictionary<string, HashSet<string>> dictionary)
        {
            Console.Write($"Loading {fileName}... ");
            
            foreach (var line in File.ReadLines(fileName))
            {
                if (string.IsNullOrEmpty(line)) continue;

                var data = line.Split(";");
                if (data.Length != 2) continue;

                var word = data[0];
                var otherWords = data[1].Split("|").ToHashSet();
                
                dictionary.Add(word, otherWords);
            }

            Console.WriteLine($"done, total records: {dictionary.Count}");
        }
    }
}