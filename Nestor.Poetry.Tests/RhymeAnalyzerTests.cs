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
            double s = _rhymeAnalyzer.ScoreRhyme(new WordWithStress("какие", 2), new WordWithStress("боевые", 3));
            Console.WriteLine(s);
            Assert.Pass();
        }
    }
}