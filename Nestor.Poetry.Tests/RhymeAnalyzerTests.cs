using System;
using NUnit.Framework;

namespace Nestor.Poetry.Tests
{
    [TestFixture]
    public class RhymeAnalyzerTests
    {
        private RhymeAnalyzer _rhymeAnalyzer;
        
        [SetUp]
        public void Setup()
        {
            _rhymeAnalyzer ??= new RhymeAnalyzer();
        }

        [Test]
        public void Test()
        {
            double s = _rhymeAnalyzer.ScoreRhyme(new WordWithStress("факты", 1), new WordWithStress("фанты", 1));
            Console.WriteLine(s);
            Assert.Pass();
        }
    }
}