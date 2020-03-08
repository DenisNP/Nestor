using System.Collections.Generic;
using Nestor.Data;

namespace Nestor.Models
{
    public class Word
    {
        public string Stem { get; set; }
        public ushort ParadigmId { get; set; }
        
        private IStorage _storage;
        private Paradigm _paradigm;

        public Word()
        {
            
        }

        public Word(string rawString)
        {
            var data = rawString.Split(";");
            Stem = data[0];
            ParadigmId = ushort.Parse(data[1]);
        }

        public Word Load(IStorage storage, List<Paradigm> paradigms)
        {
            _storage = storage;
            _paradigm = paradigms[ParadigmId];

            return this;
        }
        
        public string[] GetAllForms()
        {
            var forms = new string[_paradigm.Rules.Length];
            for (var i = 0; i < _paradigm.Rules.Length; i++)
            {
                forms[i] = GetForm(i);
            }

            return forms;
        }

        public string GetForm(int idx)
        {
            return $"{_storage.GetPrefix(_paradigm.Rules[idx].Prefix)}{Stem}{_storage.GetSuffix(_paradigm.Rules[idx].Suffix)}";
        }

        public string Lemma()
        {
            return GetForm(0);
        }

        public override string ToString()
        {
            return $"{Stem};{ParadigmId}";
        }
    }
}