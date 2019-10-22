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
            var calculated = new Dictionary<string, Dictionary<string, double>>();

            foreach (var word in vocabulary.Keys)
            {
                var best = new List<Word>();
                var worst = new List<Word>();
                foreach (var secondWord in vocabulary.Keys)
                {
                    if (word != secondWord)
                    {
                        double distance;
                        if (calculated.ContainsKey(secondWord) && calculated[secondWord].ContainsKey(word))
                        {
                            distance = calculated[secondWord][word];
                        }
                        else
                        {
                            distance = ScalarMultiply(vocabulary[word], vocabulary[secondWord]);
                            if (!calculated.ContainsKey(word))
                            {
                                calculated.Add(word, new Dictionary<string, double>());
                            }
                            calculated[word].Add(secondWord, distance);
                        }
                        
                        if (best.Count < 20 || best.Last().Distance < distance)
                        {
                            best.Add(new Word
                            {
                                Value = secondWord,
                                Distance = ScalarMultiply(vocabulary[word], vocabulary[secondWord])
                            });
                            best = best.OrderByDescending(x => x.Distance).Take(20).ToList();
                        }
                        
                        if (worst.Count < 20 || worst.Last().Distance > distance)
                        {
                            worst.Add(new Word
                            {
                                Value = secondWord,
                                Distance = ScalarMultiply(vocabulary[word], vocabulary[secondWord])
                            });
                            worst = worst.OrderBy(x => x.Distance).Take(20).ToList();
                        }
                    }
                }

                var record = new Record
                {
                    Best = best,
                    Worst = worst
                };
                
                _dawgBuilder.Insert(word, record);
                count++;

                if (count % 10 == 0)
                {
                    Console.WriteLine("..." + count + " done");
                    Console.WriteLine(word + ": " + record + "\n");
                }

                if (count >= 1000)
                {
                    break;
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