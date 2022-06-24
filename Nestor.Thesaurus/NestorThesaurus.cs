using System;

namespace Nestor.Thesaurus
{
    public class NestorThesaurus
    {
        private static ThesaurusDatabase _database;
        private readonly NestorMorph _nestor;
        
        public NestorThesaurus(NestorMorph nestorMorph = null)
        {
            _database ??= new ThesaurusDatabase();
            _nestor = nestorMorph ?? new NestorMorph();
        }
        
        public RelatedWord[] GetStraightRelations(string lemma, WordRelation relations)
        {
            throw new NotImplementedException();
            // для каждого флага из relations нужно для lemma вернуть список объектов RelatedWord,
            // в каждом таком объекте слово или словосочетание и тип взаимоотношения с исходным словом
        }

        public RelatedWord[] GetInvertedRelations(string lemma, WordRelation relations)
        {
            throw new NotImplementedException();
            // для слова нужно вернуть все слова, чьё взаимоотношение с ними попадает под relations
            // если в методе выше для слова "птица" и relation = Domain нужно вернуть "биология",
            // то в этом методе для слова "биология" и relation = Domain нужно вернуть "птица"
        }
        
        private RelatedWord ConstructRelatedWord(string rawText, WordRelation relation)
        {
            string[] lemmas = _nestor.Lemmatize(rawText, MorphOption.Distinct);
            return new RelatedWord(lemmas, rawText, relation);
        }
    }
}