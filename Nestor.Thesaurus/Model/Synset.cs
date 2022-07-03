using Newtonsoft.Json;

namespace Nestor.Thesaurus.Model;

public class Synset
{
    public string Definition { get; set; }
    public string Id { get; set; }
    public Pos PartOfSpeech { get; set; }
    public string Title { get; set; }
}