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
			fred.ResetPlayerStartTurnBasedState();
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
			AllPlayers.LoadData();

			DndGame dndGame = DndGame.Instance;
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			dndGame.AddPlayer(fred);

			PlayerActionShortcut battleaxe = fred.GetShortcut("Battleaxe (1H)");
			fred.Use(battleaxe);

			CharacterClass barbarianClass = fred.Classes.FirstOrDefault(x => x.Name == "Barbarian");
			Assert.IsNotNull(barbarianClass);
			barbarianClass.Level = 16;
			fred.ResetPlayerActionBasedState();

			Assert.AreEqual(0, fred.damageOffsetThisRoll);

			fred.ActivateFeature("Rage");
			Assert.AreEqual(0, fred.damageOffsetThisRoll);
			fred.Use(battleaxe);
			Assert.AreEqual(4, fred.damageOffsetThisRoll);

			AssignedFeature rageFeature = fred.GetFeature("Rage");
			Assert.IsTrue(rageFeature.Feature.IsActive);

			Expressions.Do("Set(_rage,false)", fred);

			Assert.IsFalse(rageFeature.Feature.IsActive);
			Assert.IsFalse(fred.GetFeature("BarbarianMelee").Feature.IsActive);
			fred.Use(battleaxe);
			Assert.IsFalse(fred.GetFeature("BarbarianMelee").Feature.IsActive);

			Assert.AreEqual(0, fred.damageOffsetThisRoll);
			

			fred.ActivateFeature("Rage");
			Assert.IsTrue(fred.GetFeature("BarbarianMelee").Feature.IsActive);
			Assert.AreEqual(4, fred.damageOffsetThisRoll);

			fred.DeactivateFeature("Rage");
			Assert.IsFalse(fred.GetFeature("BarbarianMelee").Feature.IsActive);
			Assert.AreEqual(0, fred.damageOffsetThisRoll);
		}


		[TestMethod]
		public void TestBarbarianMeleeConditions()
		{
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			fred.ResetPlayerStartTurnBasedState();
			AssignedFeature barbarianMelee = fred.GetFeature("BarbarianMelee");
			Assert.IsFalse(barbarianMelee.ConditionsSatisfied());
			Expressions.Do("Set(_rage,true)", fred);
			fred.GetAttackingAbilityModifier(WeaponProperties.Melee, AttackType.Melee);
			Assert.IsTrue(barbarianMelee.ConditionsSatisfied());
		}

	 	[TestMethod]
		public void TestBarbarianMeleeRageExpiration()
		{
			AllPlayers.LoadData();
			History.TimeClock = new DndTimeClock();
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			fred.ResetPlayerStartTurnBasedState();
			AssignedFeature barbarianMelee = fred.GetFeature("BarbarianMelee");

			Assert.IsFalse(barbarianMelee.ConditionsSatisfied());

			fred.ActivateFeature("Rage");
			PlayerActionShortcut battleaxe = fred.GetShortcut("Battleaxe (1H)");
			fred.Use(battleaxe);

			Assert.IsTrue(barbarianMelee.ConditionsSatisfied());

			// Now test alarm system to turn off rage after one minute....
			History.TimeClock.Advance(DndTimeSpan.FromSeconds(59));
			Assert.IsTrue(barbarianMelee.ConditionsSatisfied());
			History.TimeClock.Advance(DndTimeSpan.FromSeconds(1));
			Assert.IsFalse(barbarianMelee.ConditionsSatisfied());
		}

		[TestMethod]
		public void TestBarbarianMeleeConditionsWithVariousWeapons()
		{
			AllPlayers.LoadData();
			History.TimeClock = new DndTimeClock();
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			fred.ResetPlayerStartTurnBasedState();
			AssignedFeature barbarianMelee = fred.GetFeature("BarbarianMelee");

			Assert.IsFalse(barbarianMelee.ConditionsSatisfied());

			fred.ActivateFeature("Rage");
			Assert.IsFalse(barbarianMelee.ConditionsSatisfied());  // Not yet. We need to be using the right weapon.

			PlayerActionShortcut battleaxe = fred.GetShortcut("Battleaxe (1H)");
			fred.Use(battleaxe);

			Assert.IsTrue(Expressions.GetBool("InRage", fred));
			Assert.IsTrue(Expressions.GetBool("AttackIsMelee", fred));
			Assert.IsTrue(Expressions.GetBool("AttackingWith(strength)", fred));
			Assert.IsTrue(barbarianMelee.ConditionsSatisfied());

			PlayerActionShortcut longbow = fred.GetShortcut("Longbow");
			fred.Use(longbow);
			Assert.IsFalse(barbarianMelee.ConditionsSatisfied());
		}

		[TestMethod]
		public void TestBarbarianMeleeConditionsWithFinesseWeapons()
		{
			History.TimeClock = new DndTimeClock();
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			fred.ResetPlayerStartTurnBasedState();
			AssignedFeature barbarianMelee = fred.GetFeature("BarbarianMelee");

			Assert.IsFalse(barbarianMelee.ConditionsSatisfied());

			fred.ActivateFeature("Rage");
			Assert.IsFalse(barbarianMelee.ConditionsSatisfied());  // Not yet. We need to be using the right weapon.

			PlayerActionShortcut shortsword = fred.GetShortcut("Shortsword");
			fred.Use(shortsword);
			Assert.IsTrue(barbarianMelee.ConditionsSatisfied());

			fred.baseDexterity = 18;
			fred.baseStrength = 10;
			shortsword.UpdatePlayerAttackingAbility(fred);
			fred.Use(shortsword);
			Assert.IsFalse(barbarianMelee.ConditionsSatisfied());  // Should not be satisfied because dexterity is now the ability of choice to use with this finesse weapon.
		}
	}
}
