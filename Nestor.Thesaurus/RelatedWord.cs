using System.Linq;

namespace Nestor.Thesaurus
{
    public record RelatedWord(string Lemma, string TextRaw, WordRelation Relation);
}