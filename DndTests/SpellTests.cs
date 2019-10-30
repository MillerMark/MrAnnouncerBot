using System;
using System.Collections.Generic;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class SpellTests
	{
		private TestContext testContextInstance;
		const int Level1 = 1;
		const int Level2 = 2;
		const int Level3 = 3;
		const int Level4 = 4;
		const int Level5 = 5;
		const int Level6 = 6;
		const int Level7 = 7;
		const int Level8 = 8;
		const int Level9 = 9;
		const int Level10 = 10;
		const int Level11 = 11;
		const int Level12 = 12;
		const int Level13 = 13;
		const int Level14 = 14;
		const int Level15 = 15;
		const int Level16 = 16;
		const int Level17 = 17;
		const int Level18 = 18;
		const int Level19 = 19;
		const int Level20 = 20;

		const int SlotLevel1 = 1;
		const int SlotLevel2 = 2;
		const int SlotLevel3 = 3;
		const int SlotLevel4 = 4;
		const int SlotLevel5 = 5;
		const int SlotLevel6 = 6;
		const int SlotLevel7 = 7;
		const int SlotLevel8 = 8;
		const int SlotLevel9 = 9;
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
		private const int Cantrip = 0;
		[TestMethod]
		public void TestRayOfFrost()
		{
			Spell rayOfFrost = AllSpells.Get("Ray of Frost");
			Assert.IsNotNull(rayOfFrost);
			Assert.AreEqual(DndTimeSpan.OneAction, rayOfFrost.CastingTime);
			Assert.AreEqual(DndTimeSpan.Zero, rayOfFrost.Duration);
			Assert.IsTrue(rayOfFrost.HasRange(60, SpellRangeType.DistanceFeet));
			Assert.IsFalse(rayOfFrost.RequiresConcentration);
			Assert.AreEqual(Cantrip, rayOfFrost.Level);
			Assert.AreEqual(SpellComponents.Somatic | SpellComponents.Verbal, rayOfFrost.Components);
			Assert.AreEqual("1d8(cold)", rayOfFrost.DieStr);
		}

		[TestMethod]
		public void TestPlayerLevels()
		{
			Assert.AreEqual("1d8(cold)", AllSpells.Get("Ray of Frost", 1, 1).DieStr);
			Assert.AreEqual("1d8(cold)", AllSpells.Get("Ray of Frost", 1, 4).DieStr);
			Assert.AreEqual("2d8(cold)", AllSpells.Get("Ray of Frost", 1, 5).DieStr);
			Assert.AreEqual("2d8(cold)", AllSpells.Get("Ray of Frost", 1, 10).DieStr);
			Assert.AreEqual("3d8(cold)", AllSpells.Get("Ray of Frost", 1, 11).DieStr);
			Assert.AreEqual("3d8(cold)", AllSpells.Get("Ray of Frost", 1, 16).DieStr);
			Assert.AreEqual("4d8(cold)", AllSpells.Get("Ray of Frost", 1, 17).DieStr);
			Assert.AreEqual("4d8(cold)", AllSpells.Get("Ray of Frost", 1, 22).DieStr);
		}

		[TestMethod]
		public void TestSpellSlotLevels()
		{
			Assert.AreEqual("2d8,1d6", AllSpells.Get("Chaos Bolt", 1).DieStr);
			Assert.AreEqual("2d8,2d6", AllSpells.Get("Chaos Bolt", 2).DieStr);
			Assert.AreEqual("2d8,3d6", AllSpells.Get("Chaos Bolt", 3).DieStr);
			Assert.AreEqual("2d8,4d6", AllSpells.Get("Chaos Bolt", 4).DieStr);
			Assert.AreEqual("2d8,5d6", AllSpells.Get("Chaos Bolt", 5).DieStr);
			Assert.AreEqual("2d8,6d6", AllSpells.Get("Chaos Bolt", 6).DieStr);
			Assert.AreEqual("2d8,7d6", AllSpells.Get("Chaos Bolt", 7).DieStr);
		}

		[TestMethod]
		public void TestSpellcastingAbilityModifier()
		{
			AllSpells.Get("Cure Wounds", 1);
		}


		[TestMethod]
		public void TestChaosBolt()
		{
			Spell chaosBolt = AllSpells.Get("Chaos Bolt");
			Assert.IsTrue(chaosBolt.MorePowerfulWhenCastAtHigherLevels);
			Assert.IsNotNull(chaosBolt);
			Assert.AreEqual(DndTimeSpan.OneAction, chaosBolt.CastingTime);
			Assert.AreEqual(DndTimeSpan.Zero, chaosBolt.Duration);
			Assert.IsTrue(chaosBolt.HasRange(120, SpellRangeType.DistanceFeet));
			Assert.IsFalse(chaosBolt.RequiresConcentration);
			Assert.AreEqual(1, chaosBolt.Level);
			Assert.AreEqual(SpellComponents.Somatic | SpellComponents.Verbal, chaosBolt.Components);
		}

		[TestMethod]
		public void TestSpellSlotLevelPower()
		{
			Spell silentImage = AllSpells.Get("Silent Image");
			Assert.AreEqual(1, silentImage.Level);
			Assert.IsFalse(silentImage.MorePowerfulWhenCastAtHigherLevels);

			Spell sleep = AllSpells.Get("Sleep");
			Assert.AreEqual(1, sleep.Level);
			Assert.IsTrue(sleep.MorePowerfulWhenCastAtHigherLevels);
			sleep.RecalculateDieStr(2, 5, 0);
			Assert.AreEqual("7d8(hpCapacity)", sleep.DieStr);

			sleep.RecalculateDieStr(3, 5, 0);
			Assert.AreEqual("9d8(hpCapacity)", sleep.DieStr);

			sleep.RecalculateDieStr(4, 5, 0);
			Assert.AreEqual("11d8(hpCapacity)", sleep.DieStr);

			sleep.RecalculateDieStr(5, 5, 0);
			Assert.AreEqual("13d8(hpCapacity)", sleep.DieStr);

			Spell astralProjection = AllSpells.Get("Astral Projection");
			Assert.AreEqual(9, astralProjection.Level);
			Assert.IsFalse(astralProjection.MorePowerfulWhenCastAtHigherLevels);
		}

		[TestMethod]
		public void SpellSorcererSlotTests()
		{
			const string Sorcerer = "Sorcerer";
			Assert.AreEqual(2, DndUtils.GetAvailableSpellSlots(Sorcerer, Level1, SlotLevel1));
			Assert.AreEqual(0, DndUtils.GetAvailableSpellSlots(Sorcerer, Level1, SlotLevel2));

			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level2, SlotLevel1));
			Assert.AreEqual(0, DndUtils.GetAvailableSpellSlots(Sorcerer, Level2, SlotLevel2));

			Assert.AreEqual(4, DndUtils.GetAvailableSpellSlots(Sorcerer, Level3, SlotLevel1));
			Assert.AreEqual(2, DndUtils.GetAvailableSpellSlots(Sorcerer, Level3, SlotLevel2));
			Assert.AreEqual(0, DndUtils.GetAvailableSpellSlots(Sorcerer, Level3, SlotLevel3));

			Assert.AreEqual(4, DndUtils.GetAvailableSpellSlots(Sorcerer, Level4, SlotLevel1));
			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level4, SlotLevel2));
			Assert.AreEqual(0, DndUtils.GetAvailableSpellSlots(Sorcerer, Level4, SlotLevel3));

			Assert.AreEqual(4, DndUtils.GetAvailableSpellSlots(Sorcerer, Level5, SlotLevel1));
			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level5, SlotLevel2));
			Assert.AreEqual(2, DndUtils.GetAvailableSpellSlots(Sorcerer, Level5, SlotLevel3));
			Assert.AreEqual(0, DndUtils.GetAvailableSpellSlots(Sorcerer, Level5, SlotLevel4));

			Assert.AreEqual(4, DndUtils.GetAvailableSpellSlots(Sorcerer, Level6, SlotLevel1));
			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level6, SlotLevel2));
			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level6, SlotLevel3));
			Assert.AreEqual(0, DndUtils.GetAvailableSpellSlots(Sorcerer, Level6, SlotLevel4));

			Assert.AreEqual(4, DndUtils.GetAvailableSpellSlots(Sorcerer, Level7, SlotLevel1));
			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level7, SlotLevel2));
			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level7, SlotLevel3));
			Assert.AreEqual(1, DndUtils.GetAvailableSpellSlots(Sorcerer, Level7, SlotLevel4));
			Assert.AreEqual(0, DndUtils.GetAvailableSpellSlots(Sorcerer, Level7, SlotLevel5));

			Assert.AreEqual(4, DndUtils.GetAvailableSpellSlots(Sorcerer, Level8, SlotLevel1));
			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level8, SlotLevel2));
			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level8, SlotLevel3));
			Assert.AreEqual(2, DndUtils.GetAvailableSpellSlots(Sorcerer, Level8, SlotLevel4));
			Assert.AreEqual(0, DndUtils.GetAvailableSpellSlots(Sorcerer, Level8, SlotLevel5));

			Assert.AreEqual(4, DndUtils.GetAvailableSpellSlots(Sorcerer, Level9, SlotLevel1));
			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level9, SlotLevel2));
			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level9, SlotLevel3));
			Assert.AreEqual(3, DndUtils.GetAvailableSpellSlots(Sorcerer, Level9, SlotLevel4));
			Assert.AreEqual(1, DndUtils.GetAvailableSpellSlots(Sorcerer, Level9, SlotLevel5));
			Assert.AreEqual(0, DndUtils.GetAvailableSpellSlots(Sorcerer, Level9, SlotLevel6));
		}

		[TestMethod]
		public void TestSaveSpellEvents()
		{
			AllPlayers.Invalidate();
			History.TimeClock = new DndTimeClock();

			Character merkin = AllPlayers.GetFromId(PlayerID.Merkin);
			Monster joeTheVineBlight = MonsterBuilder.BuildVineBlight("Joe");

			Spell zzzSaveSpell = AllSpells.Get("ZZZ Test Save Spell", merkin, 3);

			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			game.AddPlayer(merkin);
			game.AddMonster(joeTheVineBlight);
			merkin.Cast(zzzSaveSpell, joeTheVineBlight);
			merkin.ReadyRollDice(DiceRollType.DamageOnly, "3d6(fire)", 12);
			List<CastedSpell> activeSpells = merkin.GetActiveSpells();
			Assert.IsNotNull(activeSpells);
			Assert.AreEqual(1, activeSpells.Count);
			Assert.AreEqual("onCast", Expressions.GetStr("Get(_spellState)", merkin));
			game.AdvanceClock(DndTimeSpan.FromMinutes(1));
			Assert.AreEqual("onDispel", Expressions.GetStr("Get(_spellState)", merkin));
			activeSpells = merkin.GetActiveSpells();
			Assert.IsNotNull(activeSpells);
			Assert.AreEqual(0, activeSpells.Count);
		}

		[TestMethod]
		public void TestAttackSpellEventsTimeout()
		{
			AllPlayers.Invalidate();
			Character merkin = AllPlayers.GetFromId(PlayerID.Merkin);
			Monster joeTheVineBlight = MonsterBuilder.BuildVineBlight("Joe");

			Spell zzzRangeSpell = AllSpells.Get("ZZZ Test Range Spell", merkin, 3);

			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			game.AddPlayer(merkin);
			game.AddMonster(joeTheVineBlight);
			game.EnteringCombat();
			merkin.Cast(zzzRangeSpell, joeTheVineBlight);
			List<CastedSpell> activeSpells = merkin.GetActiveSpells();
			Assert.IsNotNull(activeSpells);
			Assert.AreEqual(0, activeSpells.Count);
			Assert.AreEqual("onCasting", Expressions.GetStr("Get(_spellState)", merkin));
			const int hiddenThreshold = 12;

			merkin.ReadyRollDice(DiceRollType.Attack, "1d20(:score),2d8(:damage),1d6(:damage)", hiddenThreshold);
			Assert.AreEqual("onCast", Expressions.GetStr("Get(_spellState)", merkin));
			const int damage = 7;
			merkin.DieRollStopped(hiddenThreshold, damage);

			game.AdvanceClock(DndTimeSpan.FromMinutes(1));
			Assert.AreEqual("onDispel", Expressions.GetStr("Get(_spellState)", merkin));
			activeSpells = merkin.GetActiveSpells();
			Assert.IsNotNull(activeSpells);
			Assert.AreEqual(0, activeSpells.Count);
			game.ExitingCombat();
		}

		[TestMethod]
		public void TestAttackSpellEventsOnPlayerAttacks()
		{
			AllPlayers.Invalidate();
			Character merkin = AllPlayers.GetFromId(PlayerID.Merkin);
			Monster joeVineBlight = MonsterBuilder.BuildVineBlight("Joe");

			Spell zzzSaveSpell = AllSpells.Get("ZZZ Test Save Spell", merkin, 3);

			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			game.AddPlayer(merkin);
			game.AddMonster(joeVineBlight);
			game.EnteringCombat();
			merkin.Cast(zzzSaveSpell, joeVineBlight);

			const int hiddenThreshold = 12;
			const int damage = 7;

			Assert.AreEqual("onCasting", Expressions.GetStr("Get(_spellState)", merkin));
			merkin.ReadyRollDice(DiceRollType.DamageOnly, "1d6(fire)");
			Assert.AreEqual("onCast", Expressions.GetStr("Get(_spellState)", merkin));

			merkin.WillAttack(joeVineBlight, Attack.Melee("Unarmed Strike", 5, 5));
			Assert.AreEqual("onPlayerAttacks", Expressions.GetStr("Get(_spellState)", merkin));

			merkin.ReadyRollDice(DiceRollType.Attack, "1d20(:score),2d8(:damage),1d6(:damage)", hiddenThreshold);
			merkin.DieRollStopped(hiddenThreshold, damage);
			Assert.AreEqual("onPlayerHitsTarget", Expressions.GetStr("Get(_spellState)", merkin));

			List<CastedSpell> activeSpells = merkin.GetActiveSpells();
			Assert.IsNotNull(activeSpells);
			game.ExitingCombat();
		}

		[TestMethod]
		public void TestWrathfulSmiteDuration()
		{
			Spell wrathfulSmite = AllSpells.Get(SpellNames.WrathfulSmite);
			Assert.IsNotNull(wrathfulSmite);
			Assert.AreEqual(TimeSpan.FromMinutes(1), wrathfulSmite.Duration.GetTimeSpan());
		}

		[TestMethod]
		public void TestWrathfulSmite()
		{
			AllPlayers.Invalidate();
			AllSpells.Invalidate();
			AllActionShortcuts.Invalidate();
			Character ava = AllPlayers.GetFromId(PlayerID.Ava);
			PlayerActionShortcut greatsword = ava.GetShortcut("Greatsword");
			Assert.IsNotNull(greatsword);
			Monster joeVineBlight = MonsterBuilder.BuildVineBlight("Joe");

			List<PlayerActionShortcut> wrathfulSmites = AllActionShortcuts.Get(ava.playerID, SpellNames.WrathfulSmite);
			Assert.AreEqual(1, wrathfulSmites.Count);
			PlayerActionShortcut wrathfulSmite = wrathfulSmites[0];
			Assert.IsNotNull(wrathfulSmite);

			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			game.AddPlayer(ava);
			game.AddMonster(joeVineBlight);
			DateTime gameStartTime = game.Time;

			game.EnteringCombat();

			ava.Hits(joeVineBlight, greatsword);  // Action. Ava is first to fight.
			ava.Cast(wrathfulSmite.Spell);  // Bonus Action - Wrathful Smite lasts for one minute.
			Assert.IsTrue(ava.SpellIsActive(SpellNames.WrathfulSmite));

			joeVineBlight.Misses(ava, AttackNames.Constrict);

			AvaMeleeMissesJoe();		// Round 2
			Assert.AreEqual(6, game.SecondsSince(gameStartTime));
			
			joeVineBlight.Misses(ava, AttackNames.Constrict);
			Assert.AreEqual(6, game.SecondsSince(gameStartTime));

			AvaMeleeMissesJoe();    // Round 3
			Assert.AreEqual(12, game.SecondsSince(gameStartTime));

			joeVineBlight.Misses(ava, AttackNames.Constrict);
			Assert.AreEqual(12, game.SecondsSince(gameStartTime));

			AvaMeleeMissesJoe();
			Assert.AreEqual(18, game.SecondsSince(gameStartTime));

			joeVineBlight.Misses(ava, AttackNames.Constrict);
			Assert.AreEqual(18, game.SecondsSince(gameStartTime));

			//`+++NOW THE HIT....
			ava.Hits(joeVineBlight, greatsword);
			Assert.AreEqual(24, game.SecondsSince(gameStartTime));

			Assert.IsTrue(ava.SpellIsActive(SpellNames.WrathfulSmite));  // Wrathful Smite spell is not yet dispelled, however its impact on attack rolls is done.

			Assert.AreEqual($"Target must make a Wisdom saving throw or be frightened of {ava.name} until the spell ends. As an action, the creature can make a Wisdom check against {ava.name}'s spell save DC ({ava.GetSpellSaveDC()}) to steel its resolve and end this {wrathfulSmite.Spell.Name} spell.", game.lastMessageSentToDungeonMaster);

			joeVineBlight.Misses(ava, AttackNames.Constrict);
			Assert.AreEqual(24, game.SecondsSince(gameStartTime));

			ava.Misses(joeVineBlight, greatsword);  // Advancing Round (Ava's turn again).
			Assert.AreEqual("", ava.additionalDiceThisRoll);  // No more die roll effects.
			Assert.AreEqual("", ava.trailingEffectsThisRoll);
			Assert.AreEqual("", ava.dieRollEffectsThisRoll);
			Assert.AreEqual("", ava.dieRollMessageThisRoll);

			Assert.AreEqual(30, game.SecondsSince(gameStartTime));
			game.AdvanceClock(DndTimeSpan.FromSeconds(30));

			Assert.IsFalse(ava.SpellIsActive(SpellNames.WrathfulSmite));  // Wrathful Smite spell should finally be dispelled.

			void AvaMeleeMissesJoe()
			{
				Assert.AreEqual("", ava.additionalDiceThisRoll);
				Assert.AreEqual("", ava.trailingEffectsThisRoll);
				Assert.AreEqual("", ava.dieRollEffectsThisRoll);
				Assert.AreEqual("", ava.dieRollMessageThisRoll);
				ava.Misses(joeVineBlight, greatsword);  // Advancing Round (Ava's turn again).
				Assert.AreEqual("1d6(psychic)", ava.additionalDiceThisRoll);
				Assert.AreEqual("Ravens;Spirals", ava.trailingEffectsThisRoll);
				Assert.AreEqual("PaladinSmite", ava.dieRollEffectsThisRoll);
				Assert.AreEqual("Wrathful Smite", ava.dieRollMessageThisRoll);
				Assert.IsTrue(ava.SpellIsActive(SpellNames.WrathfulSmite));
			}
		}
	}
}
