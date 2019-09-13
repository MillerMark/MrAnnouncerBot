using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DndTests
{
	[TestClass]
	public class ModTests
	{
		[TestMethod]
		public void TestHighMods()
		{
			Character player = CharacterBuilder.BuildTestWizard();
			player.baseIntelligence = 18;
			player.baseStrength = 17;
			player.baseCharisma = 16;
			player.baseDexterity = 15;
			player.baseConstitution = 14;
			player.baseWisdom = 13;
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
			player.baseIntelligence = 12;
			player.baseStrength = 11;
			player.baseCharisma = 10;
			player.baseDexterity = 9;
			player.baseConstitution = 8;
			player.baseWisdom = 7;
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
			player.baseIntelligence = 6;
			player.baseStrength = 5;
			player.baseCharisma = 4;
			player.baseDexterity = 3;
			player.baseConstitution = 2;
			player.baseWisdom = 1;
			Assert.AreEqual(-2, player.intelligenceMod);
			Assert.AreEqual(-3, player.strengthMod);
			Assert.AreEqual(-3, player.charismaMod);
			Assert.AreEqual(-4, player.dexterityMod);
			Assert.AreEqual(-4, player.constitutionMod);
			Assert.AreEqual(-5, player.wisdomMod);
		}

		[TestMethod]
		public void TestItemEquipMod()
		{
			ItemViewModel ringOfLeaping = TestStorageHelper.GetExistingItem("Ring of the Faithful Leap");
			Character testWizard = CharacterBuilder.BuildTestWizard();
			const int initialDexterity = 12;
			const int initialStrength = 11;
			const int initialSpeed = 30;

			testWizard.baseDexterity = initialDexterity;
			testWizard.baseStrength = initialStrength;
			testWizard.baseWalkingSpeed = initialSpeed;
			Assert.AreEqual(VantageKind.Normal, testWizard.GetSkillCheckDice(Skills.athletics));
			Assert.AreEqual(VantageKind.Normal, testWizard.GetSkillCheckDice(Skills.acrobatics));
			Assert.AreEqual(initialDexterity, testWizard.Dexterity);
			Assert.AreEqual(initialStrength, testWizard.Strength);
			Assert.AreEqual(initialSpeed, testWizard.WalkingSpeed);
			testWizard.Equip(ringOfLeaping);
			Assert.AreEqual(initialDexterity + 1, testWizard.Dexterity);
			Assert.AreEqual(initialStrength + 1, testWizard.Strength);
			Assert.AreEqual(initialSpeed + 5, testWizard.WalkingSpeed);
			Assert.AreEqual(VantageKind.Advantage, testWizard.GetSkillCheckDice(Skills.athletics));
			Assert.AreEqual(VantageKind.Advantage, testWizard.GetSkillCheckDice(Skills.acrobatics));

			testWizard.Unequip(ringOfLeaping);

			Assert.AreEqual(VantageKind.Normal, testWizard.GetSkillCheckDice(Skills.athletics));
			Assert.AreEqual(VantageKind.Normal, testWizard.GetSkillCheckDice(Skills.acrobatics));
			Assert.AreEqual(initialDexterity, testWizard.Dexterity);
			Assert.AreEqual(initialStrength, testWizard.Strength);
			Assert.AreEqual(initialSpeed, testWizard.WalkingSpeed);
		}

		[TestMethod]
		public void TestBreastplate()
		{
			ItemViewModel breastplate = TestStorageHelper.GetExistingItem("Breastplate");
			Character testBarbarian = CharacterBuilder.BuildTestBarbarian();
			const double initialArmorClass = 12;

			testBarbarian.baseDexterity = 12;
			double dexterityMod = (testBarbarian.Dexterity - 10) / 2;
			testBarbarian.baseArmorClass = initialArmorClass;
			Assert.AreEqual(initialArmorClass, testBarbarian.ArmorClass);

			testBarbarian.Equip(breastplate);

			const double breastplateAbsoluteAC = 15;
			Assert.AreEqual(breastplateAbsoluteAC + Math.Min(dexterityMod, 2), testBarbarian.ArmorClass);

			testBarbarian.Unequip(breastplate);

			Assert.AreEqual(initialArmorClass, testBarbarian.ArmorClass);

			testBarbarian.baseDexterity = 14;
			dexterityMod = (testBarbarian.Dexterity - 10) / 2;
			testBarbarian.Equip(breastplate);

			Assert.AreEqual(breastplateAbsoluteAC + Math.Min(dexterityMod, 2), testBarbarian.ArmorClass);


			testBarbarian.Unequip(breastplate);

			Assert.AreEqual(initialArmorClass, testBarbarian.ArmorClass);

			testBarbarian.baseDexterity = 16;
			dexterityMod = (testBarbarian.Dexterity - 10) / 2;
			Assert.IsTrue(dexterityMod >= 3);
			testBarbarian.Equip(breastplate);

			Assert.AreEqual(breastplateAbsoluteAC + Math.Min(dexterityMod, 2), testBarbarian.ArmorClass);
		}
	}
	[TestClass]
	public class SecondaryTests
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
		public void Test()
		{
			
		}
	}
}
