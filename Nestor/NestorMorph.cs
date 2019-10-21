using System;
using System.IO;
using System.Linq;
using DawgSharp;

namespace Nestor
{
    public class NestorMorph
    {
        private Dawg<(string, string)> _dawg;

        public NestorMorph()
        {
            Console.Write("Nestor loading data...");
            _dawg = Dawg<(string, string)>.Load(File.OpenRead("dict.bin"), reader =>
            {
                var str = reader.ReadString();
                var data = str.Split("|");
                return (data[0], data[1]);
            });
            Console.WriteLine("Ok");
        }

        public bool HasOneOfLemmas(string inputWord, params string[] lemmas)
        {
            return lemmas.Any(l => HasLemma(inputWord, l));
        }

        public bool HasLemma(string inputWord, string lemma)
        {
            var word = inputWord.ToLower().Trim();
            if (word == lemma) return true;
            var lemmas = _dawg[word];

            return lemmas.Item1 == lemma || lemmas.Item2 == lemma;
        }
    }
}