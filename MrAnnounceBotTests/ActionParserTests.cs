using NUnit.Framework;
using MrAnnouncerBot;
using System.Linq;

namespace MrAnnounceBotTests
{
    [TestFixture]
    public class ActionParserTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void A1_NullOrWhitespace_ReturnsEmpty(string input)
        {
            var result = ActionParser.ParseLines(input).ToList();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void A2_SingleLine_ReturnsOneEntry()
        {
            var result = ActionParser.ParseLines("scene: MyScene").ToList();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Key,   Is.EqualTo("scene"));
            Assert.That(result[0].Value, Is.EqualTo("MyScene"));
        }

        [Test]
        public void A3_MultiLineCRLF_ReturnsTwoEntries()
        {
            var result = ActionParser.ParseLines("scene: A\r\ndelay: 500").ToList();
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Key,   Is.EqualTo("scene"));
            Assert.That(result[1].Key,   Is.EqualTo("delay"));
            Assert.That(result[1].Value, Is.EqualTo("500"));
        }

        [Test]
        public void A4_LFOnly_ReturnsTwoEntries()
        {
            var result = ActionParser.ParseLines("scene: A\ndelay: 500").ToList();
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void A5_LineWithNoColon_IsSkipped()
        {
            var result = ActionParser.ParseLines("this has no colon\nscene: X").ToList();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Key, Is.EqualTo("scene"));
        }

        [Test]
        public void A6_KeyAndValueAreTrimmed()
        {
            var result = ActionParser.ParseLines("  scene  :  My Scene  ").ToList();
            Assert.That(result[0].Key,   Is.EqualTo("scene"));
            Assert.That(result[0].Value, Is.EqualTo("My Scene"));
        }

        [Test]
        public void A7_ValueContainsColon_FullValuePreserved()
        {
            var result = ActionParser.ParseLines("obs: SceneName, Source:Name, show").ToList();
            Assert.That(result[0].Key,   Is.EqualTo("obs"));
            Assert.That(result[0].Value, Is.EqualTo("SceneName, Source:Name, show"));
        }

        [Test]
        public void A8_BlankLinesBetweenEntries_Skipped()
        {
            var result = ActionParser.ParseLines("scene: A\n\n\ndelay: 100").ToList();
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [TestCase("SCENE")]
        [TestCase("Scene")]
        [TestCase("scene")]
        public void A9_KeyIsCaseFolded(string keyVariant)
        {
            var result = ActionParser.ParseLines($"{keyVariant}: X").ToList();
            Assert.That(result[0].Key, Is.EqualTo("scene"));
        }
    }
}
