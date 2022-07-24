using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Nestor.Thesaurus.Model;

public class Synset
{
    public string Definition { get; set; }
    public string Id { get; set; }
    [JsonPropertyName("part_of_speech")]
    public Pos PartOfSpeech { get; set; }
    public string Title { get; set; }
}