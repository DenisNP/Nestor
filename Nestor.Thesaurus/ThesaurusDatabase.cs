using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
        private Dictionary<string, Sense> _senses = new();

        /// <summary>
        /// Синсет = множество сущностей Sense с одинаковыми значениями и с одной частью речи
        /// </summary>
        private Dictionary<string, Synset> _synsets = new();

        /// <summary>
        /// Синонимы из других частей речи
        /// </summary>
        private Dictionary<string, string[]> _posSynonymyLeftByRight = new();
        private Dictionary<string, string[]> _posSynonymyRightByLeft = new ();

        /// <summary>
        /// Меронимы и холонимы - части и целые, например, "желудь" / "дуб"
        /// </summary>
        private Dictionary<string, string[]> _meronymsByHolonym = new ();
        private Dictionary<string, string[]> _holonymsByMeronym = new ();

        /// <summary>
        /// Классы и экземпляры, например, "Смоленск" / "областной центр"
        /// </summary>
        private Dictionary<string, string[]> _classByInstance = new ();
        private Dictionary<string, string[]> _instanceByClass = new ();

        /// <summary>
        /// Гипонимы и гиперонимы - более частные и более общие понятия, например, "спаржа" / "овощи"
        /// </summary>
        private Dictionary<string, string[]> _hyponymsByHypernym = new ();
        private Dictionary<string, string[]> _hypernymsByHyponym = new ();

        /// <summary>
        /// Предпосылки и возможные выводы из них, например, "прибежать" / "бегать" (TODO только для глаголов)
        /// </summary>
        private Dictionary<string, string[]> _conclusionByPremise = new ();
        private Dictionary<string, string[]> _premiseByConclusion = new ();

        /// <summary>
        /// Домены и их атрибуты, например, "спорт" / "мяч"
        /// </summary>
        private Dictionary<string, string[]> _domainsByItem = new ();
        private Dictionary<string, string[]> _itemsByDomain = new ();

        /// <summary>
        /// От каких слов произошло данное, и какие произошли от него, например, "приятель" / "приятельский"
        /// Как правило однокоренные 
        /// </summary>
        private Dictionary<string, string[]> _derivativesBySource = new ();
        private Dictionary<string, string[]> _sourcesByDerivative = new ();

        /// <summary>
        /// Из каких слов состоит фраза, и в каких фразах участвует слово, например, "чувство" / "порыв чувств"
        /// </summary>
        private Dictionary<string, string[]> _phrasesByWord = new ();
        private Dictionary<string, string[]> _wordsByPhrase = new ();

        /// <summary>
        /// Причины и следствия, например, (TODO только для глаголов)
        /// </summary>
        private Dictionary<string, string[]> _causesByEffect = new ();
        private Dictionary<string, string[]> _effectsByCause = new ();

        /// <summary>
        /// Антонимы - слова противоположные по смыслу
        /// </summary>
        private Dictionary<string, string[]> _antonymyLeftByRight = new ();
        private Dictionary<string, string[]> _antonymyRightByLeft = new ();

        /// <summary>
        /// Ассоциации - слова, которые принадлежат одной группе, например, "прибежать" / "бегать"
        /// </summary>
        private Dictionary<string, string[]> _associationsByRelation = new ();
        private Dictionary<string, string[]> _relationsByAssociation = new ();

        /// <summary>
        /// Основной конструктор словаря - читает json файлы выгруженные из реляционной СУБД и записывает из в объект базы.
        /// </summary>
        /// <param name="pathToThesaurus"> Путь к архиву с JSON файлам тезауруса </param>
        public ThesaurusDatabase(string pathToThesaurus)
        {
            Console.WriteLine("Nestor is loading synset thesaurus...");
            
            try
            {
                var assembly = Assembly.GetCallingAssembly();
                using Stream file = assembly.GetManifestResourceStream($"Nestor.Thesaurus.Dict.{pathToThesaurus}")
                                    ?? throw new IOException();

                using var zip = new ZipArchive(file, ZipArchiveMode.Read);

                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    var name = entry.Name.Split(".")[0];
                    Type type = GetTypeFromString(name);

                    using var stream = entry.Open();
                    using var sr = new StreamReader(stream);
                    
                    switch (name)
                    {
                        case "synset": 
                            var synsets = ReadJsonStream(sr.ReadToEnd(), type) as Synset[];
                            foreach (var synset in synsets)
                            {
                                _synsets.Add(synset.Id, synset);
                            }
                            break;
                        case "sense": 
                            var senses = ReadJsonStream(sr.ReadToEnd(), type) as Sense[];
                            foreach (var sense in senses)
                            {
                                _senses.Add(sense.Id, sense);
                            }
                            break;
                        case "pos_synonymy_relation":
                            FillDictionary(ReadJsonStream(sr.ReadToEnd(), type) as PosSynonymyRelation[],
                                _posSynonymyLeftByRight,
                                _posSynonymyRightByLeft,
                                (h) => h.LeftId,
                                (h) => h.RightId); 
                            break;
                        case "meronymy_relation": 
                            FillDictionary(ReadJsonStream(sr.ReadToEnd(), type) as MeronymyRelation[],
                                _holonymsByMeronym,
                                _meronymsByHolonym,
                                (h) => h.HolonymId,
                                (h) => h.MeronymId);  
                            break;
                        case "instance_relation": 
                            FillDictionary(ReadJsonStream(sr.ReadToEnd(), type) as InstanceRelation[],
                                _classByInstance,
                                _instanceByClass,
                                (h) => h.ClassId,
                                (h) => h.InstanceId); 
                            break;
                        case "hypernym_relation": 
                            FillDictionary(ReadJsonStream(sr.ReadToEnd(), type) as HypernymRelation[],
                                _hypernymsByHyponym,
                                _hyponymsByHypernym,
                                (h) => h.HypernymId,
                                (h) => h.HyponymId);
                            break;
                        case "entailment_relation": 
                            FillDictionary(ReadJsonStream(sr.ReadToEnd(), type) as EntailmentRelation[],
                                _conclusionByPremise,
                                _premiseByConclusion,
                                (h) => h.ConclusionId,
                                (h) => h.PremiseId);
                            break;
                        case "domain_relation": 
                            FillDictionary(ReadJsonStream(sr.ReadToEnd(), type) as DomainRelation[],
                                _domainsByItem,
                                _itemsByDomain,
                                (h) => h.DomainId,
                                (h) => h.DomainItemId);  
                            break;
                        case "derivation_relation": 
                            FillDictionary(ReadJsonStream(sr.ReadToEnd(), type) as DerivationRelation[],
                                _derivativesBySource,
                                _sourcesByDerivative,
                                (h) => h.DerivativeId,
                                (h) => h.SourceId); 
                            break;
                        case "composition_relation": 
                            FillDictionary(ReadJsonStream(sr.ReadToEnd(), type) as CompositionRelation[],
                                _phrasesByWord,
                                _wordsByPhrase,
                                (h) => h.PhraseId,
                                (h) => h.WordId);  
                            break;
                        case "cause_relation": 
                            FillDictionary(ReadJsonStream(sr.ReadToEnd(), type) as CauseRelation[],
                                _causesByEffect,
                                _effectsByCause,
                                (h) => h.CauseId,
                                (h) => h.EffectId); 
                            break;
                        case "association_relation": 
                            FillDictionary(ReadJsonStream(sr.ReadToEnd(), type) as AssociationRelation[],
                                _associationsByRelation,
                                _relationsByAssociation,
                                (h) => h.AssociationId,
                                (h) => h.RelationId);
                            break;
                        case "antonymy_relation": 
                            FillDictionary(ReadJsonStream(sr.ReadToEnd(), type) as AntonymyRelation[],
                                _antonymyLeftByRight,
                                _antonymyRightByLeft,
                                (h) => h.LeftId,
                                (h) => h.RightId);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading thesaurus: " + e.Message);
                throw new IOException($"Cannot load file: {pathToThesaurus}");
            }
            Console.WriteLine("Thesaurus loaded...OK");
        }

        private object ReadJsonStream(string stream, Type type)
        {
            var result = JsonConvert.DeserializeObject(
                stream,
                type,
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
                Console.WriteLine($"...{result.GetType().Name}");
            }
            else
            {
                throw new FileLoadException($"Error parsing JSON objects from thesaurus file: {type.Name}!");
            }
            return result;
        }
  
        public Sense[] GetSenses(string[] senseIds)
        {
            return senseIds.Select(senseId => _senses[senseId]).ToArray();
        }
        
        public Sense[] GetAllSenses()
        {
            return _senses.Values.ToArray();
        }
        public Synset[] GetSynsets(string[] synsetIds)
        {
            return synsetIds.Select(synsetId => _synsets[synsetId]).ToArray();
        }

        // public string GetLemmaForSynset(Synset synset)
        // {
        //     return Senses.FirstOrDefault(s => s.SynsetId == synset.Id)?.Lemma;
        // }

        public Synset[] GetPosSynonyms(string[] ids) // part of speech synonyms - not invertible
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_posSynonymyLeftByRight
                    .TryGetValue(id, out var synonyms) ? synonyms : Array.Empty<string>());
            }
            return GetSynsets(resultIds.ToArray());
        }

        public Synset[] GetHypernyms(string[] ids) // inverse hyponyms
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_hypernymsByHyponym
                    .TryGetValue(id, out var hypernyms) ? hypernyms : Array.Empty<string>());
            }
            return GetSynsets(resultIds.ToArray());
        }

        public Synset[] GetHyponyms(string[] ids) // inverse hypernyms
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_hyponymsByHypernym
                    .TryGetValue(id, out var hyponyms) ? hyponyms : Array.Empty<string>());
            }
            return GetSynsets(resultIds.ToArray());
        }

        public Synset[] GetHolonyms(string[] ids) // inverse meronyms 
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_holonymsByMeronym
                    .TryGetValue(id, out var holonyms) ? holonyms : Array.Empty<string>());
            }
            return GetSynsets(resultIds.ToArray());
        }

        public Synset[] GetMeronyms(string[] ids) // inverse holonyms
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_meronymsByHolonym
                    .TryGetValue(id , out var holonym) ? holonym : Array.Empty<string>());
            }
            return GetSynsets(resultIds.ToArray());
        }

        public Synset[] GetDomains(string[] ids) // inverse domain items
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_domainsByItem
                    .TryGetValue(id, out var domainIds) ? domainIds : Array.Empty<string>());
            }
            return GetSynsets(resultIds.ToArray());
        }

        public Synset[] GetDomainItems(string[] ids) // inverse domains
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_itemsByDomain
                    .TryGetValue(id, out var itemIds) ? itemIds : Array.Empty<string>());
            }
            return GetSynsets(resultIds.ToArray());
        }

        public Synset[] GetAssociations(string[] ids)
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_associationsByRelation
                    .TryGetValue(id, out var associationIds) ? associationIds : Array.Empty<string>());
            }
            return GetSynsets(resultIds.ToArray());
        }
        
        public Synset[] GetAscRelations(string[] ids)
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_relationsByAssociation
                    .TryGetValue(id, out var relationIds) ? relationIds : Array.Empty<string>());
            }
            return GetSynsets(resultIds.ToArray());
        }

        public Synset[] GetCauses(string[] ids) //inverse effects
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_causesByEffect
                    .TryGetValue(id, out var causeIds) ? causeIds : Array.Empty<string>());
            }
            return GetSynsets(resultIds.ToArray());
        }

        public Synset[] GetEffects(string[] ids) // inverse causes
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_effectsByCause
                    .TryGetValue(id, out var effectIds) ? effectIds : Array.Empty<string>());
            }
            return GetSynsets(resultIds.ToArray());
        }

        public Sense[] GetDerivations(string[] ids) // words with same root
        {
            var resultIds = new List<string>();
            foreach (var id in ids)
            {
                resultIds.AddRange(_derivativesBySource
                    .TryGetValue(id, out var derivativeIds) ? derivativeIds : Array.Empty<string>());
            }
            return GetSenses(resultIds.ToArray());
        }
        
        private Type GetTypeFromString(string typeName)
        {
            return typeName switch
            {
                "synset" => typeof(Synset[]),
                "sense" => typeof(Sense[]),
                "pos_synonymy_relation" => typeof(PosSynonymyRelation[]),
                "antonymy_relation" => typeof(AntonymyRelation[]),
                "association_relation" => typeof(AssociationRelation[]),
                "composition_relation" => typeof(CompositionRelation[]),
                "derivation_relation" => typeof(DerivationRelation[]),
                "domain_relation" => typeof(DomainRelation[]),
                "hypernym_relation" => typeof(HypernymRelation[]),
                "instance_relation" => typeof(InstanceRelation[]),
                "meronymy_relation" => typeof(MeronymyRelation[]),
                "cause_relation" => typeof(CauseRelation[]),
                "entailment_relation" => typeof(EntailmentRelation[]),
                _ => typeof(object)
            };
        }

        private void FillDictionary<T>(
            T[] source,
            Dictionary<string, string[]> leftByRight,
            Dictionary<string, string[]> rightByLeft,
            Func<T, string> getLeft,
            Func<T, string> getRight)
        {
            Console.WriteLine($"Started filling dictionary for {typeof(T)}...");
            
            foreach (var item in source)
            {
                var left = getLeft(item);
                var right = getRight(item);

                if (!leftByRight.ContainsKey(right))
                {
                    leftByRight.Add(right, new string[] { left });
                }
                else
                {
                    leftByRight[right] = leftByRight[right]
                        .Concat(new string[] { left }).ToArray();
                }

                if (!rightByLeft.ContainsKey(left))
                {
                    rightByLeft.Add(left, new string[] { right });
                }
                else
                {
                    rightByLeft[left] = rightByLeft[left]
                        .Concat(new string[] { right }).ToArray();
                }
            }
            Console.WriteLine($"Finished filling dictionaries... Lengths: {leftByRight.Count}, {rightByLeft.Count}");
        }
 
    }
}