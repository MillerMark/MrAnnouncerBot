using System;
using System.Collections.Generic;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class DiceRollTests
	{
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		[TestMethod]
		public void TestChaosBolt()
		{
			AllPlayers.Invalidate();
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			Character merkin = AllPlayers.GetFromName("Merkin");
			game.AddPlayer(merkin);

			List<PlayerActionShortcut> actionShortcuts = AllActionShortcuts.Get(merkin.playerID, "Chaos Bolt");
			Assert.AreEqual(3, actionShortcuts.Count);
			DiceRoll chaosBoltLevel3 = DiceRoll.GetFrom(actionShortcuts[2]);
			Assert.IsTrue(chaosBoltLevel3.IsMagic);
			Assert.AreEqual(DiceRollType.ChaosBolt, chaosBoltLevel3.Type);
			Assert.AreEqual("2d8(),3d6()", chaosBoltLevel3.DamageHealthExtraDice);
		}

		DiceRoll AssertSimpleSpell(string spellName, int spellCasterLevel, int spellCasterAbilityModifier = 0, int spellSlotLevel = -1)
		{
			Character player = PlayerHelper.GetLilCutiePaladin();
			DiceRoll roll = DiceRollHelper.GetSpellFrom(spellName, player, spellSlotLevel);
			Assert.AreEqual(DiceRollType.CastSimpleSpell, roll.Type);
			return roll;
		}

		[TestMethod]
		public void VerifySimpleSpells()
		{
			const int spellCasterLevel = 6;
			AssertSimpleSpell("Light", spellCasterLevel);
			AssertSimpleSpell("Bless", spellCasterLevel);
			AssertSimpleSpell("Command", spellCasterLevel, 0, 1);
			AssertSimpleSpell("Command", spellCasterLevel, 0, 2);
			AssertSimpleSpell("Heroism", spellCasterLevel);
			AssertSimpleSpell("Aid", spellCasterLevel);
			AssertSimpleSpell("Enhance Ability", spellCasterLevel);
			AssertSimpleSpell("Enthrall", spellCasterLevel);
			AssertSimpleSpell("Lesser Restoration", spellCasterLevel);
			AssertSimpleSpell("Zone of Truth", spellCasterLevel);
		}

		[TestMethod]
		public void When_CureWounds_is_cast_then_roll_1d8_plus_4_healing()
		{
			Character player = PlayerHelper.GetLilCutiePaladin();
			DiceRoll roll = DiceRollHelper.GetSpellFrom("Cure Wounds", player, 1);
			Assert.AreEqual(DiceRollType.HealthOnly, roll.Type);
			Assert.AreEqual("1d8+4(healing)", roll.DamageHealthExtraDice);
		}

		void AssertRoll(string spellName, Character player, int spellSlotLevel, DiceRollType expectedDiceRollType, string expectedDice)
		{
			DiceRoll roll = DiceRollHelper.GetSpellFrom(spellName, player, spellSlotLevel);
			Assert.AreEqual(expectedDiceRollType, roll.Type);
			Assert.AreEqual(expectedDice, roll.DamageHealthExtraDice);
		}

		[TestMethod]
		public void TestLilCutieSpells()
		{
			Character player = PlayerHelper.GetLilCutiePaladin();
			AssertRoll("Cure Wounds", player, 1, DiceRollType.HealthOnly, "1d8+4(healing)");
			AssertRoll("Cure Wounds", player, 2, DiceRollType.HealthOnly, "2d8+4(healing)");
			AssertRoll("Guiding Bolt", player, 1, DiceRollType.Attack, "4d6(radiant)");
			AssertRoll("Guiding Bolt", player, 2, DiceRollType.Attack, "5d6(radiant)");
		}

		[TestMethod]
		public void TestDivineSmite()
		{
			Character player = PlayerHelper.GetLilCutiePaladin();
			AssertRoll("Divine Smite", player, 1, DiceRollType.DamageOnly, "1d8(radiant)");
			AssertRoll("Divine Smite", player, 2, DiceRollType.DamageOnly, "2d8(radiant)");
		}

	}
}
