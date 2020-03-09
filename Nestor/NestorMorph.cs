using System;
using System.Collections.Generic;
using DawgSharp;
using Nestor.Data;
using Nestor.Models;

namespace Nestor
{
    public partial class NestorMorph
    {
        private Dawg<Word> _dawgSingle;
        private Dawg<Word[]> _dawgMulti;
        private static readonly HashSet<string> Prepositions = new HashSet<string>();
        private static readonly Storage Storage = new Storage();
        private static readonly List<ushort[]> Paradigms = new List<ushort[]>();

        public NestorMorph()
        {
            LoadAdditional();
            LoadParadigms();
            LoadMorphology();
            GC.Collect();

            Console.ReadKey();
        }
    }
}