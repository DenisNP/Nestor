using System.Text.Json.Serialization;

namespace Nestor.Thesaurus.Model;

public class Sense
{
    [JsonPropertyName("concept_id")]
    public int ConceptId { get; set; }
    [JsonPropertyName("entry_id")]
    public int EntryId { get; set; }
    public string Id { get; set; }
    public string Lemma { get; set; }
    [JsonPropertyName("main_word")]
    public string MainWord { get; set; }
    public int Meaning { get; set; }
    public string Name { get; set; }
    [JsonPropertyName("part_of_speech")]
    public Pos PartOfSpeech { get; set; }
    [JsonPropertyName("synset_id")]
    public string SynsetId { get; set; }
    [JsonPropertyName("synt_type")]
    public string SyntType { get; set; }
}

