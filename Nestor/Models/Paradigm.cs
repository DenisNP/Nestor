using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nestor.Models
{
    public class Paradigm
    {
        public string Stem { get; set; }
        public int[] Prefixes { get; set; }
        public int[] Suffixes { get; set; }
        public int[] Accents { get; set; }
        public int[][] Tags { get; set; }
        
        private readonly Storage _storage;
        
        public Paradigm(Storage storage)
        {
            _storage = storage;
        }

        public bool IsEqualTo(Paradigm other)
        {
            return ToString() == other.ToString();
        }

        public string[] GetAllForms()
        {
            var forms = new string[Prefixes.Length];
            for (var i = 0; i < Prefixes.Length; i++)
            {
                forms[i] = $"{_storage.GetPrefix(Prefixes[i])}{Stem}{_storage.GetSuffix(Suffixes[i])}";
            }

            return forms;
        }

        public override string ToString()
        {
            return $"{Stem}" +
                   $"|{Prefixes.Join(";")}" +
                   $"|{Suffixes.Join(";")}" +
                   $"|{Accents.Join(";")}" +
                   $"|{Tags.Select(tag => tag.Join(",")).Join(";")}";
        }
    }
}