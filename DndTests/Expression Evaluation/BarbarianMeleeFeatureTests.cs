using System;
using System.Linq;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class BarbarianMeleeFeatureTests
	{
		static BarbarianMeleeFeatureTests()
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
		public void TestBarbarianMeleeDamageOffset()
		{
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			fred.StartTurnResetState();
			Assert.AreEqual(0, fred.damageOffsetThisRoll);
			fred.ActivateFeature("BarbarianMelee");
			Assert.AreEqual(2, fred.damageOffsetThisRoll);

			fred.DeactivateFeature("BarbarianMelee");

			CharacterClass barbarianClass = fred.Classes.FirstOrDefault(x => x.Name == "Barbarian");
			Assert.IsNotNull(barbarianClass);
			fred.ResetPlayerActionBasedState();
			barbarianClass.Level = 9;
			fred.ActivateFeature("BarbarianMelee");
			Assert.AreEqual(3, fred.damageOffsetThisRoll);

			fred.DeactivateFeature("BarbarianMelee");
			fred.ResetPlayerActionBasedState();
			barbarianClass.Level = 16;
			fred.ActivateFeature("BarbarianMelee");
			Assert.AreEqual(4, fred.damageOffsetThisRoll);
		}

		[TestMethod]
		public void TestBarbarianMeleeDeactivate()
		{
			AllPlayers.Invalidate();
			AllFeatures.Invalidate();
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			game.AddPlayer(fred);
			game.Start();

			PlayerActionShortcut greataxe = fred.GetShortcut("Greataxe");
			fred.Use(greataxe);

			CharacterClass barbarianClass = fred.Classes.FirstOrDefault(x => x.Name == "Barbarian");
			Assert.IsNotNull(barbarianClass);
			barbarianClass.Level = 16;
			fred.ResetPlayerActionBasedState();

			Assert.AreEqual(0, fred.damageOffsetThisRoll);

			fred.ActivateFeature("WildSurgeRage");
			Assert.AreEqual(0, fred.damageOffsetThisRoll);
			fred.Use(greataxe);
			Assert.AreEqual(4, fred.damageOffsetThisRoll);

			AssignedFeature rageFeature = fred.GetFeature("WildSurgeRage");
			Assert.IsTrue(rageFeature.Feature.IsActive);

			Expressions.Do("Set(_rage,false)", fred);

			Assert.IsFalse(rageFeature.Feature.IsActive);
			Assert.IsFalse(fred.GetFeature("BarbarianMelee").Feature.IsActive);
			fred.Use(greataxe);
			Assert.IsFalse(fred.GetFeature("BarbarianMelee").Feature.IsActive);

			Assert.AreEqual(0, fred.damageOffsetThisRoll);
			

			fred.ActivateFeature("WildSurgeRage");
			Assert.IsTrue(fred.GetFeature("BarbarianMelee").Feature.IsActive);
			Assert.AreEqual(4, fred.damageOffsetThisRoll);

			fred.DeactivateFeature("WildSurgeRage");
			Assert.IsFalse(fred.GetFeature("BarbarianMelee").Feature.IsActive);
			Assert.AreEqual(0, fred.damageOffsetThisRoll);
		}


		[TestMethod]
		public void TestBarbarianMeleeConditions()
		{
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			game.AddPlayer(fred);
			game.Start();
			fred.StartTurnResetState();
			AssignedFeature barbarianMelee = fred.GetFeature("BarbarianMelee");
			Assert.IsFalse(barbarianMelee.ShouldActivateNow());
			Expressions.Do("Set(_rage,true)", fred);
			fred.GetAttackingAbilityModifier(WeaponProperties.Melee, AttackType.Melee);
			Assert.IsTrue(barbarianMelee.ShouldActivateNow());
		}

	 	[TestMethod]
		public void TestBarbarianMeleeRageExpiration()
		{
			AllPlayers.Invalidate();
			AllFeatures.Invalidate();
			History.TimeClock = new DndTimeClock();
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			game.AddPlayer(fred);
			game.Start();

			fred.StartTurnResetState();
			AssignedFeature barbarianMelee = fred.GetFeature("BarbarianMelee");

			Assert.IsFalse(barbarianMelee.ShouldActivateNow());

			fred.ActivateFeature("WildSurgeRage");
			PlayerActionShortcut greataxe = fred.GetShortcut("Greataxe");
			fred.Use(greataxe);

			Assert.IsTrue(barbarianMelee.ShouldActivateNow());

			// Now test alarm system to turn off rage after one minute....
			History.TimeClock.Advance(DndTimeSpan.FromSeconds(59));
			Assert.IsTrue(barbarianMelee.ShouldActivateNow());
			History.TimeClock.Advance(DndTimeSpan.FromSeconds(1));
			Assert.IsFalse(barbarianMelee.ShouldActivateNow());
		}

		[TestMethod]
		public void TestBarbarianMeleeConditionsWithVariousWeapons()
		{
			AllPlayers.Invalidate();
			History.TimeClock = new DndTimeClock();
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			game.AddPlayer(fred);
			game.Start();
			fred.StartTurnResetState();
			AssignedFeature barbarianMelee = fred.GetFeature("BarbarianMelee");

			Assert.IsFalse(barbarianMelee.ShouldActivateNow());

			fred.ActivateFeature("WildSurgeRage");
			Assert.IsFalse(barbarianMelee.ShouldActivateNow());  // Not yet. We need to be using the right weapon.

			PlayerActionShortcut greataxe = fred.GetShortcut("Greataxe");
			Assert.IsNotNull(greataxe);
			fred.Use(greataxe);

			Assert.IsTrue(Expressions.GetBool("InRage", fred));
			Assert.IsTrue(Expressions.GetBool("AttackIsMelee", fred));
			Assert.IsTrue(Expressions.GetBool("AttackingWith(strength)", fred));
			Assert.IsTrue(barbarianMelee.ShouldActivateNow());

			PlayerActionShortcut longbow = fred.GetShortcut("Longbow");
			fred.Use(longbow);
			Assert.IsFalse(barbarianMelee.ShouldActivateNow());
		}

		[TestMethod]
		public void TestBarbarianMeleeConditionsWithFinesseWeapons()
		{
			History.TimeClock = new DndTimeClock();
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			game.AddPlayer(fred);
			game.Start();
			fred.StartTurnResetState();
			AssignedFeature barbarianMelee = fred.GetFeature("BarbarianMelee");

			Assert.IsFalse(barbarianMelee.ShouldActivateNow());

			fred.ActivateFeature("WildSurgeRage");
			Assert.IsFalse(barbarianMelee.ShouldActivateNow());  // Not yet. We need to be using the right weapon.

			PlayerActionShortcut dagger = fred.GetShortcut("Dagger");
			Assert.IsNotNull(dagger);
			fred.Use(dagger);
			Assert.IsTrue(barbarianMelee.ShouldActivateNow());

			fred.baseDexterity = 18;
			fred.baseStrength = 10;
			dagger.UpdatePlayerAttackingAbility(fred, false);
			fred.Use(dagger);
			Assert.IsFalse(barbarianMelee.ShouldActivateNow());  // Should not be satisfied because dexterity is now the ability of choice to use with this finesse weapon.
		}
	}
}
