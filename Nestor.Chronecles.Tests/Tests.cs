using Nestor.Chronicles;
using NUnit.Framework;

namespace Nestor.Chronecles.Tests
{
    public class Tests
    {

        [Test]
        public void TestCloud()
        {
            var nc = new NestorChronicles();
            var list = nc.Cloud(new []{"мотоцикл", "машина", "колесо"}, 1);
            Assert.Greater(list.Count, 0);
        }

        [Test]
        public void TestNeighbours()
        {
            var nc = new NestorChronicles();
            var list = nc.Neighbours("провод", 3);
            Assert.Greater(list.Count, 0);
        }

        [Test]
        public void TestGetRecord()
        {
            var nc = new NestorChronicles();
            var record = nc.GetRecord("аббат");
            Assert.Greater(record.Best.Count, 0);
        }
    }
}