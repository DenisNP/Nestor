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
            RhymingPair r1 = _rhymeAnalyzer.ScoreRhyme("нежно", "снежный");
            Assert.Greater(r1.Score, 0.8);
            
            RhymingPair r2 = _rhymeAnalyzer.ScoreRhyme("любовь", "морковь");
            Assert.Greater(r2.Score, 0.8);
            Assert.Less(r2.Score, 0.95);
            
            RhymingPair r3 = _rhymeAnalyzer.ScoreRhyme("пакля", "рвакля");
            Assert.Greater(r3.Score, 0.8);
            
            RhymingPair r4 = _rhymeAnalyzer.ScoreRhyme("палка", "селёдка");
            Assert.Less(r4.Score, 0.3);
            
            RhymingPair r5 = _rhymeAnalyzer.ScoreRhyme("рация", "акция");
            Assert.Greater(r5.Score, 0.4);
            Assert.Less(r5.Score, 0.8);
            
            RhymingPair r6 = _rhymeAnalyzer.ScoreRhyme("окно", "стекло");
            Assert.Less(r6.Score, 0.4);
            
            RhymingPair r7 = _rhymeAnalyzer.ScoreRhyme("стена", "страна");
            Assert.Greater(r7.Score, 0.9);
        }
    }
}