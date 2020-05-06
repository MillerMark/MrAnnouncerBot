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
			AllKnownItems.Invalidate();
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
			List<PlayerActionShortcut> actionShortcuts = AllActionShortcuts.Get(PlayerID.LilCutie, "Battleaxe");
			Assert.IsNotNull(actionShortcuts);
			Assert.AreEqual(2, actionShortcuts.Count);
			Assert.AreEqual("Battleaxe (1H)", actionShortcuts[0].DisplayText);
			Assert.AreEqual("Battleaxe (2H)", actionShortcuts[1].DisplayText);
		}

		[TestMethod]
		public void TestGreatsword()
		{
			List<PlayerActionShortcut> greatSwords = AllActionShortcuts.Get(PlayerID.LilCutie, "Greatsword");
			Assert.IsNotNull(greatSwords);
			Assert.AreEqual(1, greatSwords.Count);
			Assert.AreEqual(3, greatSwords[0].AttackingAbilityModifier);
			Assert.AreEqual(1, greatSwords[0].PlusModifier);
			Assert.IsTrue(greatSwords[0].UsedWithProficiency);
		}

		[TestMethod]
		public void TestAvaActionShortcuts()
		{
			List<PlayerActionShortcut> avaShortcuts = AllActionShortcuts.Get(PlayerID.LilCutie);
			PlayerActionShortcut javelin = avaShortcuts.FirstOrDefault(x => x.DisplayText == "Javelin");
			Assert.IsNotNull(javelin);

			PlayerActionShortcut thunderousSmite = avaShortcuts.FirstOrDefault(x => x.DisplayText == "Thunderous Smite");
			Assert.AreEqual(3, thunderousSmite.Windups.Count);
			Assert.IsNotNull(thunderousSmite);
		}

		[TestMethod]
		public void TestSpellCastingTimeTransitionToShortcut()
		{
			List<PlayerActionShortcut> merkinShortcuts = AllActionShortcuts.Get(PlayerID.Merkin);
			PlayerActionShortcut shieldShortcut = merkinShortcuts.FirstOrDefault(x => x.DisplayText == "Shield");
			Assert.AreEqual(TurnPart.Reaction, shieldShortcut.Part);
		}
	}
}
