using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class ActionShortcutsTests
	{
		static ActionShortcutsTests()
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
		public void TestBattleaxeMultiples()
		{
			List<PlayerActionShortcutDto> rawDtos = AllActionShortcuts.LoadData(Folders.InCoreData("DnD - Shortcuts.csv"));
			PlayerActionShortcutDto battleAxe = rawDtos.FirstOrDefault(x => x.name == "Battleaxe" && x.player == "Ava");
			Assert.IsNotNull(battleAxe);
			List<PlayerActionShortcut> actionShortcuts = PlayerActionShortcut.From(battleAxe);
			Assert.IsNotNull(actionShortcuts);
			Assert.AreEqual(2, actionShortcuts.Count);
			Assert.AreEqual("Battleaxe (1H)", actionShortcuts[0].Name);
			Assert.AreEqual("Battleaxe (2H)", actionShortcuts[1].Name);
		}

		[TestMethod]
		public void TestGreatsword()
		{
			List<PlayerActionShortcut> greatSwords = AllActionShortcuts.Get(PlayerID.Ava, "Greatsword");
			Assert.IsNotNull(greatSwords);
			Assert.AreEqual(1, greatSwords.Count);
			Assert.AreEqual(3, greatSwords[0].AttackingAbilityModifier);
			Assert.AreEqual(1, greatSwords[0].PlusModifier);
			Assert.IsTrue(greatSwords[0].UsedWithProficiency);
		}

		[TestMethod]
		public void TestAvaActionShortcuts()
		{
			List<PlayerActionShortcut> avaShortcuts = AllActionShortcuts.Get(PlayerID.Ava);
			PlayerActionShortcut javelin = avaShortcuts.FirstOrDefault(x => x.Name == "Javelin");
			Assert.IsNotNull(javelin);

			PlayerActionShortcut thunderousSmite = avaShortcuts.FirstOrDefault(x => x.Name == "Thunderous Smite");
			Assert.AreEqual(3, thunderousSmite.Windups.Count);
			Assert.IsNotNull(thunderousSmite);
		}

		[TestMethod]
		public void TestSpellCastingTimeTransitionToShortcut()
		{
			List<PlayerActionShortcut> merkinShortcuts = AllActionShortcuts.Get(PlayerID.Merkin);
			PlayerActionShortcut shieldShortcut = merkinShortcuts.FirstOrDefault(x => x.Name == "Shield");
			Assert.AreEqual(TurnPart.Reaction, shieldShortcut.Part);
		}
	}
}
