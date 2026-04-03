using CommonCore;

namespace MrAnnounceBotTests
{
    [TestFixture]
    public class StrExtensionsTests
    {
        // ──────────────────────────────────────────────────────────────
        // ToDouble
        // ──────────────────────────────────────────────────────────────

        [TestCase("3.14",   3.14)]
        [TestCase("0",      0.0)]
        [TestCase("-7.5",  -7.5)]
        [TestCase("  42 ", 42.0)]
        public void ToDouble_ValidInput_ReturnsExpected(string input, double expected)
            => Assert.That(input.ToDouble(), Is.EqualTo(expected).Within(1e-10));

        [TestCase("abc")]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("1.2.3")]
        public void ToDouble_InvalidInput_ReturnsZero(string input)
            => Assert.That(input.ToDouble(), Is.EqualTo(0.0));

        // ──────────────────────────────────────────────────────────────
        // ToDecimal
        // ──────────────────────────────────────────────────────────────

        [TestCase("9.99",  9.99)]
        [TestCase("0",     0.0)]
        [TestCase("-1.5", -1.5)]
        [TestCase("  10 ", 10.0)]
        public void ToDecimal_ValidInput_ReturnsExpected(string input, double expectedAsDouble)
            => Assert.That(input.ToDecimal(), Is.EqualTo((decimal)expectedAsDouble));

        [TestCase("xyz")]
        [TestCase("")]
        [TestCase("1.2.3")]
        public void ToDecimal_InvalidInput_ReturnsZero(string input)
            => Assert.That(input.ToDecimal(), Is.EqualTo(0m));

        // ──────────────────────────────────────────────────────────────
        // ToInt
        // ──────────────────────────────────────────────────────────────

        [TestCase("5",    0, 5)]
        [TestCase("-3",   0, -3)]
        [TestCase("  7 ", 0, 7)]
        public void ToInt_ValidInput_ReturnsExpected(string input, int defaultValue, int expected)
            => Assert.That(input.ToInt(defaultValue), Is.EqualTo(expected));

        [TestCase("abc",  0,   0)]
        [TestCase("abc",  99, 99)]
        [TestCase("",     0,   0)]
        [TestCase("1.5",  0,   0)]   // decimal string is not a valid int
        public void ToInt_InvalidInput_ReturnsDefault(string input, int defaultValue, int expected)
            => Assert.That(input.ToInt(defaultValue), Is.EqualTo(expected));

        [Test]
        public void ToInt_DefaultParamIsZero()
            => Assert.That("bad".ToInt(), Is.EqualTo(0));

        // ──────────────────────────────────────────────────────────────
        // GetFirstDouble
        // ──────────────────────────────────────────────────────────────

        [Test]
        public void GetFirstDouble_PlainNumber_ReturnsIt()
            => Assert.That("42".GetFirstDouble(), Is.EqualTo(42.0));

        [Test]
        public void GetFirstDouble_EmbeddedInText_ExtractsFirst()
            => Assert.That("abc 3.14 xyz".GetFirstDouble(), Is.EqualTo(3.14).Within(1e-10));

        [Test]
        public void GetFirstDouble_NegativeEmbedded_ReturnsNegative()
            => Assert.That("price: -9.99 dollars".GetFirstDouble(), Is.EqualTo(-9.99).Within(1e-10));

        [Test]
        public void GetFirstDouble_AllowDecimalsFalse_ReturnsTruncatedInt()
            => Assert.That("3.99".GetFirstDouble(allowDecimals: false), Is.EqualTo(3.0));

        [Test]
        public void GetFirstDouble_NoDigit_ReturnsDefault()
            => Assert.That("no digits here".GetFirstDouble(defaultValue: -1.0), Is.EqualTo(-1.0));

        [Test]
        public void GetFirstDouble_NoDigit_DefaultIsZero()
            => Assert.That("abc".GetFirstDouble(), Is.EqualTo(0.0));

        [Test]
        public void GetFirstDouble_NegativeInteger_ReturnsNegative()
            => Assert.That("val -5 end".GetFirstDouble(), Is.EqualTo(-5.0));

        [Test]
        public void GetFirstDouble_LeadingZeroDecimal_ReturnsCorrectly()
            => Assert.That("0.5".GetFirstDouble(), Is.EqualTo(0.5).Within(1e-10));

        [Test]
        public void GetFirstDouble_MultipleNumbers_ReturnsFirst()
            => Assert.That("10 20 30".GetFirstDouble(), Is.EqualTo(10.0));

        // ──────────────────────────────────────────────────────────────
        // GetFirstInt
        // ──────────────────────────────────────────────────────────────

        [Test]
        public void GetFirstInt_EmbeddedNumber_ReturnsFloor()
            => Assert.That("level 7!".GetFirstInt(), Is.EqualTo(7));

        [Test]
        public void GetFirstInt_DecimalInInput_Truncates()
            => Assert.That("3.99".GetFirstInt(), Is.EqualTo(3));

        [Test]
        public void GetFirstInt_NoDigit_ReturnsDefault()
            => Assert.That("nope".GetFirstInt(defaultValue: -99), Is.EqualTo(-99));

        [Test]
        public void GetFirstInt_NegativeNumber_ReturnsNegative()
            => Assert.That("score -5".GetFirstInt(), Is.EqualTo(-5));

        // ──────────────────────────────────────────────────────────────
        // SameLetters
        // ──────────────────────────────────────────────────────────────

        [TestCase("hello", "hello", true)]
        [TestCase("Hello", "hello", true)]
        [TestCase("HELLO", "hello", true)]
        [TestCase("hello", "world", false)]
        [TestCase("",      "",      true)]
        [TestCase("abc",   "abcd",  false)]
        public void SameLetters_VariousCases(string a, string b, bool expected)
            => Assert.That(a.SameLetters(b), Is.EqualTo(expected));

        // ──────────────────────────────────────────────────────────────
        // EverythingAfter
        // ──────────────────────────────────────────────────────────────

        [Test]
        public void EverythingAfter_MatchExists_ReturnsSubstringAfter()
            => Assert.That("hello world".EverythingAfter("hello "), Is.EqualTo("world"));

        [Test]
        public void EverythingAfter_NoMatch_ReturnsEmpty()
            => Assert.That("hello".EverythingAfter("xyz"), Is.EqualTo(string.Empty));

        [Test]
        public void EverythingAfter_MultipleOccurrences_UsesFirstOccurrence()
            => Assert.That("a:b:c".EverythingAfter(":"), Is.EqualTo("b:c"));

        [Test]
        public void EverythingAfter_MatchAtEnd_ReturnsEmpty()
            => Assert.That("hello:".EverythingAfter(":"), Is.EqualTo(string.Empty));

        // ──────────────────────────────────────────────────────────────
        // EverythingAfterLast
        // ──────────────────────────────────────────────────────────────

        [Test]
        public void EverythingAfterLast_MultipleOccurrences_UsesLastOccurrence()
            => Assert.That("a:b:c".EverythingAfterLast(":"), Is.EqualTo("c"));

        [Test]
        public void EverythingAfterLast_NoMatch_ReturnsEmpty()
            => Assert.That("hello".EverythingAfterLast("xyz"), Is.EqualTo(string.Empty));

        [Test]
        public void EverythingAfterLast_SingleOccurrence_SameAsEverythingAfter()
            => Assert.That("hello world".EverythingAfterLast(" "), Is.EqualTo("world"));

        // ──────────────────────────────────────────────────────────────
        // EverythingBefore
        // ──────────────────────────────────────────────────────────────

        [Test]
        public void EverythingBefore_MatchExists_ReturnsSubstringBefore()
            => Assert.That("hello world".EverythingBefore(" "), Is.EqualTo("hello"));

        [Test]
        public void EverythingBefore_NoMatch_ReturnsEmpty()
            => Assert.That("hello".EverythingBefore("xyz"), Is.EqualTo(string.Empty));

        [Test]
        public void EverythingBefore_MultipleOccurrences_UsesFirstOccurrence()
            => Assert.That("a:b:c".EverythingBefore(":"), Is.EqualTo("a"));

        [Test]
        public void EverythingBefore_NullInput_ReturnsEmpty()
            => Assert.That(((string)null!).EverythingBefore(":"), Is.EqualTo(string.Empty));

        // ──────────────────────────────────────────────────────────────
        // EverythingBeforeLast
        // ──────────────────────────────────────────────────────────────

        [Test]
        public void EverythingBeforeLast_MultipleOccurrences_UsesLastOccurrence()
            => Assert.That("a:b:c".EverythingBeforeLast(":"), Is.EqualTo("a:b"));

        [Test]
        public void EverythingBeforeLast_NoMatch_ReturnsEmpty()
            => Assert.That("hello".EverythingBeforeLast("xyz"), Is.EqualTo(string.Empty));

        [Test]
        public void EverythingBeforeLast_NullInput_ReturnsEmpty()
            => Assert.That(((string)null!).EverythingBeforeLast(":"), Is.EqualTo(string.Empty));

        // ──────────────────────────────────────────────────────────────
        // EverythingBetween vs EverythingBetweenNarrow
        // Uses nested delimiters: "[outer [inner] outer]"
        // ──────────────────────────────────────────────────────────────

        // EverythingBetween: EverythingAfter(begin).EverythingBeforeLast(end) → wide/outermost
        [Test]
        public void EverythingBetween_NestedDelimiters_ReturnsOutermostContent()
        {
            // EverythingAfter("[") → "outer [inner] outer]"
            // .EverythingBeforeLast("]") → "outer [inner] outer"
            var result = "[outer [inner] outer]".EverythingBetween("[", "]");
            Assert.That(result, Is.EqualTo("outer [inner] outer"));
        }

        // EverythingBetweenNarrow: EverythingAfterLast(begin).EverythingBefore(end) → narrow/innermost
        [Test]
        public void EverythingBetweenNarrow_NestedDelimiters_ReturnsInnermostContent()
        {
            // EverythingAfterLast("[") → "inner] outer]"
            // .EverythingBefore("]") → "inner"
            var result = "[outer [inner] outer]".EverythingBetweenNarrow("[", "]");
            Assert.That(result, Is.EqualTo("inner"));
        }

        [Test]
        public void EverythingBetween_NoBeginMatch_ReturnsEmpty()
            => Assert.That("hello world".EverythingBetween("[", "]"), Is.EqualTo(string.Empty));

        [Test]
        public void EverythingBetween_NoEndMatch_ReturnsEmpty()
            => Assert.That("[hello world".EverythingBetween("[", "]"), Is.EqualTo(string.Empty));

        [Test]
        public void EverythingBetween_SimpleCase_ReturnsContent()
            => Assert.That("[content]".EverythingBetween("[", "]"), Is.EqualTo("content"));

        [Test]
        public void EverythingBetweenNarrow_SimpleCase_ReturnsContent()
            => Assert.That("[content]".EverythingBetweenNarrow("[", "]"), Is.EqualTo("content"));

        [Test]
        public void EverythingBetween_MultipleNonNested_ReturnsFullSpan()
        {
            // "[a][b]" → EverythingAfter("[")="a][b]", EverythingBeforeLast("]")="a][b"  → "a][b"
            var result = "[a][b]".EverythingBetween("[", "]");
            Assert.That(result, Is.EqualTo("a][b"));
        }

        [Test]
        public void EverythingBetweenNarrow_MultipleNonNested_ReturnsLastSegment()
        {
            // "[a][b]" → EverythingAfterLast("[")="b]", EverythingBefore("]")="b" → "b"
            var result = "[a][b]".EverythingBetweenNarrow("[", "]");
            Assert.That(result, Is.EqualTo("b"));
        }

        // ──────────────────────────────────────────────────────────────
        // Has
        // ──────────────────────────────────────────────────────────────

        [TestCase("hello world", "world",  true)]
        [TestCase("hello world", "xyz",    false)]
        [TestCase("hello world", "",       true)]
        [TestCase("",            "x",      false)]
        public void Has_VariousCases(string str, string match, bool expected)
            => Assert.That(str.Has(match), Is.EqualTo(expected));

        [Test]
        public void Has_CaseSensitive_ReturnsFalseForWrongCase()
            => Assert.That("Hello".Has("hello"), Is.False);

        // ──────────────────────────────────────────────────────────────
        // HasSomething
        // ──────────────────────────────────────────────────────────────

        [TestCase("hello",  true)]
        [TestCase("  hi  ", true)]
        [TestCase("",       false)]
        [TestCase("   ",    false)]
        [TestCase(null,     false)]
        public void HasSomething_VariousCases(string? input, bool expected)
            => Assert.That(input.HasSomething(), Is.EqualTo(expected));

        // ──────────────────────────────────────────────────────────────
        // InitialCap
        // ──────────────────────────────────────────────────────────────

        [Test]
        public void InitialCap_LowerCase_CapitalizesFirst()
            => Assert.That("hello world".InitialCap(), Is.EqualTo("Hello world"));

        [Test]
        public void InitialCap_AlreadyCapped_Unchanged()
            => Assert.That("Hello".InitialCap(), Is.EqualTo("Hello"));

        [Test]
        public void InitialCap_SingleChar_Capitalized()
            => Assert.That("a".InitialCap(), Is.EqualTo("A"));

        [Test]
        public void InitialCap_EmptyString_ReturnsEmpty()
            => Assert.That(string.Empty.InitialCap(), Is.EqualTo(string.Empty));

        [Test]
        public void InitialCap_NullInput_ReturnsEmpty()
            => Assert.That(((string)null!).InitialCap(), Is.EqualTo(string.Empty));

        [Test]
        public void InitialCap_AllCaps_OnlyFirstLetterAffected()
            => Assert.That("hELLO".InitialCap(), Is.EqualTo("HELLO"));
    }
}
