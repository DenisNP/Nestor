using System.Linq;
using NUnit.Framework;

namespace Nestor.Poetry.Tests
{
    [TestFixture]
    public class FootAnalyzerTests
    {
        private FootAnalyser _footAnalyser;
        
        [SetUp]
        public void Setup()
        {
            _footAnalyser ??= new FootAnalyser();
        }

        [Test]
        public void GetStressesTest()
        {
            const string w1 = "замковый";
            StressType[] s1 = _footAnalyser.GetPoeticStresses(w1);
            Assert.AreEqual(3, s1.Length);
            Assert.AreEqual(StressType.CanBeStressed, s1[0]);
            Assert.AreEqual(StressType.CanBeStressed, s1[1]);
            Assert.AreEqual(StressType.StrictlyUnstressed, s1[2]);
            
            const string w2 = "параграфов";
            StressType[] s2 = _footAnalyser.GetPoeticStresses(w2);
            Assert.AreEqual(4, s2.Length);
            Assert.AreEqual(StressType.StrictlyStressed, s2[1]);
            Assert.AreEqual(3, s2.Count(s => s == StressType.StrictlyUnstressed));

            const string w3 = "душ";
            StressType[] s3 = _footAnalyser.GetPoeticStresses(w3);
            Assert.AreEqual(1, s3.Length);
            Assert.AreEqual(StressType.CanBeStressed, s3[0]);

            const string w4 = "бурдылька";
            StressType[] s4 = _footAnalyser.GetPoeticStresses(w4);
            Assert.AreEqual(3, s4.Length);
            Assert.IsTrue(s4.All(s => s == StressType.CanBeStressed));
        }
    }
}