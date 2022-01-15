using System.Linq;
using Nestor;
using Nestor.Models;
using NUnit.Framework;

namespace NestorTests
{
    [TestFixture]
    public class Tests
    {
        private NestorMorph _nMorph;
        
        [SetUp]
        public void SetUp()
        {
            _nMorph ??= new NestorMorph();
        }
        
        [Test]
        public void TestTokenize()
        {
            string[] tokens = _nMorph.Tokenize("Пришёл, увидел, победил. 123-45, 67//# Как-то раз.");
            Assert.AreEqual(5, tokens.Length);
            Assert.AreEqual("пришёл", tokens[0]);
            Assert.AreEqual("как-то", tokens[3]);

            string[] tokensNum = _nMorph.Tokenize("В 140 солнц закат пылал.", MorphOption.KeepNumbers);
            Assert.AreEqual(5, tokensNum.Length);
            Assert.AreEqual("140", tokensNum[1]);

            string[] tokensPrepositions = _nMorph.Tokenize("Под о ш в а", MorphOption.RemoveServicePos);
            Assert.AreEqual(1, tokensPrepositions.Length);
            Assert.AreEqual("ш", tokensPrepositions[0]);

            string[] tokensExistent = _nMorph.Tokenize(
                "Съешь ещё этих бурдылек и выпей куздру",
                MorphOption.RemoveNonExistent
            );
            Assert.AreEqual(5, tokensExistent.Length);
            Assert.False(tokensExistent.Contains("бурдылек"));
        }

        [Test]
        public void TestLemmatize()
        {
            string[] lemmas = _nMorph.Lemmatize("Кошки стали бурлеть в округе");
            Assert.AreEqual(5, lemmas.Length);
            Assert.True(lemmas.Contains("кошка"));

            string[] lemmasFull = _nMorph.Lemmatize(
                "Кошки стали бурлеть в округе",
                MorphOption.InsertAllLemmas | MorphOption.Distinct
            );
            Assert.AreEqual(7, lemmasFull.Length);
            Assert.True(lemmasFull.Contains("сталь"));
            Assert.True(lemmasFull.Contains("стать"));
            Assert.True(lemmasFull.Contains("округ"));
            Assert.True(lemmasFull.Contains("округа"));
        }

        [Test]
        public void TestRemovePrepositions()
        {
            const string phrase = "чай с вареньем и мятой в тени со светом";
            string[] tokens = _nMorph.Tokenize(phrase, MorphOption.RemoveServicePos);

            Assert.False(tokens.Contains("с"));
            Assert.False(tokens.Contains("со"));
            Assert.False(tokens.Contains("и"));
            Assert.False(tokens.Contains("в"));

            string[] lemmas = _nMorph.Lemmatize(
                phrase,
                MorphOption.RemoveServicePos | MorphOption.InsertAllLemmas | MorphOption.Distinct
            );
            
            Assert.False(lemmas.Contains("с"));
            Assert.False(lemmas.Contains("со"));
            Assert.False(lemmas.Contains("и"));
            Assert.False(lemmas.Contains("в"));
        }

        [Test]
        public void TestMultiAccent()
        {
            const string word = "пустынных";
            Word[] info = _nMorph.WordInfo(word);
            Assert.AreEqual(1, info.Length);

            WordForm[] forms = info.First().ExactForms(word);
            int[] accents = forms.Select(f => f.Stress).Distinct().ToArray();
            
            Assert.AreEqual(2, accents.Length);
        }
        
        [Test]
        public void TestAbsentMainForm()
        {
            const string word = "некому";
            Word[] info = _nMorph.WordInfo(word);
            Assert.AreEqual(1, info.Length);

            WordForm[] forms = info[0].ExactForms(word);
            Assert.AreEqual(1, forms.Length);

            WordForm form = forms[0];
            Assert.AreEqual(1, form.Stress);
        }

        [Test]
        public void TestWordInfo()
        {
            const string word = "стали";
            Word[] info = _nMorph.WordInfo(word);
            Assert.AreEqual(2, info.Length);

            // first
            Word first = info.SingleOrDefault(w => w.Lemma.Word == "сталь");
            Assert.IsNotNull(first);
            
            Assert.IsTrue(first.Tag.Pos == Pos.Noun);
            Assert.IsTrue(first.Tag.Gender == Gender.Feminine);
            
            WordForm[] firstForms = first.ExactForms(word);
            Assert.IsTrue(firstForms.Any(f => f.Tag.Number == Number.Plural));
            Assert.IsTrue(firstForms.Any(f => f.Tag.Case == Case.Genitive && f.Tag.Number == Number.Singular));
            Assert.IsTrue(firstForms.Any(f => f.Tag.Case == Case.Accusative && f.Tag.Number == Number.Plural));
            Assert.IsTrue(firstForms.Any(f => f.Tag.Case == Case.Dative && f.Tag.Number == Number.Singular));
            Assert.IsTrue(firstForms.Any(f => f.Tag.Case == Case.Prepositional && f.Tag.Number == Number.Singular));
            
            // second
            Word second = info.SingleOrDefault(w => w.Lemma.Word == "стать");
            Assert.IsNotNull(second);
            
            Assert.IsTrue(second.Tag.Pos == Pos.Verb);

            WordForm[] secondsForms = second.ExactForms(word);
            Assert.IsTrue(secondsForms.Any(f => f.Tag.Tense == Tense.Past));
            Assert.IsTrue(secondsForms.Any(f => f.Tag.Number == Number.Plural));
        }

        [Test]
        public void TestFindForm()
        {
            const string w1 = "красивый";
            Word[] info1 = _nMorph.WordInfo(w1);
            Assert.GreaterOrEqual(1, info1.Length);

            Word first = info1[0];
            WordForm f1 = first.ClosestForm(gender: Gender.Feminine, Case.Nominative, Number.Singular);
            Assert.IsNotNull(f1);
            Assert.AreEqual("красивая", f1.Word);

            WordForm f2 = first.ClosestForm(Gender.Feminine, Case.Accusative, Number.Singular);
            Assert.IsNotNull(f2);
            Assert.AreEqual("красивую", f2.Word);

            WordForm f3 = first.ClosestForm(Gender.None, Case.Genitive, Number.Plural);
            Assert.IsNotNull(f3);
            Assert.AreEqual("красивых", f3.Word);

            const string w2 = "красить";
            Word[] info2 = _nMorph.WordInfo(w2);
            Assert.GreaterOrEqual(1, info2.Length);

            Word second = info2.FirstOrDefault(i => i.Tag.Pos == Pos.Verb);
            Assert.IsNotNull(second);

            WordForm s1 = second.ClosestForm(gender: Gender.Feminine, number: Number.Singular, tense: Tense.Past);
            Assert.IsNotNull(s1);
            Assert.AreEqual("красила", s1.Word);
        }

        [Test]
        public void TestAccentIndex()
        {
            Word[] info = _nMorph.WordInfo("трансформатор");
            Assert.AreEqual(1, info.Length);

            Word first = info.First();
            int index = first.Lemma.GetStressIndex();
            
            Assert.AreEqual(9, index);
        }

        [TearDown]
        public void Dispose()
        {
            _nMorph = null;
        }
    }
}