using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class WildShapeTests
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

		private static Character GetTestDruid()
		{
			Character druid = PlayerHelper.GetPlayerAtLevel("Druid", 2);

			druid.baseCharisma = 9;
			druid.baseStrength = 12;
			druid.baseIntelligence = 15;
			druid.baseWisdom = 10;
			druid.baseDexterity = 11;
			druid.baseConstitution = 9;
			druid.proficiencyBonus = 2;
			return druid;
		}

		[TestMethod]
		public void TestDruidWildShapeAbilityScoresAndBonuses()
		{
			Character druid = GetTestDruid();
			druid.proficiencyBonus = 0;

			Assert.AreEqual(12, druid.Strength);
			Assert.AreEqual(10, druid.PassivePerception);
			Assert.AreEqual(11, druid.Dexterity);
			Assert.AreEqual(9, druid.Constitution);
			Assert.AreEqual(-1, druid.constitutionMod);
			Assert.AreEqual(2, druid.intelligenceMod);
			druid.WildShape = AllMonsters.GetByKind("Giant Badger");
			Assert.AreEqual(13, druid.Strength);
			Assert.AreEqual(10, druid.Dexterity);
			Assert.AreEqual(15, druid.Constitution);
			Assert.AreEqual(11, druid.PassivePerception);
			Assert.AreEqual(2, druid.constitutionMod);
			Assert.AreEqual(2, druid.intelligenceMod);
		}

		[TestMethod]
		public void TestDruidWildShapeSkillChecks()
		{
			Character druid = GetTestDruid();
			druid.proficientSkills = Skills.athletics;

			Assert.AreEqual(1, druid.strengthMod);
			Assert.AreEqual(3, druid.skillModAthletics);
			druid.WildShape = AllMonsters.GetByKind("Giant Ape");
			Assert.AreEqual(6, druid.strengthMod);
			Assert.AreEqual(9, druid.skillModAthletics);
		}

		[TestMethod]
		public void TestDruidWildSenses()
		{
			Character druid = GetTestDruid();
			druid.proficientSkills = Skills.athletics;

			Assert.AreEqual(0, druid.blindsightRadius);
			druid.WildShape = AllMonsters.GetByKind("Giant Bat");
			Assert.AreEqual(60, druid.blindsightRadius);
			Assert.AreEqual(11, druid.PassivePerception);
		}
	}
}
