using System.Collections.Generic;
using System.Linq;

namespace Nestor.Thesaurus
{
    public class NestorThesaurus
    {
        private static ThesaurusDatabase _database;
        private readonly NestorMorph _nestor;
        
        public NestorThesaurus(NestorMorph nestorMorph = null)
        {
            _database ??= new ThesaurusDatabase("SynsetDB");
            _nestor = nestorMorph ?? new NestorMorph();
        }
        
        /// <summary>
        /// Для каждого флага из relations нужно для lemma вернуть список объектов RelatedWord,
        /// в каждом таком объекте слово или словосочетание и тип взаимоотношения с исходным словом
        /// </summary>
        /// <param name="lemma"> Лемма </param>
        /// <param name="relations"> Флаги отношений через | </param>
        /// <returns>Массив синсетов соответствующих отношениям</returns>
        public RelatedWord[] GetStraightRelations(string lemma, WordRelation relations)
        {
            var result = new List<RelatedWord>();

            var senses = _database.Senses.Where(s => s.Lemma == lemma.ToUpper()).ToArray();
            var synsets = _database.GetSynsets(senses.Select(s=> s.SynsetId).ToArray());
            var synsetIds = synsets.Select(s => s.Id).ToArray();

            // if (relations.HasFlag(WordRelation.None))
            //     return result.ToArray();

            if (relations.HasFlag(WordRelation.Hyponym))
            {
                result.AddRange(_database.GetHyponyms(synsetIds).Select(h =>
                    ConstructRelatedWord(h.Title, WordRelation.Hyponym)));
            }

            if (relations.HasFlag(WordRelation.SameRoot))
            {   
                //возвращаю sense а не synset
                var derivatives = _database.GetDerivations(senses.Select(s=> s.Id).ToArray()); 

                result.AddRange(derivatives
                    .Select(derivative => 
                        ConstructRelatedWord(derivative.Name, WordRelation.SameRoot)));
            }

            if (relations.HasFlag(WordRelation.Synset))
            {
                result.AddRange(synsets.Select(s => 
                    ConstructRelatedWord(s.Title, WordRelation.Synset)));
            }

            if (relations.HasFlag(WordRelation.Hypernym))
            {
                result.AddRange(_database.GetHypernyms(synsetIds).Select(h =>
                    ConstructRelatedWord(h.Title, WordRelation.Hypernym)));
            }

            if (relations.HasFlag(WordRelation.Domain))
            {
                result.AddRange(_database.GetDomains(synsetIds).Select(d =>
                    ConstructRelatedWord(d.Title, WordRelation.Domain)));
            }

            if (relations.HasFlag(WordRelation.PartOfSpeechSynonym))
            {
                result.AddRange(_database.GetPosSynonyms(synsetIds).Select(p =>
                    ConstructRelatedWord(p.Title, WordRelation.PartOfSpeechSynonym)));
            }

            if (relations.HasFlag(WordRelation.Holonym))
            {
                result.AddRange(_database.GetHolonyms(synsetIds).Select(h =>
                    ConstructRelatedWord(h.Title, WordRelation.Holonym)));
            }
            
            if (relations.HasFlag(WordRelation.Meronym))
            {
                result.AddRange(_database.GetMeronyms(synsetIds).Select(m =>
                    ConstructRelatedWord(m.Title, WordRelation.Meronym)));
            }

            if (relations.HasFlag(WordRelation.Association))
            {
                result.AddRange(_database.GetAssociations(synsetIds).Select(a =>
                    ConstructRelatedWord(a.Title, WordRelation.Association)));
            }
            
            if (relations.HasFlag(WordRelation.Cause))
            {
                result.AddRange(_database.GetCauses(synsetIds).Select(c => 
                    ConstructRelatedWord(c.Title,WordRelation.Cause)));
            }
            
            if (relations.HasFlag(WordRelation.Effect))
            {
                result.AddRange(_database.GetEffects(synsetIds).Select(e =>
                    ConstructRelatedWord(e.Title, WordRelation.Effect)));
            }

            return result.ToArray();
        }
        
        /// <summary>
        /// Для слова нужно вернуть все слова, чьё взаимоотношение с ними попадает под relations
        /// если в методе выше для слова "птица" и relation = Domain нужно вернуть "биология",
        /// то в этом методе для слова "биология" и relation = Domain нужно вернуть "птица"
        /// </summary>
        /// <param name="lemma"></param>
        /// <param name="inverted_relations"></param>
        /// <returns></returns>
        public RelatedWord[] GetInvertedRelations(string lemma, WordRelation inverted_relations)
        {
            var result = new List<RelatedWord>();

            var senses = _database.Senses.Where(s => s.Lemma == lemma.ToUpper()).ToArray();
            var synsets = _database.GetSynsets(senses.Select(s=> s.SynsetId).ToArray());
            var synsetIds = synsets.Select(s => s.Id).ToArray();

            if (inverted_relations.HasFlag(WordRelation.Hyponym))
            {
                result.AddRange(_database.GetHypernyms(synsetIds).Select(h =>
                    ConstructRelatedWord(h.Title, WordRelation.Hypernym)));
            }

            if (inverted_relations.HasFlag(WordRelation.SameRoot))
            {   
                //возвращаю sense а не synset
                var derivatives = _database.GetDerivations(senses.Select(s=> s.Id).ToArray()); 

                result.AddRange(derivatives
                    .Select(derivative => 
                        ConstructRelatedWord(derivative.Name, WordRelation.SameRoot)));
            }
            
            //not inversable
            if (inverted_relations.HasFlag(WordRelation.Synset))
            {
                result.AddRange(synsets.Select(s => 
                    ConstructRelatedWord(s.Title, WordRelation.Synset)));
            }

            if (inverted_relations.HasFlag(WordRelation.Hypernym))
            {
                result.AddRange(_database.GetHyponyms(synsetIds).Select(h =>
                    ConstructRelatedWord(h.Title, WordRelation.Hyponym)));
            }

            if (inverted_relations.HasFlag(WordRelation.Domain))
            {
                result.AddRange(_database.GetDomainItems(synsetIds).Select(d =>
                    ConstructRelatedWord(d.Title, WordRelation.DomainItem)));
            }
            
            //not inversable
            if (inverted_relations.HasFlag(WordRelation.PartOfSpeechSynonym))
            {
                result.AddRange(_database.GetPosSynonyms(synsetIds).Select(p =>
                    ConstructRelatedWord(p.Title, WordRelation.PartOfSpeechSynonym)));
            }

            if (inverted_relations.HasFlag(WordRelation.Holonym))
            {
                result.AddRange(_database.GetMeronyms(synsetIds).Select(m =>
                    ConstructRelatedWord(m.Title, WordRelation.Meronym)));
            }
        
            if (inverted_relations.HasFlag(WordRelation.Meronym))
            {
                result.AddRange(_database.GetMeronyms(synsetIds).Select(h =>
                    ConstructRelatedWord(h.Title, WordRelation.Holonym)));
            }

            if (inverted_relations.HasFlag(WordRelation.Association))
            {
                result.AddRange(_database.GetAscRelations(synsetIds).Select(a =>
                    ConstructRelatedWord(a.Title, WordRelation.RelatedAssociation)));
            }
        
            if (inverted_relations.HasFlag(WordRelation.Cause))
            {
                result.AddRange(_database.GetEffects(synsetIds).Select(e => 
                    ConstructRelatedWord(e.Title,WordRelation.Effect)));
            }
        
            if (inverted_relations.HasFlag(WordRelation.Effect))
            {
                result.AddRange(_database.GetCauses(synsetIds).Select(c =>
                    ConstructRelatedWord(c.Title, WordRelation.Cause)));
            }

            return result.ToArray();
        }
        
        private RelatedWord ConstructRelatedWord(string rawText, WordRelation relation)
        {
            var lemma = _nestor.Lemmatize(rawText, MorphOption.Distinct).First();
            return new RelatedWord(lemma, rawText, relation);
        }
    }
}