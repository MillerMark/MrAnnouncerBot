using MrAnnouncerBot;

namespace MrAnnounceBotTests
{
    [TestFixture]
    public class GetNameAndPhraseTests
    {
        // Helper to keep test cases concise
        private static (string name, string phrase) Parse(string msg)
        {
            MrAnnouncerBot.MrAnnouncerBot.GetNameAndPhrase(msg, out var name, out var phrase);
            return (name, phrase);
        }

        // GNP1 — space separator: name and phrase extracted correctly
        [TestCase("!mark hello",       "mark",     "hello")]
        [TestCase("!fred hi there",    "fred",     "hi there")]
        [TestCase("!campbell howdy",   "campbell", "howdy")]
        public void GNP1_SpaceSeparator_ExtractsNameAndPhrase(string msg, string expectedName, string expectedPhrase)
        {
            var (name, phrase) = Parse(msg);
            Assert.That(name,   Is.EqualTo(expectedName));
            Assert.That(phrase, Is.EqualTo(expectedPhrase));
        }

        // GNP2 — colon separator: name and phrase extracted correctly
        [TestCase("!mark:hello",       "mark", "hello")]
        [TestCase("!fred:hi there",    "fred", "hi there")]
        [TestCase("!rory:howdy",       "rory", "howdy")]
        public void GNP2_ColonSeparator_ExtractsNameAndPhrase(string msg, string expectedName, string expectedPhrase)
        {
            var (name, phrase) = Parse(msg);
            Assert.That(name,   Is.EqualTo(expectedName));
            Assert.That(phrase, Is.EqualTo(expectedPhrase));
        }

        // GNP3 — when both separators present, the first one wins
        [TestCase("!mark hello:world",  "mark", "hello:world")]  // space comes first
        [TestCase("!mark:hello world",  "mark", "hello world")]  // colon comes first
        public void GNP3_BothSeparators_FirstOneWins(string msg, string expectedName, string expectedPhrase)
        {
            var (name, phrase) = Parse(msg);
            Assert.That(name,   Is.EqualTo(expectedName));
            Assert.That(phrase, Is.EqualTo(expectedPhrase));
        }

        // GNP4 — "says: " keyword is stripped from phrase
        [TestCase("!mark says: hello",          "mark", "hello")]
        [TestCase("!fred says: hi there",       "fred", "hi there")]
        [TestCase("!mark SAYS: Hello World",    "mark", "Hello World")]  // case-insensitive
        public void GNP4_SaysKeyword_IsStripped(string msg, string expectedName, string expectedPhrase)
        {
            var (name, phrase) = Parse(msg);
            Assert.That(name,   Is.EqualTo(expectedName));
            Assert.That(phrase, Is.EqualTo(expectedPhrase));
        }

        // GNP5 — "thinks: " keyword wraps phrase in parentheses
        [TestCase("!mark thinks: hello",         "mark", "(hello)")]
        [TestCase("!fred thinks: hi there",      "fred", "(hi there)")]
        [TestCase("!mark THINKS: Hello World",   "mark", "(Hello World)")]  // case-insensitive
        public void GNP5_ThinksKeyword_WrapsInParens(string msg, string expectedName, string expectedPhrase)
        {
            var (name, phrase) = Parse(msg);
            Assert.That(name,   Is.EqualTo(expectedName));
            Assert.That(phrase, Is.EqualTo(expectedPhrase));
        }

        // GNP6 — no separator at all → both outputs are null
        [TestCase("!mark")]
        [TestCase("noexclamation")]
        [TestCase("x")]
        public void GNP6_NoSeparator_ReturnsBothNull(string msg)
        {
            var (name, phrase) = Parse(msg);
            Assert.That(name,   Is.Null);
            Assert.That(phrase, Is.Null);
        }

        // GNP7 — colon at the very end is not valid; space is used instead (if present)
        [TestCase("!mark hello:",  "mark", "hello:")]  // space first, colon at end → space wins
        public void GNP7_ColonAtEnd_SpaceUsedInstead(string msg, string expectedName, string expectedPhrase)
        {
            var (name, phrase) = Parse(msg);
            Assert.That(name,   Is.EqualTo(expectedName));
            Assert.That(phrase, Is.EqualTo(expectedPhrase));
        }

        // GNP8 — colon at the very last position only (no space) → both null
        [TestCase("!mark:")]
        public void GNP8_OnlyColonAtEnd_NoSpace_ReturnsBothNull(string msg)
        {
            var (name, phrase) = Parse(msg);
            Assert.That(name,   Is.Null);
            Assert.That(phrase, Is.Null);
        }

        // GNP9 — space at the very last position only (no colon) → both null
        [TestCase("!mark ")]
        public void GNP9_OnlySpaceAtEnd_NoColon_ReturnsBothNull(string msg)
        {
            var (name, phrase) = Parse(msg);
            Assert.That(name,   Is.Null);
            Assert.That(phrase, Is.Null);
        }

        // GNP10 — name is lowercased regardless of input casing
        [TestCase("!MARK hello",  "mark")]
        [TestCase("!Fred hi",     "fred")]
        [TestCase("!CAMPBELL x",  "campbell")]
        public void GNP10_NameIsLowercased(string msg, string expectedName)
        {
            var (name, _) = Parse(msg);
            Assert.That(name, Is.EqualTo(expectedName));
        }

        // GNP11 — whitespace around name/phrase is trimmed
        [TestCase("!mark  hello",   "mark", "hello")]  // extra space after separator trimmed from phrase
        [TestCase("!mark: hello",   "mark", "hello")]  // leading space after colon trimmed from phrase
        public void GNP11_ExtraWhitespace_IsTrimmed(string msg, string expectedName, string expectedPhrase)
        {
            var (name, phrase) = Parse(msg);
            Assert.That(name,   Is.EqualTo(expectedName));
            Assert.That(phrase, Is.EqualTo(expectedPhrase));
        }

        // GNP12 — non-keyword phrase is passed through unchanged
        [TestCase("!mark:Hello World!",  "mark", "Hello World!")]
        [TestCase("!fred hello there",   "fred", "hello there")]
        public void GNP12_PlainPhrase_PassedThroughUnchanged(string msg, string expectedName, string expectedPhrase)
        {
            var (name, phrase) = Parse(msg);
            Assert.That(name,   Is.EqualTo(expectedName));
            Assert.That(phrase, Is.EqualTo(expectedPhrase));
        }
    }
}
