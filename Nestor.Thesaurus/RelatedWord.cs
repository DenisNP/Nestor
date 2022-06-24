using System.Linq;

namespace Nestor.Thesaurus
{
    public record RelatedWord(string[] Lemmas, string TextRaw, WordRelation Relation)
    {
        public string Word => Lemmas.First();
        public bool IsCollocation => Lemmas.Length > 1;
    }
}