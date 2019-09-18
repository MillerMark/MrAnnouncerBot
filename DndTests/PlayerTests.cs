using System;
using System.Collections.Generic;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class PlayerTests
	{
		static PlayerTests()
		{
			Folders.UseTestData = true;
		}
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
		public void TestPlayerLoad()
		{
			Character willy = AllPlayers.Get("Willy");
			Assert.AreEqual(5, willy.level);
			Assert.AreEqual(43, willy.hitPoints);
			Assert.AreEqual(200, willy.goldPieces);
			Assert.AreEqual(Skills.insight | Skills.perception | Skills.performance | Skills.slightOfHand | Skills.stealth, willy.proficientSkills);
			Assert.AreEqual(Skills.deception | Skills.persuasion, willy.doubleProficiency);
			Assert.AreEqual(Ability.dexterity | Ability.intelligence, willy.savingThrowProficiency);
			Assert.AreEqual(Ability.none, willy.spellCastingAbility);
			Assert.AreEqual(3, willy.initiative);
			Assert.AreEqual(VantageKind.Advantage, willy.rollInitiative);
			Assert.AreEqual("#710138", willy.dieBackColor);
			Assert.AreEqual(4, willy.headshotIndex);
		}

		[TestMethod]
		public void TestWeaponProficiencies()
		{
			Character willy = AllPlayers.Get("Willy");
			Assert.IsTrue(willy.IsProficientWith(Weapons.HandCrossbow));
			Assert.IsTrue(willy.IsProficientWith(Weapons.Longbow));
			Assert.IsTrue(willy.IsProficientWith(Weapons.Longsword));
			Assert.IsTrue(willy.IsProficientWith(Weapons.Rapier));
			Assert.IsTrue(willy.IsProficientWith(Weapons.Shortsword));
			Assert.IsTrue(willy.IsProficientWith(Weapons.Simple));
			Assert.IsFalse(willy.IsProficientWith(Weapons.Martial));

			Character shemo = AllPlayers.Get("Shemo");
			Assert.IsTrue(shemo.IsProficientWith(Weapons.Club));
			Assert.IsTrue(shemo.IsProficientWith(Weapons.Dagger));
			Assert.IsTrue(shemo.IsProficientWith(Weapons.Dart));
			Assert.IsTrue(shemo.IsProficientWith(Weapons.Javelin));
			Assert.IsTrue(shemo.IsProficientWith(Weapons.Mace));
			Assert.IsTrue(shemo.IsProficientWith(Weapons.Quarterstaff));
			Assert.IsTrue(shemo.IsProficientWith(Weapons.Sickle));
			Assert.IsTrue(shemo.IsProficientWith(Weapons.Scimitar));
			Assert.IsTrue(shemo.IsProficientWith(Weapons.Sling));
			Assert.IsTrue(shemo.IsProficientWith(Weapons.Spear));
			Assert.IsFalse(shemo.IsProficientWith(Weapons.Simple));
			Assert.IsFalse(shemo.IsProficientWith(Weapons.Martial));

			Character lady = AllPlayers.Get("Lady");
			Assert.IsTrue(lady.IsProficientWith(Weapons.LightCrossbow));
			Assert.IsTrue(lady.IsProficientWith(Weapons.Simple));
			Assert.IsTrue(lady.IsProficientWith(Weapons.Martial));

			Character merkin = AllPlayers.Get("Merkin");
			Assert.IsTrue(merkin.IsProficientWith(Weapons.Wizard));
			Assert.IsFalse(merkin.IsProficientWith(Weapons.Simple));
			Assert.IsFalse(merkin.IsProficientWith(Weapons.Martial));
			Assert.IsFalse(merkin.IsProficientWith(Weapons.UnarmedStrike));

			Character fred = AllPlayers.Get("Fred");
			Assert.IsTrue(fred.IsProficientWith(Weapons.Simple));
			Assert.IsTrue(fred.IsProficientWith(Weapons.Martial));
		}

		[TestMethod]
		public void TestLady()
		{
			Character lady = AllPlayers.Get("Lady");
			Assert.AreEqual(5, lady.level);
			Assert.AreEqual(39, lady.hitPoints);
			Assert.AreEqual(0, lady.goldPieces);
			Assert.AreEqual(Skills.animalHandling | Skills.arcana| Skills.intimidation| Skills.investigation| Skills.perception | Skills.survival, lady.proficientSkills);
			Assert.AreEqual(Skills.none, lady.doubleProficiency);
			Assert.AreEqual(Ability.strength| Ability.constitution, lady.savingThrowProficiency);
			Assert.AreEqual(Ability.none, lady.spellCastingAbility);
			Assert.AreEqual(2, lady.initiative);
			Assert.AreEqual(VantageKind.Normal, lady.rollInitiative);
			Assert.AreEqual(37, lady.hueShift);
			Assert.AreEqual("#fea424", lady.dieBackColor);
			Assert.AreEqual(0, lady.headshotIndex);
		}

		[TestMethod]
		public void TestLeftmostPriority()
		{
			List<Character> activePlayers = AllPlayers.GetActive();
			Assert.AreEqual(5, activePlayers.Count);
			Assert.IsTrue(activePlayers[0].name.StartsWith("Fred"));
			Assert.IsTrue(activePlayers[1].name.StartsWith("Willy"));
			Assert.IsTrue(activePlayers[2].name.StartsWith("Lady"));
			Assert.IsTrue(activePlayers[3].name.StartsWith("Merkin"));
			Assert.IsTrue(activePlayers[4].name.StartsWith("Ava"));
		}

		[TestMethod]
		public void TestPlayerIds()
		{
			Character lady = AllPlayers.Get("Lady");
			Assert.AreEqual(PlayerID.Lady, lady.playerID);
			Character willy = AllPlayers.Get("Willy");
			Assert.AreEqual(PlayerID.Willy, willy.playerID);
			Character merkin = AllPlayers.Get("Merkin");
			Assert.AreEqual(PlayerID.Merkin, merkin.playerID);
			Character fred = AllPlayers.Get("Fred");
			Assert.AreEqual(PlayerID.Fred, fred.playerID);
			Character ava = AllPlayers.Get("Ava");
			Assert.AreEqual(PlayerID.Ava, ava.playerID);
			Character shemo = AllPlayers.Get("Shemo");
			Assert.AreEqual(PlayerID.Shemo, shemo.playerID);
		}
	}
}
