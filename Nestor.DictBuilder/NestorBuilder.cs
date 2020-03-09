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
        private readonly Dictionary<string, Word[]> _dictionary = new Dictionary<string, Word[]>();
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
                            $"tagGroups: {_storage.GetTagGroups().Count}"
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
                "single-paradigma words",
                (writer, words) => writer.Write(words.First().ToString())
            );

            // clear stems if they are the same for all paradigms
            var multiWords = _dictionary.Where(x => x.Value.Length > 1).ToList();
            foreach (var mw in multiWords)
            {
                var allStems = mw.Value.Select(w => w.Stem).ToHashSet();
                if (allStems.Count == 1)
                {
                    for (var k = 1; k < mw.Value.Length; k++)
                    {
                        mw.Value[k] = new Word{ Stem = "", ParadigmId = mw.Value[k].ParadigmId };
                    }
                }
            }
            
            BuildSaveDawg(
                multiWords,
                "dict_multiple.bin",
                "multiple-paradigma words",
                (writer, words) => writer.Write(string.Join("|", words.Select(x => x.ToString())))
            );
            
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
            var hash = ParadigmHelper.ToString(paradigm);

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
                paradigm = _paradigms[paradigmId];
            }

            // write all words
            var word = new Word
            {
                Stem = stem,
                ParadigmId = (ushort)paradigmId
            };
            
            var forms = word.GetAllForms(paradigm, _storage).ToList();
            var altFormsToAdd = altForms.Where(af => af != "" && !forms.Contains(af));
            forms.AddRange(altFormsToAdd);

            foreach (var form in forms)
            {
                if (!_dictionary.ContainsKey(form))
                {
                    _dictionary.Add(form, new []{word});
                }
                else
                {
                    var oldList = _dictionary[form];
                    if (oldList.Any(w => w.Stem == word.Stem && w.ParadigmId == word.ParadigmId))
                    {
                        continue;
                    }
                    _dictionary[form] = oldList.Append(word).ToArray();
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