using System.Text.Json.Serialization;

namespace Nestor.Thesaurus.Model;

public class PosSynonymyRelation
{
    public string LeftId { get; set; }
    public string RightId { get; set; }
}

public class MeronymyRelation
{
    public string HolonymId { get; set; }
    public string MeronymId { get; set; }
}

public class InstanceRelation
{
    public string ClassId { get; set; }
    public string InstanceId { get; set; }
}

public class HypernymRelation
{
    public string HypernymId { get; set; }
    public string HyponymId { get; set; }
}

public class EntailmentRelation
{
    public string ConclusionId { get; set; }
    public string PremiseId { get; set; }
}

public class DomainRelation
{
    public string DomainId { get; set; }
    public string DomainItemId { get; set; }
}

public class DerivationRelation
{
    public string DerivativeId { get; set; }
    public string SourceId { get; set; }
}

public class CompositionRelation
{
    public string PhraseId { get; set; }
    public string WordId { get; set; }
}

public class CauseRelation
{
    public string CauseId { get; set; }
    public string EffectId { get; set; }
}

public class AntonymyRelation
{
    public string LeftId { get; set; }
    public string RightId { get; set; }
}

public class AssociationRelation
{
    public string AssociationId { get; set; }
    public string RelationId { get; set; }
}