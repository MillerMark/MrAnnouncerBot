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
			Expressions.ExceptionThrown += Expressions_ExceptionThrown;
		}

		private static void Expressions_ExceptionThrown(object sender, DndCoreExceptionEventArgs ea)
		{
			throw ea.Ex; // Simply re-throw the exception
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
			AllPlayers.Invalidate();
			Feature barbarianMelee = AllFeatures.Get("BarbarianMelee");
			Assert.IsNotNull(barbarianMelee);
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			game.AddPlayer(fred);
			game.Start();

			fred.StartTurnResetState();

			Assert.IsFalse(Expressions.GetBool("BarbarianMelee(strength)", fred));
			//Expressions.LogHistory = true;
			Expressions.Do("Set(_rage,true)", fred);
			Assert.IsFalse(Expressions.GetBool("BarbarianMelee(strength)", fred));
			fred.GetAttackingAbilityModifier(WeaponProperties.Melee, AttackType.Melee);
			Assert.IsTrue(Expressions.GetBool("BarbarianMelee(strength)", fred));
		}

		[TestMethod]
		public void TestSecondWind()
		{
			AllPlayers.Invalidate();
			Character fred = AllPlayers.Get("Old Fred");
			fred.ActivateFeature("SecondWind");
			Assert.AreEqual("1d10+4(healing)", fred.diceWeAreRolling);
		}

		[TestMethod]
		public void TestAlwaysOnFeatureActivation()
		{
			AllPlayers.Invalidate();
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			Character lady = game.AddPlayer(AllPlayers.GetFromId(PlayerID.Lady));
			Assert.IsNotNull(lady);
			Assert.IsFalse(lady.IsResistantTo(DamageType.Lightning, AttackKind.Any));
			Assert.IsFalse(lady.IsResistantTo(DamageType.Thunder, AttackKind.Any));
			Assert.IsFalse(lady.IsResistantTo(DamageType.Lightning, AttackKind.Magical));
			Assert.IsFalse(lady.IsResistantTo(DamageType.Thunder, AttackKind.Magical));
			Assert.IsFalse(lady.IsResistantTo(DamageType.Lightning, AttackKind.NonMagical));
			Assert.IsFalse(lady.IsResistantTo(DamageType.Thunder, AttackKind.NonMagical));
			game.Start();
			Assert.IsTrue(lady.IsResistantTo(DamageType.Lightning, AttackKind.Any));
			Assert.IsTrue(lady.IsResistantTo(DamageType.Thunder, AttackKind.Any));
			Assert.IsTrue(lady.IsResistantTo(DamageType.Lightning, AttackKind.Magical));
			Assert.IsTrue(lady.IsResistantTo(DamageType.Thunder, AttackKind.Magical));
			Assert.IsTrue(lady.IsResistantTo(DamageType.Lightning, AttackKind.NonMagical));
			Assert.IsTrue(lady.IsResistantTo(DamageType.Thunder, AttackKind.NonMagical));
		}
	}
}
