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
            var s = _nNyms.Synonyms(word);
            
            Assert.IsTrue(s.Contains("противник"));
        }
    }
}