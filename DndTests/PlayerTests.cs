using System;
using System.Collections.Generic;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class PlayerTests
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
		public void TestPlayerLoad()
		{
			Folders.UseTestData = true;
			Character willy = AllPlayers.Get("Willy");
			Assert.AreEqual(5, willy.level);
			Assert.AreEqual(35, willy.hitPoints);
			Assert.AreEqual(150, willy.goldPieces);
			Assert.AreEqual(Skills.insight | Skills.perception | Skills.performance | Skills.slightOfHand | Skills.stealth, willy.proficientSkills);
			Assert.AreEqual(Skills.deception | Skills.persuasion, willy.doubleProficiency);
			Assert.AreEqual(Ability.Dexterity | Ability.Intelligence, willy.savingThrowProficiency);
			Assert.AreEqual(Ability.None, willy.spellCastingAbility);
			Assert.AreEqual(3, willy.initiative);
			Assert.AreEqual(VantageKind.Advantage, willy.rollInitiative);
			Assert.AreEqual("#710138", willy.dieBackColor);
			Assert.AreEqual(4, willy.headshotIndex);
			
			
		}

		[TestMethod]
		public void TestLady()
		{
			Folders.UseTestData = true;
			Character lady = AllPlayers.Get("Lady");
			Assert.AreEqual(5, lady.level);
			Assert.AreEqual(39, lady.hitPoints);
			Assert.AreEqual(0, lady.goldPieces);
			Assert.AreEqual(Skills.animalHandling | Skills.arcana| Skills.intimidation| Skills.investigation| Skills.perception | Skills.survival, lady.proficientSkills);
			Assert.AreEqual(Skills.none, lady.doubleProficiency);
			Assert.AreEqual(Ability.Strength| Ability.Constitution, lady.savingThrowProficiency);
			Assert.AreEqual(Ability.None, lady.spellCastingAbility);
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
			Assert.AreEqual(4, activePlayers.Count);
			Assert.IsTrue(activePlayers[0].name.StartsWith("Fred"));
			Assert.IsTrue(activePlayers[1].name.StartsWith("Lady"));
			Assert.IsTrue(activePlayers[2].name.StartsWith("Merkin"));
			Assert.IsTrue(activePlayers[3].name.StartsWith("Ava"));
		}

		[TestMethod]
		public void TestPlayerIds()
		{
			Folders.UseTestData = true;
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
