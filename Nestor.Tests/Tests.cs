using Nestor;
using NUnit.Framework;

namespace NestorTests
{
    public class Tests
    {

        [Test]
        public void TestComplexPhrase()
        {
            var nestor = new NestorMorph();
            
            var case1 = nestor.CheckPhrase("привет меня зовут Иван мне 40 лет и я молодец", true,  
                "выход",
                "выйти",
                "выходить",
                "закрыть",
                "закрывать",
                "хватит",
                "хватить",
                "конец",
                "закончить",
                "заканчивать"
            );
            
            Assert.AreEqual(false, case1);
        }
        
        [Test]
        public void TestDefaultDifference()
        {
            var nestor = new NestorMorph();
            
            var case1 = nestor.CheckPhrase("я иду гулять", false,  "идти гулять");
            Assert.AreEqual(true, case1);
            
            var case2 = nestor.CheckPhrase("я сейчас иду гулять", false,  "идти гулять");
            Assert.AreEqual(false, case2);
            
            var case3 = nestor.CheckPhrase("я очень сейчас иду гулять", false,  "я идти гулять");
            Assert.AreEqual(true, case3);
            var case4 = nestor.CheckPhrase("я очень сильно сейчас иду гулять", false,  "идти гулять");
            Assert.AreEqual(false, case4);
        }
        
        [Test]
        public void TestMaxDifference()
        {
            var nestor = new NestorMorph();
            
            var case1 = nestor.CheckPhrase("я иду гулять",1, false,  "идти гулять");
            Assert.AreEqual(true, case1);
            
            var case2 = nestor.CheckPhrase("я иду гулять",0, false,  "идти гулять");
            Assert.AreEqual(false, case2);
        }
        
        [Test]
        public void TestHasLemmas()
        {
            var nestor = new NestorMorph();
            
            var case1 = nestor.HasOneOfLemmas("стали", "сталь", "стать");
            Assert.AreEqual(true, case1);
            
            var case2 = nestor.HasOneOfLemmas("душе", "душ", "душа");
            Assert.AreEqual(true, case2);
        }
        
        [Test]
        public void TestHasLemma()
        {
            var nestor = new NestorMorph();
            
            var case1 = nestor.HasLemma("попугаи", "попугай");
            Assert.AreEqual(true, case1);
            
            var case2 = nestor.HasLemma("ежи", "ёж");
            Assert.AreEqual(true, case2);
            
            var case3 = nestor.HasLemma("стали", "сталь");
            Assert.AreEqual(true, case3);
            
            var case4 = nestor.HasLemma("стали", "стать");
            Assert.AreEqual(true, case4);
        }
    }
}