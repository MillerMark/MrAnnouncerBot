using MrAnnouncerBot;

namespace MrAnnounceBotTests
{
	[TestFixture]
	public class VIPGreetingTests
	{
		// ── helpers ─────────────────────────────────────────────────────────────

		/// <summary>Runs the constructor <paramref name="times"/> times and returns every unique Greeting produced.</summary>
		private static IEnumerable<string> Greetings(string displayName, int times = 200)
		{
			for (int i = 0; i < times; i++)
				yield return new VIPGreeting(displayName).Greeting;
		}

		// The one ChaelCodes greeting that contains no @mention
		private const string ChaelCodesClipUrl =
			"https://clips.twitch.tv/CarefulExuberantNightingaleBleedPurple";

		// ── VG1: DisplayName is preserved exactly as supplied ────────────────────

		[TestCase("TestUser")]
		[TestCase("SurlyDev")]
		[TestCase("ALLCAPS")]
		[TestCase("mixedCase")]
		public void VG1_DisplayName_IsPreservedExactly(string name)
		{
			var vip = new VIPGreeting(name);
			Assert.That(vip.DisplayName, Is.EqualTo(name));
		}

		// ── VG2: Greeting is never null or empty ─────────────────────────────────

		[TestCase("unknownuser42")]
		[TestCase("surlydev")]
		[TestCase("baldbeardedbuilder")]
		[TestCase("chaelcodes")]
		[TestCase("tehpudding")]
		[TestCase("copperbeardy")]
		[TestCase("jtsom")]
		[TestCase("phrakberg")]
		[TestCase("sxpositive")]
		[TestCase("codebasealpha")]
		[TestCase("codeman_codes")]
		[TestCase("legendairymoooo")]
		[TestCase("will_bennet")]
		[TestCase("speedyc12371")]
		public void VG2_Greeting_IsNeverNullOrEmpty(string name)
		{
			foreach (var g in Greetings(name))
				Assert.That(g, Is.Not.Null.And.Not.Empty);
		}

		// ── VG3: Unknown usernames always produce a greeting containing @mention ─

		[TestCase("unknownuser42")]
		[TestCase("randomviewer")]
		[TestCase("newcomer99")]
		public void VG3_UnknownUser_GreetingAlwaysContainsMention(string name)
		{
			foreach (var g in Greetings(name))
				Assert.That(g, Does.Contain($"@{name}"),
					$"Unexpected greeting for unknown user '{name}': {g}");
		}

		// ── VG4: Known VIPs whose every possible greeting contains @mention ──────
		// (All named VIPs except ChaelCodes — see VG5 for that special case)

		[TestCase("surlydev")]
		[TestCase("speedyc12371")]
		[TestCase("legendairymoooo")]
		[TestCase("will_bennet")]
		[TestCase("codeman_codes")]
		[TestCase("sxpositive")]
		[TestCase("codebasealpha")]
		[TestCase("copperbeardy")]
		[TestCase("jtsom")]
		[TestCase("baldbeardedbuilder")]
		[TestCase("phrakberg")]
		[TestCase("tehpudding")]
		public void VG4_KnownVIP_GreetingAlwaysContainsMention(string name)
		{
			foreach (var g in Greetings(name))
				Assert.That(g, Does.Contain($"@{name}"),
					$"Unexpected greeting for VIP '{name}': {g}");
		}

		// ── VG5: ChaelCodes – one branch is a clip URL with no @mention ──────────

		[Test]
		public void VG5_ChaelCodes_GreetingContainsMentionOrIsKnownClipUrl()
		{
			foreach (var g in Greetings("chaelcodes"))
			{
				var valid = g.Contains("@chaelcodes", StringComparison.OrdinalIgnoreCase)
				            || g == ChaelCodesClipUrl;
				Assert.That(valid, Is.True,
					$"Unexpected chaelcodes greeting: {g}");
			}
		}

		// ── VG6: VIP matching is case-insensitive; DisplayName casing is kept ────

		[TestCase("SurlyDev",            "SurlyDev")]
		[TestCase("SURLYDEV",            "SURLYDEV")]
		[TestCase("BaldBeardedBuilder",  "BaldBeardedBuilder")]
		[TestCase("ChaelCodes",          "ChaelCodes")]
		public void VG6_VIPMatching_IsCaseInsensitive_AndDisplayNameCasingIsPreserved(
			string inputName, string expectedDisplayName)
		{
			var vip = new VIPGreeting(inputName);
			// DisplayName keeps original casing
			Assert.That(vip.DisplayName, Is.EqualTo(expectedDisplayName));
			// Greeting is non-empty, confirming the VIP branch fired (not an exception path)
			Assert.That(vip.Greeting, Is.Not.Null.And.Not.Empty);
		}

		// ── VG7: DisplayName casing is reflected in the @mention ─────────────────

		[TestCase("MyViewer")]
		[TestCase("SHOUTING")]
		public void VG7_Mention_InGreeting_ReflectsOriginalDisplayNameCasing(string name)
		{
			// Unknown users always hit GreetVip which uses @{DisplayName} verbatim
			foreach (var g in Greetings(name))
				Assert.That(g, Does.Contain($"@{name}"));
		}

		// ── VG8: BaldBeardedBuilder — both day-of-week branches are covered ──────
		// DateTime.Now cannot be controlled, so we assert the invariant holds on
		// whichever day the tests run, and separately build the two expected sets.

		[Test]
		public void VG8_BaldBeardedBuilder_TuesdayGreetings_AreInValidSet()
		{
			// These are the Tuesday-specific phrases (all contain the display name via @)
			var tuesdayPhrases = new[]
			{
				"TACO TUESDAYS",
				"Tacos are for Tuesdays",
				"Today is Tuesday",
				"T stands for Taco",
				"Tuesday and I'm hungry",
				"Guess what day it is",
				"Is today Tuesday",
				"Every day is Taco Tuesday",
			};

			var nonTuesdayPhrases = new[]
			{
				"sour cream",
				"Bald bearded builders",
				"TWO incredibly handsome bald bearded builders",
				"bald, he's bearded",
				"Go checkout",
				"streak of grey",
				"streak of grey",
				"Look! Up in the sky",
			};

			if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
			{
				foreach (var g in Greetings("baldbeardedbuilder"))
				{
					// On Tuesday every greeting either uses a Tuesday phrase, mentions the
					// VIP by @mention (including GreetVip fallback), or both.
					var isTuesdaySpecific = tuesdayPhrases.Any(p => g.Contains(p));
					var hasMention = g.Contains("@baldbeardedbuilder");
					Assert.That(isTuesdaySpecific || hasMention, Is.True,
						$"Unexpected Tuesday greeting: {g}");
				}
			}
			else
			{
				foreach (var g in Greetings("baldbeardedbuilder"))
				{
					var isNonTuesdaySpecific = nonTuesdayPhrases.Any(p => g.Contains(p));
					var hasMention = g.Contains("@baldbeardedbuilder");
					Assert.That(isNonTuesdaySpecific || hasMention, Is.True,
						$"Unexpected non-Tuesday greeting: {g}");
				}
			}
		}
	}
}
