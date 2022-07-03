using System.Linq;

namespace Nestor.Thesaurus
{
    public record RelatedWord(string Lemma, string TextRaw, WordRelation Relation)
    {
        public string Word => Lemma;
        //public bool IsCollocation => Lemmas.Length > 1;
    }
}