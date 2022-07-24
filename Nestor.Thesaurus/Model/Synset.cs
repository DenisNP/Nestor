namespace Nestor.Thesaurus.Model;

public class Synset
{
    private string _title;

    public string Definition { get; set; }
    public string Id { get; set; }
    public Pos PartOfSpeech { get; set; }

    public string Title
    {
        get => _title;
        set => _title = value.ToLower();
    }
}