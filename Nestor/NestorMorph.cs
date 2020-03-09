using System;
using System.Collections.Generic;
using System.Linq;
using DawgSharp;
using Nestor.Data;
using Nestor.Models;

namespace Nestor
{
    public partial class NestorMorph
    {
        private Dawg<int> _dawgSingle;
        private Dawg<int[]> _dawgMulti;
        private static readonly HashSet<string> Prepositions = new HashSet<string>();
        private static readonly Storage Storage = new Storage();
        private static readonly List<ushort[]> Paradigms = new List<ushort[]>();

        public NestorMorph()
        {
            LoadAdditional();
            LoadParadigms();
            LoadWords();
            LoadMorphology();
            GC.Collect();
        }

        public Word[] Parse(string form)
        {
            int[] found = null;
            var single = _dawgSingle[form];
            if (single == 0)
            {
                var multiple = _dawgMulti[form];
                if (multiple != null)
                {
                    found = multiple;
                }
            }
            else
            {
                found = new[] {single};
            }

            // word not found, return default with its initial form
            if (found == null)
            {
                var raw = new WordRaw
                {
                    Stem = form,
                    ParadigmId = 0
                };
                return new []{ new Word(raw, Storage, Paradigms) };
            }

            return found.Select(WordById).ToArray();
        }

        private Word WordById(int id)
        {
            var wordRaw = Storage.GetWord(id);
            return new Word(wordRaw, Storage, Paradigms);
        }
    }
}