using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using DawgSharp;
using Nestor.Data;
using Nestor.Models;

namespace Nestor
{
    public partial class NestorMorph
    {
        private void LoadAdditional()
        {
            Console.WriteLine("Nestor loading additional data...");
            
            // prepositions
            var prepositions = new List<string>();
            Utils.LoadFileToList(prepositions, "prepositions.txt");
            prepositions.ForEach(p => Prepositions.Add(p));

            Console.WriteLine($"...prepositions: {Prepositions.Count}");
            
            // storage
            Utils.LoadFileToList(Storage.GetPrefixes(), "prefixes.txt");
            Console.WriteLine($"...prefixes: {Storage.GetPrefixes().Count}");
            
            Utils.LoadFileToList(Storage.GetSuffixes(), "suffixes.txt");
            Console.WriteLine($"...suffixes: {Storage.GetSuffixes().Count}");
            
            Utils.LoadFileToList(Storage.GetTags(), "tags.txt");
            Console.WriteLine($"...tags: {Storage.GetTags().Count}");
            
            Utils.LoadFileToList(Storage.GetTagGroups(), "tag_groups.txt");
            Console.WriteLine($"...tag groups: {Storage.GetTagGroups().Count}");
        }

        private void LoadParadigms()
        {
            // paradigms
            var paradigmsRaw = new List<string>();
            Utils.LoadFileToList(paradigmsRaw, "paradigms.txt");

            foreach (var paradigm in paradigmsRaw)
            {
                Paradigms.Add(new Paradigm(paradigm));
            }

            Console.WriteLine($"...paradigms: {Paradigms.Count}");
        }

        private void LoadMorphology()
        {
            Console.Write("Nestor loading morphology...");

            _dawg = Dawg<Word[]>.Load(Utils.LoadFile("dict.bin"),
                reader =>
                {
                    var str = reader.ReadString();
                    return str.Split("|")
                        .Select(w => new Word(w).Load(Storage, Paradigms))
                        .ToArray();
                });
            
            Console.WriteLine("Ok");
        }
    }
}