using System.Collections.Generic;
using NUnit.Framework;

namespace Nestor.Nyms.Tests
{
    public class Tests
    {
        private NestorNyms _nNyms;
        
        [SetUp]
        public void Setup()
        {
            _nNyms ??= new NestorNyms();
        }

        [Test]
        public void TestSynonyms()
        {
            const string word = "враг";
            HashSet<string> s = _nNyms.Synonyms(word);
            Assert.IsTrue(s.Contains("противник"));

            bool areSynonyms = _nNyms.AreSynonyms("красивый", "прекрасный");
            Assert.IsTrue(areSynonyms);
        }

        [Test]
        public void TestAntonyms()
        {
            const string word = "враг";
            HashSet<string> s = _nNyms.Antonyms(word);
            
            Assert.IsTrue(s.Contains("друг"));

            bool areAntonyms = _nNyms.AreAntonyms("большой", "маленький");
            Assert.IsTrue(areAntonyms);
        }
    }
}