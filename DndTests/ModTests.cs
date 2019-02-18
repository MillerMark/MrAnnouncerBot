using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DndTests
{
	[TestClass]
	public class ModTests
	{
		[TestMethod]
		public void TestHighMods()
		{
			Character player = CharacterBuilder.BuildTestWizard();
			player.intelligence = 18;
			player.strength = 17;
			player.charisma = 16;
			player.dexterity = 15;
			player.constitution = 14;
			player.wisdom = 13;
			Assert.AreEqual(4, player.intelligenceMod);
			Assert.AreEqual(3, player.strengthMod);
			Assert.AreEqual(3, player.charismaMod);
			Assert.AreEqual(2, player.dexterityMod);
			Assert.AreEqual(2, player.constitutionMod);
			Assert.AreEqual(1, player.wisdomMod);
		}

		[TestMethod]
		public void TestMiddleMods()
		{
			Character player = CharacterBuilder.BuildTestWizard();
			player.intelligence = 12;
			player.strength = 11;
			player.charisma = 10;
			player.dexterity = 9;
			player.constitution = 8;
			player.wisdom = 7;
			Assert.AreEqual(1, player.intelligenceMod);
			Assert.AreEqual(0, player.strengthMod);
			Assert.AreEqual(0, player.charismaMod);
			Assert.AreEqual(-1, player.dexterityMod);
			Assert.AreEqual(-1, player.constitutionMod);
			Assert.AreEqual(-2, player.wisdomMod);
		}

		[TestMethod]
		public void TestLowMods()
		{
			Character player = CharacterBuilder.BuildTestWizard();
			player.intelligence = 6;
			player.strength = 5;
			player.charisma = 4;
			player.dexterity = 3;
			player.constitution = 2;
			player.wisdom = 1;
			Assert.AreEqual(-2, player.intelligenceMod);
			Assert.AreEqual(-3, player.strengthMod);
			Assert.AreEqual(-3, player.charismaMod);
			Assert.AreEqual(-4, player.dexterityMod);
			Assert.AreEqual(-4, player.constitutionMod);
			Assert.AreEqual(-5, player.wisdomMod);
		}
	}
}
