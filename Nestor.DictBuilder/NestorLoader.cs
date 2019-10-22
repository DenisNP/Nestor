using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using DawgSharp;

namespace Nestor.DictBuilder
{
    public class NestorLoader
    {
        private DawgBuilder<List<string>> _dawgBuilder = new DawgBuilder<List<string>> ();
        
        public void BuildDictionary(string inputFileName, string outputFileName)
        {
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
                    var lines = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        var lineRaw = reader.ReadLine();
                        if (lineRaw == null) continue;

                        var line = lineRaw.Trim();
                        if (line == "")
                        {
                            FlushLines(lines);
                            count += lines.Count;
                            lines.Clear();
                        }
                        else
                        {
                            lines.Add(line);
                        }
                    }
                    FlushLines(lines);
                    count += lines.Count;
                }

                Console.WriteLine("Lines loaded: " + count);
                Console.Write("Building DAWG...");
                
                var dawg = _dawgBuilder.BuildDawg();
                Console.WriteLine("Done. Nodes count: " + dawg.GetNodeCount() + ".");

                Console.Write("Saving...");
                using (var saveStream = File.Create(outputFileName))
                {
                    dawg.SaveTo(
                        saveStream,
                        (writer, list) => writer.Write(string.Join("|", list))
                    );
                }
                Console.WriteLine("Ok.");
            }
        }

        private void FlushLines(List<string> lines)
        {
            if (lines.Count == 0) return;
            var lemmaLine = lines.First().Split("|");
            var lemma = Regex.Replace(lemmaLine[0].Trim(), "[^а-яё\\-]", "");
            var secondLemma = Regex.Replace(lemmaLine[2].Trim(), "[^а-яё\\-]", "");
            
            InsertToDawg(lemma, true, "");
            if (secondLemma != lemma)
            {
                InsertToDawg(secondLemma, true, "");
            }

            for (var i = 1; i < lines.Count; i++)
            {
                var cLine = lines[i].Split("|");
                var cWord = Regex.Replace(cLine[0].Trim(), "[^а-яё\\-]", "");
                var secondCWord = Regex.Replace(cLine[2].Trim(), "[^а-яё\\-]", "");

                InsertToDawg(cWord, false, lemma, secondLemma);
                if (cWord != secondCWord)
                {
                    InsertToDawg(secondCWord, false, lemma, secondLemma);
                }
            }
        }

        private void InsertToDawg(string key, bool forceEmpty, params string[] values)
        {
            var toAdd = values.Where(v => forceEmpty || v != "").ToList();
            if (toAdd.Count == 0) return;
            _dawgBuilder.TryGetValue(key, out var list);
            
            if (list == null)
            {
                var uniqueAdd = new HashSet<string>(toAdd);
                _dawgBuilder.Insert(key, new List<string>(uniqueAdd));
            }
            else
            {
                toAdd = toAdd.Where(ta => !list.Contains(ta)).ToList();
                if (toAdd.Count == 0) return;
                list.AddRange(toAdd);
                _dawgBuilder.Insert(key, list);
            }
        }
    }
}