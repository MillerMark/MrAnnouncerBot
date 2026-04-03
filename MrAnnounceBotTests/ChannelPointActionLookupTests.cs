using NUnit.Framework;
using MrAnnouncerBot;
using System.Collections.Generic;

namespace MrAnnounceBotTests
{
    [TestFixture]
    public class ChannelPointActionLookupTests
    {
        private static ChannelPointAction Make(string id, string title) =>
            new ChannelPointAction { ID = id, Title = title };

        [Test]
        public void C1_ExactIdMatch_ReturnsCorrectAction()
        {
            var list = new List<ChannelPointAction> { Make("abc-123", "Scene A"), Make("xyz-456", "Scene B") };
            var result = ChannelPointActionLookup.Find(list, "abc-123", null);
            Assert.That(result.ID, Is.EqualTo("abc-123"));
        }

        [Test]
        public void C2_IdNotFound_FallsBackToTitleMatch()
        {
            var list = new List<ChannelPointAction> { Make("other-id", "Cool Scene") };
            var result = ChannelPointActionLookup.Find(list, "unknown", "Cool Scene");
            Assert.That(result.Title, Is.EqualTo("Cool Scene"));
        }

        [Test]
        public void C3_TitleMatchIsCaseInsensitive()
        {
            var list = new List<ChannelPointAction> { Make("id1", "Cool Scene") };
            var result = ChannelPointActionLookup.Find(list, "no-match", "cool scene");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Cool Scene"));
        }

        [Test]
        public void C4_NullId_SkipsIdSearch_FallsBackToTitle()
        {
            var list = new List<ChannelPointAction> { Make("id1", "My Scene") };
            var result = ChannelPointActionLookup.Find(list, null, "My Scene");
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void C5_EmptyStringId_SkipsIdSearch()
        {
            var list = new List<ChannelPointAction> { Make("id1", "My Scene") };
            var result = ChannelPointActionLookup.Find(list, "", "My Scene");
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void C6_WhitespaceId_SkipsIdSearch()
        {
            var list = new List<ChannelPointAction> { Make("id1", "My Scene") };
            var result = ChannelPointActionLookup.Find(list, "   ", "My Scene");
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void C7_IdAndTitleBothMatch_IdWins()
        {
            var byId    = Make("target-id", "Other Title");
            var byTitle = Make("other-id",  "Target Title");
            var list = new List<ChannelPointAction> { byId, byTitle };
            var result = ChannelPointActionLookup.Find(list, "target-id", "Target Title");
            Assert.That(result, Is.SameAs(byId));
        }

        [Test]
        public void C8_NeitherIdNorTitleMatch_ReturnsNull()
        {
            var list = new List<ChannelPointAction> { Make("id1", "Scene A") };
            var result = ChannelPointActionLookup.Find(list, "no-match", "No Match");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void C9_EmptyList_ReturnsNull()
        {
            var result = ChannelPointActionLookup.Find(new List<ChannelPointAction>(), "id", "title");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void C10_NullIdAndNullTitle_ReturnsNull()
        {
            var list = new List<ChannelPointAction> { Make("id1", "Scene A") };
            var result = ChannelPointActionLookup.Find(list, null, null);
            Assert.That(result, Is.Null);
        }
    }
}
