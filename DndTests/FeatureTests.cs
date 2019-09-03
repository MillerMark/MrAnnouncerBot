using System;
using System.Linq;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class FeatureTests
	{
		static FeatureTests()
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
		public void TestLoad()
		{
			Feature barbarianMelee = AllFeatures.Get("BarbarianMelee");
			Assert.IsNotNull(barbarianMelee);
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			Assert.IsFalse(Expressions.GetBool("BarbarianMelee(strength)", fred));
			Expressions.Do("Set(Rage,true)", fred);
			Assert.IsFalse(Expressions.GetBool("BarbarianMelee(strength)", fred));
			fred.GetAbilityModifier(WeaponProperties.Melee, AttackType.Melee);
			Assert.IsTrue(Expressions.GetBool("BarbarianMelee(strength)", fred));
		}

		[TestMethod]
		public void TestSecondWind()
		{
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			fred.ActivateFeature("SecondWind");
			Assert.AreEqual("1d10+4(healing)", fred.diceJustRolled);
		}
	}
}
