using System;
using System.Collections.Generic;
using DawgSharp;
using Nestor.Models;

namespace Nestor
{
    public partial class NestorMorph
    {
        private Dawg<Word[]> _dawg;
        private static readonly HashSet<string> Prepositions = new HashSet<string>();
        private static readonly Storage Storage = new Storage();
        private static readonly List<Paradigm> Paradigms = new List<Paradigm>();

        public NestorMorph()
        {
            LoadMorphology();
            LoadAdditional();
            LoadParadigms();
            GC.Collect();

            Console.ReadKey();
        }
    }
}