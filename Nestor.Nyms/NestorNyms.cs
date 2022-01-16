using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nestor.Nyms
{
    public class NestorNyms
    {
        private readonly Dictionary<string, HashSet<string>> _synonyms = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _antonyms = new Dictionary<string, HashSet<string>>();
        
        public NestorNyms()
        {
            LoadFileTo("synonyms.txt", _synonyms);
            LoadFileTo("antonyms.txt", _antonyms);
            GC.Collect();
        }

        private void LoadFileTo(string fileName, Dictionary<string, HashSet<string>> dictionary)
        {
            Console.Write($"Loading {fileName}... ");

            using StreamReader reader = LoadFile(fileName);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] data = line.Split(";");
                    if (data.Length != 2) continue;

                    string word = data[0];
                    HashSet<string> otherWords = data[1].Split("|").ToHashSet();
                
                    dictionary.Add(word, otherWords);
                }
            }

            Console.WriteLine($"done, total records: {dictionary.Count}");
        }

        private StreamReader LoadFile(string fileName)
        {
            var assembly = Assembly.GetCallingAssembly();
            Stream file = assembly.GetManifestResourceStream($"Nestor.Nyms.Dict.{fileName}");
            if (file == null)
            {
                throw new IOException($"Cannot load file {fileName}");
            }
            return new StreamReader(file, Encoding.UTF8);
        }

        public HashSet<string> Synonyms(string word)
        {
            return _synonyms.GetValueOrDefault(word);
        }

        public HashSet<string> Antonyms(string word)
        {
            return _antonyms.GetValueOrDefault(word);
        }

        public bool AreSynonyms(string first, string second)
        {
            HashSet<string> f = Synonyms(first);
            if (f?.Contains(second) == true) return true;
            HashSet<string> s = Synonyms(second);
            return s?.Contains(first) == true;
        }

        public bool AreAntonyms(string first, string second)
        {
            HashSet<string> f = Antonyms(first);
            if (f?.Contains(second) == true) return true;
            HashSet<string> s = Antonyms(second);
            return s?.Contains(first) == true;
        }
    }
}