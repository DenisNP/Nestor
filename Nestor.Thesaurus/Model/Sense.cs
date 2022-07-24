namespace Nestor.Thesaurus.Model;

public class Sense
{
    public int ConceptId { get; set; }
    public int EntryId { get; set; }
    public string Id { get; set; }

    private string _lemma;
    private string _name;

    public string Lemma
    {
        get => _lemma;
        set => _lemma = value.ToLower();
    }

    public string MainWord { get; set; }
    public int Meaning { get; set; }

    public string Name
    {
        get => _name;
        set => _name = value.ToLower();
    }

    public Pos PartOfSpeech { get; set; }
    public string SynsetId { get; set; }
    public string SyntType { get; set; }
}

