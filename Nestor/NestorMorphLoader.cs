using System;
using System.Collections.Generic;
using System.Linq;
using DawgSharp;
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

            _dawgSingle = Dawg<Word>.Load(Utils.LoadFile("dict_single.bin"),
                reader =>
                {
                    var str = reader.ReadString();
                    return new Word(str);
                });
            
            _dawgMulti = Dawg<Word[]>.Load(Utils.LoadFile("dict_multiple.bin"),
                reader =>
                {
                    var str = reader.ReadString();
                    return str.Split("|").Select(x => new Word(x)).ToArray();
                });
            
            Console.WriteLine("Ok");
        }
    }
}