using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Nestor.Thesaurus.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nestor.Thesaurus
{
    public class ThesaurusDatabase
    {
        /// <summary>
        /// Смысл - одно конкретное слово или словосочетание с конкретным же значением
        /// </summary>
        public Sense[] Senses { get; }

        /// <summary>
        /// Синсет = множество сущностей Sense с одинаковыми значениями и с одной частью речи
        /// </summary>
        private Synset[] Synsets { get; }

        /// <summary>
        /// Синонимы из других частей речи
        /// </summary>
        private PosSynonymyRelation[] PosSynonymyRelations { get; }

        /// <summary>
        /// Меронимы и холонимы - части и целые, например, "желудь" / "дуб"
        /// </summary>
        private MeronymyRelation[] MeronymyRelations { get; }

        /// <summary>
        /// Классы и экземпляры, например, "Смоленск" / "областной центр"
        /// </summary>
        private InstanceRelation[] InstanceRelations { get; }

        /// <summary>
        /// Гипонимы и гиперонимы - более частные и более общие понятия, например, "спаржа" / "овощи"
        /// </summary>
        private HypernymRelation[] HypernymRelations { get; }

        /// <summary>
        /// Предпосылки и возможные выводы из них, например, "прибежать" / "бегать" (TODO только для глаголов)
        /// </summary>
        private EntailmentRelation[] EntailmentRelations { get; }

        /// <summary>
        /// Домены и их атрибуты, например, "спорт" / "мяч"
        /// </summary>
        private DomainRelation[] DomainRelations { get; }

        /// <summary>
        /// От каких слов произошло данное, и какие произошли от него, например, "приятель" / "приятельский"
        /// Как правило однокоренные 
        /// </summary>
        private DerivationRelation[] DerivationRelations { get; }

        /// <summary>
        /// Из каких слов состоит фраза, и в каких фразах участвует слово, например, "чувство" / "порыв чувств"
        /// </summary>
        private CompositionRelation[] CompositionRelations { get; }

        /// <summary>
        /// Причины и следствия, например, (TODO только для глаголов)
        /// </summary>
        private CauseRelation[] CauseRelations { get; }

        /// <summary>
        /// Антонимы - слова противоположные по смыслу
        /// </summary>
        private AntonymyRelation[] AntonymyRelations { get; }

        /// <summary>
        /// Ассоциации - слова, которые принадлежат одной группе, например, "прибежать" / "бегать"
        /// </summary>
        public AssociationRelation[] AssociationRelations { get; }

        /// <summary>
        /// Основной конструктор словаря - читает json файлы выгруженные из реляционной СУБД и записывает из в объект базы.
        /// </summary>
        /// <param name="pathToThesaurus"> Путь к архиву с JSON файлам тезауруса </param>
        public ThesaurusDatabase(string pathToThesaurus)
        {
            Console.WriteLine("Unzipping file...");

            if (!Directory.Exists(pathToThesaurus))
            {
                try
                {
                    ZipFile.ExtractToDirectory($"{pathToThesaurus}.zip",
                        $"{pathToThesaurus}");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error loading thesaurus: " + e.Message);
                    throw new IOException($"Cannot load file: {pathToThesaurus}.zip");
                }
            }

            Console.WriteLine("Nestor is loading synset thesaurus...");

            Synsets = ReadJson<Synset>($"{pathToThesaurus}/synset.json");
            Senses = ReadJson<Sense>($"{pathToThesaurus}/sense.json");
            PosSynonymyRelations = ReadJson<PosSynonymyRelation>($"{pathToThesaurus}/pos_synonymy_relation.json");
            MeronymyRelations = ReadJson<MeronymyRelation>($"{pathToThesaurus}/meronymy_relation.json");
            HypernymRelations = ReadJson<HypernymRelation>($"{pathToThesaurus}/hypernym_relation.json");
            EntailmentRelations = ReadJson<EntailmentRelation>($"{pathToThesaurus}/entailment_relation.json");
            DomainRelations = ReadJson<DomainRelation>($"{pathToThesaurus}/domain_relation.json");
            DerivationRelations = ReadJson<DerivationRelation>($"{pathToThesaurus}/derivation_relation.json");
            CauseRelations = ReadJson<CauseRelation>($"{pathToThesaurus}/cause_relation.json");
            AntonymyRelations = ReadJson<AntonymyRelation>($"{pathToThesaurus}/antonymy_relation.json");
            AssociationRelations = ReadJson<AssociationRelation>($"{pathToThesaurus}/association_relation.json");
            
            CompositionRelations = ReadJson<CompositionRelation>($"{pathToThesaurus}/composition_relation.json");
            InstanceRelations = ReadJson<InstanceRelation>($"{pathToThesaurus}/instance_relation.json");

            Console.WriteLine("Thesaurus loaded...OK");
        }

        private T[] ReadJson<T>(string fileName)
        {
            var result = JsonConvert.DeserializeObject<T[]>(
                File.ReadAllText(fileName),
                new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    },
                    Formatting = Formatting.Indented
                });

            if (result != null)
            {
                Console.WriteLine($"...{result[0].GetType().Name}: {result.Length}");
            }

            return result;
        }

        private Sense[] GetSenses(string[] senseIds)
        {
            return senseIds.Select(sId => Senses.FirstOrDefault(s => s.Id == sId)).ToArray();
        }

        public Synset[] GetSynsets(string[] synsetIds)
        {
            return synsetIds.Select(sId => Synsets.FirstOrDefault(s => s.Id == sId)).ToArray();
        }

        // public string GetLemmaForSynset(Synset synset)
        // {
        //     return Senses.FirstOrDefault(s => s.SynsetId == synset.Id)?.Lemma;
        // }

        public Synset[] GetPosSynonyms(string[] ids) // part of speech synonyms - not invertible
        {
            var posSynonymyRelationIds = PosSynonymyRelations
                .Where(x => ids.Contains(x.LeftId))
                .Select(s => s.RightId).ToArray();

            return GetSynsets(posSynonymyRelationIds);
        }

        public Synset[] GetHypernyms(string[] ids) // inverse hyponyms
        {
            var hypernymIds = HypernymRelations
                .Where(x => ids.Contains(x.HyponymId))
                .Select(s => s.HypernymId).ToArray();

            return GetSynsets(hypernymIds);
        }

        public Synset[] GetHyponyms(string[] ids) // inverse hypernyms
        {
            var hyponymIds = HypernymRelations
                .Where(x => ids.Contains(x.HypernymId))
                .Select(s => s.HyponymId).ToArray();

            return GetSynsets(hyponymIds);
        }

        public Synset[] GetHolonyms(string[] ids) // inverse meronyms 
        {
            var holonymIds = MeronymyRelations
                .Where(x => ids.Contains(x.MeronymId))
                .Select(s => s.HolonymId).ToArray();

            return GetSynsets(holonymIds);
        }

        public Synset[] GetMeronyms(string[] ids) // inverse holonyms
        {
            var meronymIds = MeronymyRelations
                .Where(x => ids.Contains(x.HolonymId))
                .Select(s => s.MeronymId).ToArray();

            return GetSynsets(meronymIds);
        }

        public Synset[] GetDomains(string[] ids) // inverse domain items
        {
            var domainIds = DomainRelations
                .Where(x => ids.Contains(x.DomainItemId))
                .Select(s => s.DomainId).ToArray();

            return GetSynsets(domainIds);
        }

        public Synset[] GetDomainItems(string[] ids) // inverse domains
        {
            var domainIds = DomainRelations
                .Where(x => ids.Contains(x.DomainId))
                .Select(s => s.DomainItemId).ToArray();

            return GetSynsets(domainIds);
        }

        public Synset[] GetAssociations(string[] ids)
        {
            var associationIds = AssociationRelations
                .Where(x => ids.Contains(x.RelationId))
                .Select(s => s.AssociationId).ToArray();

            return GetSynsets(associationIds);
        }
        
        public Synset[] GetAscRelations(string[] ids)
        {
            var relationIds = AssociationRelations
                .Where(x => ids.Contains(x.AssociationId))
                .Select(s => s.RelationId).ToArray();

            return GetSynsets(relationIds);
        }

        public Synset[] GetCauses(string[] ids) //inverse effects
        {
            var causeIds = CauseRelations
                .Where(x => ids.Contains(x.EffectId))
                .Select(s => s.CauseId).ToArray();

            return GetSynsets(causeIds);
        }

        public Synset[] GetEffects(string[] ids) // inverse causes
        {
            var effectIds = CauseRelations
                .Where(x => ids.Contains(x.CauseId))
                .Select(s => s.EffectId).ToArray();

            return GetSynsets(effectIds);
        }

        public Sense[] GetDerivations(string[] ids) // words with same root
        {
            var derivationIds = DerivationRelations
                .Where(x => ids.Contains(x.DerivativeId))
                .Select(s => s.SourceId).ToArray();

            return GetSenses(derivationIds);
        }
    }
}