using Nestor.Chronicles;
using NUnit.Framework;

namespace Nestor.Chronecles.Tests
{
    public class Tests
    {
        [Test]
        public void TestGetRecord()
        {
            var nc = new NestorChronicles();
            var record = nc.GetRecord("аббат");
            Assert.Greater(record.Best.Count, 0);
        }
    }
}