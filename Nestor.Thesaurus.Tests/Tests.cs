using NUnit.Framework;

namespace Nestor.Thesaurus.Tests;

[TestFixture]
public class Tests
{
    private NestorThesaurus _nThesaurus;
    
    [SetUp]
    public void Setup()
    {
        _nThesaurus ??= new NestorThesaurus();
    }

    [Test]
    public void TestThesaurusStraightRelations()
    {
        var hyponyms = _nThesaurus.GetStraightRelations("рост", WordRelation.Hyponym);
        Assert.That(hyponyms, Has.Length.EqualTo(28));
        
        var sameRoot = _nThesaurus.GetStraightRelations("рост", WordRelation.SameRoot);
        Assert.That(sameRoot, Has.Length.EqualTo(93));

        var synsets = _nThesaurus.GetStraightRelations("рост", WordRelation.Synset);
        Assert.That(synsets, Has.Length.EqualTo(4));
        
        var posSynonyms = _nThesaurus.GetStraightRelations("рост", WordRelation.PartOfSpeechSynonym);
        Assert.That(posSynonyms, Has.Length.EqualTo(5));
        
        var hypernyms = _nThesaurus.GetStraightRelations("рост", WordRelation.Hypernym);
        Assert.That(hypernyms, Has.Length.EqualTo(6));
        
        var domains = _nThesaurus.GetStraightRelations("рост", WordRelation.Domain);
        Assert.That(domains, Has.Length.EqualTo(2));

        var holonyms = _nThesaurus.GetStraightRelations("рост", WordRelation.Holonym);
        Assert.That(holonyms, Has.Length.EqualTo(1));
        
        var meronyms = _nThesaurus.GetStraightRelations("рост", WordRelation.Meronym);
        Assert.That(meronyms, Has.Length.EqualTo(4));
        
        var associations = _nThesaurus.GetStraightRelations("рост", WordRelation.Association);
        Assert.That(associations, Has.Length.EqualTo(6));
        
        var causes = _nThesaurus.GetStraightRelations("потерять конечность", WordRelation.Cause);
        Assert.That(causes, Has.Length.EqualTo(1));
        
        var effects = _nThesaurus.GetStraightRelations("разогнать", WordRelation.Effect);
        Assert.That(effects, Has.Length.EqualTo(2));
        
        var multipleRelations = _nThesaurus.GetStraightRelations("рост",
            WordRelation.SameRoot 
                    | WordRelation.Holonym 
                    | WordRelation.Hypernym);
        Assert.That(multipleRelations, Has.Length.EqualTo(100));
    }

    [Test]
    public void TestThesaurusInvertedRelations()
    {
        var hypernyms = _nThesaurus.GetInvertedRelations("рост", WordRelation.Hyponym);
        Assert.That(hypernyms, Has.Length.EqualTo(6));
        
        //same as not inverted
        var sameRoot = _nThesaurus.GetInvertedRelations("рост", WordRelation.SameRoot);
        Assert.That(sameRoot, Has.Length.EqualTo(93));
        
        //samer as not inverted
        var synsets = _nThesaurus.GetInvertedRelations("рост", WordRelation.Synset);
        Assert.That(synsets, Has.Length.EqualTo(4));
        
        var hyponyms = _nThesaurus.GetInvertedRelations("рост", WordRelation.Hypernym);
        Assert.That(hyponyms, Has.Length.EqualTo(28));
        
        var domainItems = _nThesaurus.GetInvertedRelations("биология", WordRelation.Domain);
        Assert.That(domainItems, Has.Length.EqualTo(2112));
        
        //same as not inverted
        var posSynonyms = _nThesaurus.GetInvertedRelations("рост", WordRelation.PartOfSpeechSynonym);
        Assert.That(posSynonyms, Has.Length.EqualTo(5));
        
        var meronyms = _nThesaurus.GetInvertedRelations("рост", WordRelation.Holonym);
        Assert.That(meronyms, Has.Length.EqualTo(4));
        
        var holonyms = _nThesaurus.GetInvertedRelations("рост", WordRelation.Meronym);
        Assert.That(holonyms, Has.Length.EqualTo(4));
        
        var associations = _nThesaurus.GetInvertedRelations("рост", WordRelation.Association);
        Assert.That(associations, Has.Length.EqualTo(6));
        
        var effects = _nThesaurus.GetInvertedRelations("смерть", WordRelation.Cause);
        Assert.That(effects, Has.Length.EqualTo(0));
        
        var causes = _nThesaurus.GetInvertedRelations("смерть", WordRelation.Effect);
        Assert.That(causes, Has.Length.EqualTo(0));
        
         var multipleRelations = _nThesaurus.GetInvertedRelations("рост",
             WordRelation.SameRoot 
                     | WordRelation.Holonym 
                     | WordRelation.Hypernym);
         Assert.That(multipleRelations, Has.Length.EqualTo(125));
    }
    
    [TearDown]
    public void Dispose()
    {
        _nThesaurus = null;
    }
}