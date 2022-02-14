using System;
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

        [Test]
        public void IambicTests()
        {
            const string line = "наряжены мы вместе город ведать";
            Foot foot = _footAnalyser.FindBestFootByLine(line, out _);
            Assert.AreEqual(FootType.Iambic, foot.Type);

            const string poem = "Ты погрусти, когда умрет поэт,\n" +
                                "Покуда звон ближайшей из церквей\n" +
                                "Не возвестит, что этот низкий свет\n" +
                                "Я променял на низший мир червей.";
            foot = _footAnalyser.FindBestFootByPoem(poem);
            Assert.AreEqual(FootType.Iambic, foot.Type);
        }
        
        [Test]
        public void ChoreaTests()
        {
            const string line = "буря мглою небо кроет";
            Foot foot = _footAnalyser.FindBestFootByLine(line, out _);
            Assert.AreEqual(FootType.Chorea, foot.Type);

            const string poem = "Ласточки пропали,\n" +
                                "А вчера зарей\n" +
                                "Все грачи летали\n" +
                                "Да, как сеть, мелькали\n" +
                                "Вот над той горой.";
            foot = _footAnalyser.FindBestFootByPoem(poem);
            Assert.AreEqual(FootType.Chorea, foot.Type);
        }
        
        [Test]
        public void DactylTests()
        {
            const string line = "тучки небесные вечные странники";
            Foot foot = _footAnalyser.FindBestFootByLine(line, out _);
            Assert.AreEqual(FootType.Dactyl, foot.Type);

            const string poem = "Я, матерь божия, ныне с молитвою\n" +
                                "Пред твоим образом, ярким сиянием,\n" +
                                "Не о спасении, не перед битвою,\n" +
                                "Не с благодарностью, иль покаянием,";
            foot = _footAnalyser.FindBestFootByPoem(poem);
            Assert.AreEqual(FootType.Dactyl, foot.Type);
        }
        
        [Test]
        public void AmphibrachiumTests()
        {
            const string line = "шумела полночная вьюга";
            Foot foot = _footAnalyser.FindBestFootByLine(line, out _);
            Assert.AreEqual(FootType.Amphibrachium, foot.Type);

            const string poem = "Я долго стоял неподвижно,\n" +
                                "В далёкие звёзды вглядясь, —\n" +
                                "Меж теми звездами и мною\n" +
                                "Какая-то связь родилась.";
            foot = _footAnalyser.FindBestFootByPoem(poem);
            Assert.AreEqual(FootType.Amphibrachium, foot.Type);
        }
        
        [Test]
        public void AnapestTests()
        {
            const string line = "прозвучало над ясной рекою";
            Foot foot = _footAnalyser.FindBestFootByLine(line, out _);
            Assert.AreEqual(FootType.Anapest, foot.Type);

            const string poem = "Есть в напевах твоих сокровенных\n" +
                                "Роковая о гибели весть.\n" +
                                "Есть проклятье заветов священных,\n" +
                                "Поругание счастия есть.";
            foot = _footAnalyser.FindBestFootByPoem(poem);
            Assert.AreEqual(FootType.Anapest, foot.Type);
        }

        [Test]
        public void ExactFormsTest()
        {
            const string line = "У лукоморья дуб зеленый";
            Foot foot = _footAnalyser.FindBestFootByLine(line, out _);
            Assert.AreEqual(FootType.Iambic, foot.Type);
        }
    }
}