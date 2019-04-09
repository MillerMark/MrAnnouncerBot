using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DndTests
{

	[TestClass]
	public class AbilityTests
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
		public void TestAbilityStrRead()
		{
			Monster testMonster = new Monster();
			testMonster.SetAbilitiesFromStr(@"STR
																				15 (+2)
																				DEX
																				8 (-1)
																				CON
																				14 (+2)
																				INT
																				5 (-3)
																				WIS
																				10 (+0)
																				CHA
																				3 (-4)
																				");
			Assert.AreEqual(15, testMonster.Strength);
			Assert.AreEqual(+2, testMonster.strengthMod);
			Assert.AreEqual(8, testMonster.Dexterity);
			Assert.AreEqual(-1, testMonster.dexterityMod);
			Assert.AreEqual(14, testMonster.Constitution);
			Assert.AreEqual(+2, testMonster.constitutionMod);
			Assert.AreEqual(5, testMonster.Intelligence);
			Assert.AreEqual(-3, testMonster.intelligenceMod);
			Assert.AreEqual(10, testMonster.Wisdom);
			Assert.AreEqual(+0, testMonster.wisdomMod);
			Assert.AreEqual(3, testMonster.Charisma);
			Assert.AreEqual(-4, testMonster.charismaMod);
		}
	}
}
