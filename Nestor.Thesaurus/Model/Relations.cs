using System.Text.Json.Serialization;

namespace Nestor.Thesaurus.Model;

public class PosSynonymyRelation
{
    [JsonPropertyName("left_id")]
    public string LeftId { get; set; }
    [JsonPropertyName("right_id")]
    public string RightId { get; set; }
}

public class MeronymyRelation
{
    [JsonPropertyName("holonym_id")]
    public string HolonymId { get; set; }
    [JsonPropertyName("meronym_id")]
    public string MeronymId { get; set; }
}

public class InstanceRelation
{
    [JsonPropertyName("class_id")]
    public string ClassId { get; set; }
    [JsonPropertyName("instance_id")]
    public string InstanceId { get; set; }
}

public class HypernymRelation
{
    [JsonPropertyName("hypernym_id")]
    public string HypernymId { get; set; }
    [JsonPropertyName("hyponym_id")]
    public string HyponymId { get; set; }
}

public class EntailmentRelation
{
    [JsonPropertyName("conclusion_id")]
    public string ConclusionId { get; set; }
    [JsonPropertyName("premise_id")]
    public string PremiseId { get; set; }
}

public class DomainRelation
{
    [JsonPropertyName("domain_id")]
    public string DomainId { get; set; }
    [JsonPropertyName("domain_item_id")]
    public string DomainItemId { get; set; }
}

public class DerivationRelation
{
    [JsonPropertyName("derivative_id")]
    public string DerivativeId { get; set; }
    [JsonPropertyName("source_id")]
    public string SourceId { get; set; }
}

public class CompositionRelation
{
    [JsonPropertyName("phrase_id")]
    public string PhraseId { get; set; }
    [JsonPropertyName("word_id")]
    public string WordId { get; set; }
}

public class CauseRelation
{
    [JsonPropertyName("cause_id")]
    public string CauseId { get; set; }
    [JsonPropertyName("effect_id")]
    public string EffectId { get; set; }
}

public class AntonymyRelation
{
    [JsonPropertyName("left_id")]
    public string LeftId { get; set; }
    [JsonPropertyName("right_id")]
    public string RightId { get; set; }
}

public class AssociationRelation
{
    [JsonPropertyName("association_id")]
    public string AssociationId { get; set; }
    [JsonPropertyName("relation_id")]
    public string RelationId { get; set; }
}