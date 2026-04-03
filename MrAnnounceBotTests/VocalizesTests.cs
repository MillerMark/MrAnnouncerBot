using MrAnnouncerBot;

namespace MrAnnounceBotTests
{
    [TestFixture]
    public class VocalizesTests
    {
        // V1 — colon separator: "!mark:hello" → true
        [TestCase("!mark:hello",  "!mark")]
        [TestCase("!fred: hi",    "!fred")]
        [TestCase("!mark: ",      "!mark")]
        public void V1_ColonAfterPrefix_ReturnsTrue(string message, string prefix)
        {
            Assert.That(MrAnnouncerBot.MrAnnouncerBot.Vocalizes(message, prefix), Is.True);
        }

        // V2 — space separator: "!mark hello" → true
        [TestCase("!mark hello",   "!mark")]
        [TestCase("!campbell hi",  "!campbell")]
        [TestCase("!mark  ",       "!mark")]
        public void V2_SpaceAfterPrefix_ReturnsTrue(string message, string prefix)
        {
            Assert.That(MrAnnouncerBot.MrAnnouncerBot.Vocalizes(message, prefix), Is.True);
        }

        // V3 — message equals prefix exactly (no extra chars)
        [TestCase("!mark",     "!mark")]
        [TestCase("!fred",     "!fred")]
        [TestCase("",          "")]
        public void V3_MessageEqualsPrefix_ReturnsFalse(string message, string prefix)
        {
            Assert.That(MrAnnouncerBot.MrAnnouncerBot.Vocalizes(message, prefix), Is.False);
        }

        // V4 — message does not start with prefix
        [TestCase("hello world", "!mark")]
        [TestCase("!fred hello", "!mark")]
        [TestCase("mark: hi",    "!mark")]
        public void V4_MessageDoesNotStartWithPrefix_ReturnsFalse(string message, string prefix)
        {
            Assert.That(MrAnnouncerBot.MrAnnouncerBot.Vocalizes(message, prefix), Is.False);
        }

        // V5 — character after prefix is neither ':' nor ' '
        [TestCase("!marks hello",  "!mark")]
        [TestCase("!mark!hello",   "!mark")]
        [TestCase("!markXhello",   "!mark")]
        public void V5_InvalidSeparatorChar_ReturnsFalse(string message, string prefix)
        {
            Assert.That(MrAnnouncerBot.MrAnnouncerBot.Vocalizes(message, prefix), Is.False);
        }

        // V6 — prefix longer than message
        [TestCase("!m",    "!mark")]
        [TestCase("",      "!mark")]
        [TestCase("!mar",  "!mark")]
        public void V6_PrefixLongerThanMessage_ReturnsFalse(string message, string prefix)
        {
            Assert.That(MrAnnouncerBot.MrAnnouncerBot.Vocalizes(message, prefix), Is.False);
        }

        // V7 — empty prefix matches anything with a valid first char
        [TestCase(": hello",  "")]
        [TestCase(" hello",   "")]
        public void V7_EmptyPrefix_ValidSeparatorAsFirstChar_ReturnsTrue(string message, string prefix)
        {
            Assert.That(MrAnnouncerBot.MrAnnouncerBot.Vocalizes(message, prefix), Is.True);
        }

        // V8 — empty prefix but first char is not ':' or ' '
        [TestCase("hello",  "")]
        [TestCase("x",      "")]
        public void V8_EmptyPrefix_InvalidFirstChar_ReturnsFalse(string message, string prefix)
        {
            Assert.That(MrAnnouncerBot.MrAnnouncerBot.Vocalizes(message, prefix), Is.False);
        }
    }
}
