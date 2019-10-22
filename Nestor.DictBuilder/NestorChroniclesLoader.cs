using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using DawgSharp;
using Nestor.Chronicles;

namespace Nestor.DictBuilder
{
    public class NestorChroniclesLoader
    {
        private readonly NestorMorph _nestor = new NestorMorph();
        private readonly DawgBuilder<Chronicles.Record> _dawgBuilder = new DawgBuilder<Chronicles.Record> ();
        
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

            Calculate(vocabulary);
            
            Console.Write("Building DAWG... ");
            var dawg = _dawgBuilder.BuildDawg();
            
            Console.WriteLine("Ok, nodes: " + dawg.GetNodeCount());
            Console.WriteLine("Save DAWG");
            
            using (var writeToFile = File.Create(outputFileName))
            {
                dawg.SaveTo(writeToFile, Record.Write);
            }

            Console.WriteLine("Ok");
        }

        private void Calculate(Dictionary<string, double[]> vocabulary)
        {
            Console.WriteLine("Calculating...");
            var count = 0;

            foreach (var word in vocabulary.Keys)
            {
                var list = new List<Word>();
                foreach (var secondWord in vocabulary.Keys)
                {
                    if (word != secondWord)
                    {
                        list.Add(new Word
                        {
                            Value = secondWord,
                            Distance = ScalarMultiply(vocabulary[word], vocabulary[secondWord])
                        });
                    }
                }

                var orderedList = list.OrderByDescending(x => x.Distance).ToList();
                var record = new Record
                {
                    Best = orderedList.Take(10).ToList(),
                    Worst = orderedList.TakeLast(10).OrderBy(x => x.Distance).ToList()
                };
                
                _dawgBuilder.Insert(word, record);
                count++;

                if (count % 100 == 0)
                {
                    Console.WriteLine("..." + count + " done");
                    Console.WriteLine(word + ": " + record + "\n");
                }
            }
        }

        private int ReadLine(string line, Dictionary<string, double[]> vocabulary)
        {
            var data = line.Split(" ");
            var wordCore = data[0];

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

        private double ScalarMultiply(double[] vec1, double[] vec2)
        {
            return vec1.Select((t, i) => t * vec2[i]).Sum();
        }
    }
}