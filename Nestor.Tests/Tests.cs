using System;
using System.Linq;
using Nestor;
using NUnit.Framework;

namespace NestorTests
{
    [TestFixture]
    public class Tests
    {
        private NestorMorph _nMorph;
        
        [SetUp]
        public void SetUp()
        {
            _nMorph = new NestorMorph();
        }
        
        [Test]
        public void TestTokenize()
        {
            var tokens = _nMorph.Tokenize("Пришёл, увидел, победил. 123-45, 67//# Как-то раз.");
            Assert.AreEqual(5, tokens.Length);
            Assert.AreEqual("пришёл", tokens[0]);
            Assert.AreEqual("как-то", tokens[3]);

            var tokensNum = _nMorph.Tokenize("В 140 солнц закат пылал.", MorphOption.KeepNumbers);
            Assert.AreEqual(5, tokensNum.Length);
            Assert.AreEqual("140", tokensNum[1]);

            var tokensPrepositions = _nMorph.Tokenize("Под о ш в а", MorphOption.RemovePrepositions);
            Assert.AreEqual(1, tokensPrepositions.Length);
            Assert.AreEqual("ш", tokensPrepositions[0]);

            var tokensExistent = _nMorph.Tokenize(
                "Съешь ещё этих бурдылек и выпей куздру", 
                MorphOption.RemoveNonExistent
                );
            Assert.AreEqual(5, tokensExistent.Length);
            Assert.False(tokensExistent.Contains("бурдылек"));
        }

        [TearDown]
        public void Dispose()
        {
            _nMorph = null;
        }
    }
}