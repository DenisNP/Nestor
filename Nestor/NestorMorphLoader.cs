using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DawgSharp;
using Nestor.Models;

namespace Nestor
{
    public partial class NestorMorph
    {
        private void LoadAdditional()
        {
            Console.WriteLine("Nestor is loading additional data...");
            
            // prepositions
            var prepositions = new List<string>();
            Utils.LoadFileToList(prepositions, "prepositions.txt");
            prepositions.ForEach(p => _prepositions.Add(p));

            Console.WriteLine($"...prepositions: {_prepositions.Count}");
            
            // storage
            Utils.LoadFileToList(_storage.GetPrefixes(), "prefixes.txt");
            Console.WriteLine($"...prefixes: {_storage.GetPrefixes().Count}");
            
            Utils.LoadFileToList(_storage.GetSuffixes(), "suffixes.txt");
            Console.WriteLine($"...suffixes: {_storage.GetSuffixes().Count}");
            
            Utils.LoadFileToList(_storage.GetGrammemes(), "grammemes.txt");
            Console.WriteLine($"...grammemes: {_storage.GetGrammemes().Count}");

            var tagsRaw = new List<string>();
            Utils.LoadFileToList(tagsRaw, "tags.txt");
            foreach (string tagRaw in tagsRaw)
            {
                _storage.GetTags().Add(tagRaw.Split(" ").Select(byte.Parse).ToArray());
            }
            Console.WriteLine($"...tags: {_storage.GetTags().Count}");
            
            _storage.ParseGrammemes();
        }

        private void LoadParadigms()
        {
            // paradigms
            var paradigmsRaw = new List<string>();
            Utils.LoadFileToList(paradigmsRaw, "paradigms.txt");

            foreach (string paradigm in paradigmsRaw)
            {
                _paradigms.Add(paradigm.Split(" ").Select(ushort.Parse).ToArray());
            }

            Console.WriteLine($"...paradigms: {_paradigms.Count}");
        }

        private void LoadWords()
        {
            var wordsRaw = new List<string>();
            Utils.LoadFileToList(wordsRaw, "words.txt");

            foreach (string wordRaw in wordsRaw)
            {
                _storage.GetWords().Add(new WordRaw(wordRaw));
            }

            Console.WriteLine($"...words: {_storage.GetWords().Count}");
        }

        private void LoadMorphology()
        {
            Console.Write("Nestor is loading morphology...");

            using Stream singleDict = Utils.LoadFile("dict_single.bin");
            _dawgSingle = Dawg<int>.Load(singleDict, reader => reader.ReadInt32());
            
            using Stream multiDict = Utils.LoadFile("dict_multiple.bin");
            _dawgMulti = Dawg<int[]>.Load(multiDict,
                reader =>
                {
                    string str = reader.ReadString();
                    return str.Split(" ").Select(int.Parse).ToArray();
                });
            
            Console.WriteLine("Ok");
        }
    }
}