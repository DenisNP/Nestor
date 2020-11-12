using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using DawgSharp;
using Nestor.Data;
using Nestor.Models;

namespace Nestor.DictBuilder
{
    public class NestorBuilder
    {
        private readonly Dictionary<string, int[]> _dictionary = new Dictionary<string, int[]>();
        private readonly List<ushort[]> _paradigms = new List<ushort[]>();
        private readonly Dictionary<string, int> _paradigmsByHash = new Dictionary<string, int>();
        private readonly HashedStorage _storage = new HashedStorage();
        
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
                            $"tags: {_storage.GetTags().Count}"
                        );
                    }
                }
            }
            
            // write finish lines
            WriteWord(lines);

            Console.WriteLine("Total lines: " + count);
            
            // build dawg dictionary
            BuildSaveDawg(
                _dictionary.Where(x => x.Value.Length == 1),
                "dict_single.bin",
                "single-paradigm words",
                (writer, wordIds) => writer.Write(wordIds[0])
            );
            
            BuildSaveDawg(
                _dictionary.Where(x => x.Value.Length > 1).ToList(),
                "dict_multiple.bin",
                "multiple-paradigm words",
                (writer, wordIds) => writer.Write(string.Join(" ", wordIds))
            );
            
            // save storage
            Utils.SaveListToFile(_storage.GetPrefixes(), "prefixes.txt");
            Utils.SaveListToFile(_storage.GetSuffixes(), "suffixes.txt");
            Utils.SaveListToFile(_storage.GetGrammemes(), "grammemes.txt");
            Utils.SaveListToFile(_storage.GetTags().Select(t => string.Join(" ", t)).ToList(), "tags.txt");
            Utils.SaveListToFile(_storage.GetWords().Select(x => x.ToString()).ToList(), "words.txt");
            
            // save paradigms
            Utils.SaveListToFile(_paradigms.Select(ParadigmHelper.ToString).ToList(), "paradigms.txt");
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

            // assign paradigm identifier
            var paradigmId = Utils.ComplexAdd(_paradigmsByHash, _paradigms, paradigm, ParadigmHelper.ToString) + 1;
            paradigm = _paradigms[paradigmId - 1];

            // store new word (stem + paradigm)
            var word = new WordRaw
            {
                Stem = stem,
                ParadigmId = (short)paradigmId
            };
            var wordId = _storage.AddWord(word);
            
            // write this word id for all forms
            var forms = Word.GetAllForms(paradigm, stem, _storage).ToList();
            var altFormsToAdd = altForms.Where(af => af != "" && !forms.Contains(af));
            forms.AddRange(altFormsToAdd);

            foreach (var form in forms)
            {
                if (!_dictionary.ContainsKey(form))
                {
                    _dictionary.Add(form, new[] {wordId});
                }
                else
                {
                    var oldList = _dictionary[form];
                    if (oldList.Contains(wordId))
                    {
                        continue;
                    }

                    _dictionary[form] = oldList.Append(wordId).ToArray();
                }
            }
        }

        private void BuildSaveDawg<T>(
            IEnumerable<KeyValuePair<string, T>> values,
            string fileName,
            string dawgName,
            Action<BinaryWriter, T> writePayload
        )
        {
            Console.WriteLine($"Writing DAWG {dawgName}...");

            var dawgBuilder = new DawgBuilder<T>();
            foreach (var (key, value) in values)
            {
                dawgBuilder.Insert(key, value);
            }
            
            Console.Write("Building DAWG...");
            
            var dawg = dawgBuilder.BuildDawg();
            
            Console.WriteLine("Done. Nodes count: " + dawg.GetNodeCount() + ".");
            Console.Write("Saving...");
            
            // save it to file
            using var saveStream = File.Create(fileName);
            dawg.SaveTo(saveStream, writePayload);
            
            Console.WriteLine("Ok.");
        } 
    }
}