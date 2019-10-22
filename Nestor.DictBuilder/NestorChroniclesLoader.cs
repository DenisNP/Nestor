using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Nestor.DictBuilder
{
    public class NestorChroniclesLoader
    {
        private readonly NestorMorph _nestor = new NestorMorph();
        
        public void BuildDictionary(string inputFileName, string outputFileName)
        {
            var vocabulary = new Dictionary<string, double[]>();
            using (var zip = ZipFile.Open(inputFileName + ".zip", ZipArchiveMode.Read))
            {
                Console.WriteLine("Unzipping file...");
                var entry = zip.GetEntry(inputFileName + ".txt");
                if (entry == null)
                {
                    throw new IOException();
                }

                Console.WriteLine("Loading file: " + entry.Name);
                var count = 0;

                using (var reader = new StreamReader(entry.Open()))
                {
                    while (!reader.EndOfStream)
                    {
                        var lineRaw = reader.ReadLine();
                        if (lineRaw == null) continue;

                        count += ReadLine(lineRaw.ToLower().Trim(), vocabulary);
                    }
                }
                
                Console.WriteLine("Entries added: " + count);
            }

            var list = vocabulary
                .Select(
                    v => v.Key + " " + string.Join(
                             " ",
                             v.Value.Select(d => d.ToString("0.0000", CultureInfo.InvariantCulture)
                             )
                         )
                );
            var orderedEnumerable = list.OrderBy(l => l);
            File.WriteAllLines(inputFileName + "_new.txt", orderedEnumerable);
        }

        private int ReadLine(string line, Dictionary<string, double[]> vocabulary)
        {
            var data = line.Split(" ");
            var word = data[0];

            if (word.Contains("::")) return 0;
            var wordParts = word.Split("_");
            if (wordParts.Length != 2) return 0;

            var wordCore = wordParts[0];
            // var wordPos = wordParts[1];

            var lemmas = _nestor.GetLemmas(wordCore);
            if (lemmas == null) return 0;

            var values = data.Skip(1).Select(v => double.Parse(v, CultureInfo.InvariantCulture)).ToArray();
            if (values.Length != 300) return 0;

            lemmas = lemmas.Concat(new List<string> {wordCore}).ToArray();
            var count = 0;
            foreach (var lemma in lemmas)
            {
                if (!vocabulary.ContainsKey(lemma))
                {
                    vocabulary.Add(lemma, values);
                    count++;
                }
            }

            return count;
        }
    }
}