using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using DawgSharp;
using Nestor.Models;

namespace Nestor.DictBuilder
{
    public class NestorLoader
    {
        private readonly DawgBuilder<int[]> _dawgBuilder = new DawgBuilder<int[]> ();
        private readonly List<Paradigm> _paradigms = new List<Paradigm>();
        private readonly Storage _storage = new Storage();
        
        /// <summary>
        /// Main build method
        /// </summary>
        /// <param name="inputFileName">File of .zip dictionary archive without extension</param>
        /// <param name="outputFileName">File to save ready dictionary</param>
        public void BuildDictionary(string inputFileName, string outputFileName)
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
                    count += lines.Count;
                    lines.Clear();
                    
                    // log progress
                    if (count % 1000 == 0)
                    {
                        Console.WriteLine("Lines loaded: " + count);
                    }
                }
                else
                {
                    // if line is not empty, just add it to previous lines
                    lines.Add(line);
                }
            }
            
            // write finish lines
            WriteWord(lines);
            count += lines.Count;

            Console.WriteLine("Total lines: " + count);
            Console.Write("Building DAWG...");
            
            // build dawg dictionary
            var dawg = _dawgBuilder.BuildDawg();
            Console.WriteLine("Done. Nodes count: " + dawg.GetNodeCount() + ".");

            Console.Write("Saving...");
            
            // save it to file
            using var saveStream = File.Create(outputFileName);
            dawg.SaveTo(saveStream, (writer, list) => writer.Write(string.Join("|", list)));
            Console.WriteLine("Ok.");
            
            // save storage
            Utils.SaveListToFile(_storage.GetPrefixes(), "prefixes.txt");
            Utils.SaveListToFile(_storage.GetSuffixes(), "suffixes.txt");
            Utils.SaveListToFile(_storage.GetTags(), "tags.txt");
            
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
                || lemmaLine[0].Contains(" ")
                || Regex.Match(lemmaLine[0], "[a-z]+").Success
            )
            {
                // dont load empty words, words with space and words with english letters
                return;
            }

            var paradigm = ParadigmGenerator.Generate(lines, _storage);
            
            // find if there is this kind of paradigm already
            var similar = _paradigms.FirstOrDefault(p => p.IsEqualTo(paradigm));
            
            // assign paradigm identifier
            int paradigmId;
            if (similar == null)
            {
                paradigmId = _paradigms.Count;
                _paradigms.Add(paradigm);
            }
            else
            {
                paradigmId = _paradigms.IndexOf(similar);
                paradigm = similar;
            }
            
            // write all words
            var forms = paradigm.GetAllForms();
            foreach (var word in forms)
            {
                _dawgBuilder.TryGetValue(word, out var list);
                _dawgBuilder.Insert(
                    word,
                    list == null
                        ? new[] {paradigmId}
                        : list.Append(paradigmId).ToArray()
                );
            }
        }
    }
}