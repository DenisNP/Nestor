using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DawgSharp;
using Nestor.Models;

namespace Nestor.DictBuilder
{
    public class NestorLoader
    {
        private readonly DawgBuilder<Word[]> _dawgBuilder = new DawgBuilder<Word[]> ();
        private readonly List<Paradigm> _paradigms = new List<Paradigm>();
        private readonly Dictionary<string, int> _paradigmsByHash = new Dictionary<string, int>();
        private readonly HashedStorage _storage = new HashedStorage();

        private volatile int _numberOfThreads = 0;
        
        /// <summary>
        /// Main build method
        /// </summary>
        /// <param name="inputFileName">File of .zip dictionary archive without extension</param>
        public void BuildDictionary(string inputFileName)
        {
            using var zip = ZipFile.Open(inputFileName + ".zip", ZipArchiveMode.Read);
            
            Console.WriteLine("Unzipping file...");
            
            // unzip file
            var entry = zip.GetEntry(inputFileName + ".txt");
            if (entry == null)
            {
                throw new IOException($"Cannot load file: {inputFileName}");
            }
                
            Console.WriteLine("Loading file: " + entry.Name);
            var count = 0;

            // read file line by line
            using var reader = new StreamReader(entry.Open());
            var lines = new List<string>();
            while (!reader.EndOfStream)
            {
                var lineRaw = reader.ReadLine();
                if (lineRaw == null) continue;

                var line = lineRaw.Trim();
                if (line == "")
                {
                    // if line is empty, a new word just started, to save previous one and clean its lines
                    WriteWord(lines);
                    lines.Clear();
                }
                else
                {
                    // if line is not empty, just add it to previous lines
                    lines.Add(line.ToLower());
                    count++;
                    
                    // log progress
                    if (count % 5000 == 0)
                    {
                        Console.WriteLine(
                            $"Lines loaded: {count}, " +
                            $"paradigms: {_paradigms.Count}, " +
                            $"prefixes: {_storage.GetPrefixes().Count}, " +
                            $"suffixes: {_storage.GetSuffixes().Count}, " +
                            $"tagGroups: {_storage.GetTagGroups().Count}"
                        );
                    }
                }
            }

            while (_numberOfThreads > 0)
            {
                Thread.Sleep(1000);
            }
            
            // write finish lines
            WriteWord(lines);

            Console.WriteLine("Total lines: " + count);
            Console.Write("Building DAWG...");
            
            // build dawg dictionary
            var dawg = _dawgBuilder.BuildDawg();
            Console.WriteLine("Done. Nodes count: " + dawg.GetNodeCount() + ".");

            Console.Write("Saving...");
            
            // save it to file
            using var saveStream = File.Create("dict.bin");
            dawg.SaveTo(
                saveStream,
                (writer, words) => writer.Write(
                        string.Join("|", words.Select(w => w.ToString())
                    )
                )
            );
            Console.WriteLine("Ok.");
            
            // save storage
            Utils.SaveListToFile(_storage.GetPrefixes(), "prefixes.txt");
            Utils.SaveListToFile(_storage.GetSuffixes(), "suffixes.txt");
            Utils.SaveListToFile(_storage.GetTags(), "tags.txt");
            Utils.SaveListToFile(_storage.GetTagGroups(), "tag_groups.txt");
            
            // save paradigms
            Utils.SaveListToFile(_paradigms, "paradigms.txt");
        }

        /// <summary>
        /// Write single word to dawg
        /// </summary>
        /// <param name="lines">Word morphology lines from dict</param>
        private void WriteWord(List<string> lines)
        {
            if (lines.Count == 0) return;
            var lemmaLine = lines.First().Split("|");
            if (
                lemmaLine.Length == 0
                || lemmaLine[0].Trim().Contains(" ")
                || Regex.Match(lemmaLine[0], "[a-z]+").Success
            )
            {
                // dont load empty words, words with space and words with english letters
                return;
            }
            
            var (paradigm, stem) = ParadigmGenerator.Generate(lines, _storage, out var altForms);
            
            int paradigmId;

            // find if there is this kind of paradigm already
            var hash = paradigm.ToString();

            // assign paradigm identifier
            if (!_paradigmsByHash.ContainsKey(hash))
            {
                paradigmId = _paradigms.Count;
                _paradigms.Add(paradigm);
                _paradigmsByHash.Add(hash, paradigmId);
            }
            else
            {
                paradigmId = _paradigmsByHash[hash];
            }

            // write all words
            var word = new Word
            {
                Stem = stem,
                ParadigmId = paradigmId
            }.Load(_storage, _paradigms);
            
            var forms = word.GetAllForms().ToList();
            var altFormsToAdd = altForms.Where(af => af != "" && !forms.Contains(af));
            forms.AddRange(altFormsToAdd);

            foreach (var form in forms)
            {
                _dawgBuilder.TryGetValue(form, out var list);

                if (list == null)
                {
                    _dawgBuilder.Insert(form, new[] {word});
                }
                else
                {
                    if (list.Any(w => w.ToString() == word.ToString()))
                    {
                        continue;
                    }
                    
                    var insertArray = list.Append(word).ToArray();
                    _dawgBuilder.Insert(form, insertArray);
                }
            }
        }
    }
}