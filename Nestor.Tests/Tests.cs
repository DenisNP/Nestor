using System.Linq;
using Nestor;
using NUnit.Framework;

namespace NestorTests
{
    public class Tests
    {
        [Test]
        public void TestHyphen()
        {
            var nestor = new NestorMorph();
            var tokens = nestor.Lemmatize("Привет как-нибудь как дела").ToArray();
            Assert.AreEqual(4, tokens.Length);
        }

        [Test]
        public void TestNonCyrillic()
        {
            var nestor = new NestorMorph();
            var tokens = nestor.Lemmatize("Нажал кабаны, on 102 ++ -/  баклажана!").ToArray();
            Assert.AreEqual(5, tokens.Length);
            Assert.AreEqual("нажать", tokens[0]);
            Assert.AreEqual("кабан", tokens[1]);
            Assert.AreEqual("on", tokens[2]);
            Assert.AreEqual("102", tokens[3]);
            Assert.AreEqual("баклажан", tokens[4]);
        }

        [Test]
        public void TestLemmatize()
        {
            var nestor = new NestorMorph();
            var tokens = nestor.Lemmatize("Нажал кабаны, на бурдыльку, баклажана").ToArray();
            Assert.AreEqual(5, tokens.Length);
            Assert.AreEqual("нажать", tokens[0]);
            Assert.AreEqual("кабан", tokens[1]);
            Assert.AreEqual("на", tokens[2]);
            Assert.AreEqual("бурдыльку", tokens[3]);
            Assert.AreEqual("баклажан", tokens[4]);

            var tokens2 = nestor.Lemmatize("Нажал кабаны, на бурдыльку, баклажана", true).ToArray();
            Assert.AreEqual(4, tokens2.Length);
            Assert.AreEqual("нажать", tokens2[0]);
            Assert.AreEqual("кабан", tokens2[1]);
            Assert.AreEqual("на", tokens2[2]);
            Assert.AreEqual("баклажан", tokens2[3]);
        }

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
        public void TestOneWordPhrase()
        {
            var nestor = new NestorMorph();
            var case1 = nestor.CheckPhrase("помощь", true,  
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