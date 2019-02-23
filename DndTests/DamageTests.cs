using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class AttackTests
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
		public void TestConditionalGrapple()
		{
			Monster vineBlight = MonsterBuilder.BuildVineBlight();
			Character barbarian = CharacterBuilder.BuildTestBarbarian();
			Character elf = CharacterBuilder.BuildTestElf();
			Character druid = CharacterBuilder.BuildTestDruid();
			elf.creatureSize = CreatureSize.Small;
			barbarian.creatureSize = CreatureSize.Huge;
			Assert.IsFalse(barbarian.HasCondition(Conditions.Grappled));
			Assert.IsFalse(elf.HasCondition(Conditions.Grappled));
			Assert.IsFalse(druid.HasCondition(Conditions.Grappled));
			DamageResult elfResults = vineBlight.Attack(elf, "Constrict", 10);
			DamageResult barbarianResults = vineBlight.Attack(barbarian, "Constrict", 10);
			DamageResult druidResults = vineBlight.Attack(druid, "Constrict", 12);
			Assert.IsFalse(barbarianResults.HasCondition(Conditions.Grappled));
			Assert.IsTrue(elfResults.HasCondition(Conditions.Grappled));
			Assert.IsFalse(druidResults.HasCondition(Conditions.Grappled));  // druid saving throw avoids the grapple.
			Assert.AreEqual(-9, elfResults.hitPointChange);
			Assert.AreEqual(-9, barbarianResults.hitPointChange);
			Assert.AreEqual(-9, druidResults.hitPointChange);
			Assert.AreEqual(DamageType.Bludgeoning, elfResults.damageTypes);
			Assert.AreEqual(DamageType.Bludgeoning, barbarianResults.damageTypes);
			Assert.AreEqual(DamageType.Bludgeoning, druidResults.damageTypes);
		}
	}

	[TestClass]
	public class DamageTests
	{
		[TestMethod]
		public void TestBasicDamage()
		{
			Character player = CharacterBuilder.BuildTestDruid();
			Weapon shortSword = Weapon.buildShortSword();
			player.hitPoints = 100;
			shortSword.Attack("Stab", player);
			Assert.AreEqual(96.5, player.hitPoints);
			Assert.AreEqual(3.5, player.LastDamagePointsTaken);
			Assert.AreEqual(DamageType.Piercing, player.LastDamageTaken);
		}

		[TestMethod]
		public void TestResistance()
		{
			Character player = CharacterBuilder.BuildTestDruid();
			Weapon shortSword = Weapon.buildShortSword();
			const double startingHitPoints = 100;
			player.hitPoints = startingHitPoints;
			player.AddDamageResistance(DamageType.Piercing, AttackKind.Magical | AttackKind.NonMagical);
			shortSword.Attack("Stab", player);
			const double expectedDamage = 1.75;
			Assert.AreEqual(startingHitPoints - expectedDamage, player.hitPoints);
			Assert.AreEqual(expectedDamage, player.LastDamagePointsTaken);
		}

		[TestMethod]
		public void TestImmunities()
		{
			Character player = CharacterBuilder.BuildTestDruid();
			Weapon shortSword = Weapon.buildShortSword();
			const double startingHitPoints = 100;
			player.hitPoints = startingHitPoints;
			player.AddDamageImmunity(DamageType.Piercing, AttackKind.Any);
			shortSword.Attack("Stab", player);
			shortSword.Attack("Stab", player);
			shortSword.Attack("Stab", player);
			shortSword.Attack("Stab", player);
			Assert.AreEqual(startingHitPoints, player.hitPoints);
			Assert.AreEqual(0, player.LastDamagePointsTaken);
			Assert.AreEqual(DamageType.None, player.LastDamageTaken);
		}

		[TestMethod]
		public void TestNoResistance()
		{
			Character player = CharacterBuilder.BuildTestDruid();
			Weapon shortSword = Weapon.buildShortSword();
			const double startingHitPoints = 100;
			player.hitPoints = startingHitPoints;
			player.AddDamageResistance(DamageType.Lightning, AttackKind.Magical | AttackKind.NonMagical);
			shortSword.Attack("Stab", player);
			const double expectedDamage = 3.5;
			Assert.AreEqual(startingHitPoints - expectedDamage, player.hitPoints);
			Assert.AreEqual(expectedDamage, player.LastDamagePointsTaken);
		}

		[TestMethod]
		public void TestMagicShortswordImmunity()
		{
			Weapon magicalShortsword = Weapon.buildMagicalShortSword();
			// vs.
			Character player = CharacterBuilder.BuildTestDruid();
			const double startingHitPoints = 100;
			player.hitPoints = startingHitPoints;
			player.AddDamageImmunity(DamageType.Piercing, AttackKind.Magical);
			magicalShortsword.Attack("Stab", player);
			const double expectedDamage = 0;
			Assert.AreEqual(startingHitPoints - expectedDamage, player.hitPoints);
			Assert.AreEqual(expectedDamage, player.LastDamagePointsTaken);
			Assert.AreEqual(DamageType.None, player.LastDamageTaken);
		}

		[TestMethod]
		public void TestNonMagicalShortswordFailedImmunity()
		{
			Weapon shortsword = Weapon.buildShortSword();
			// vs.
			Character player = CharacterBuilder.BuildTestDruid();
			const double startingHitPoints = 100;
			player.hitPoints = startingHitPoints;
			player.AddDamageImmunity(DamageType.Piercing, AttackKind.Magical);
			shortsword.Attack("Stab", player);
			const double expectedDamage = 3.5;
			Assert.AreEqual(startingHitPoints - expectedDamage, player.hitPoints);
			Assert.AreEqual(expectedDamage, player.LastDamagePointsTaken);
			Assert.AreEqual(DamageType.Piercing, player.LastDamageTaken);
		}

		[TestMethod]
		public void TestNonMagicalShortswordImmunity()
		{
			Weapon shortsword = Weapon.buildShortSword();
			// vs.
			Character player = CharacterBuilder.BuildTestDruid();
			const double startingHitPoints = 100;
			player.hitPoints = startingHitPoints;
			player.AddDamageImmunity(DamageType.Piercing, AttackKind.NonMagical);
			shortsword.Attack("Stab", player);
			const double expectedDamage = 0;
			Assert.AreEqual(startingHitPoints - expectedDamage, player.hitPoints);
			Assert.AreEqual(expectedDamage, player.LastDamagePointsTaken);
			Assert.AreEqual(DamageType.None, player.LastDamageTaken);
		}


		[TestMethod]
		public void TestMultiShortswordsAnyImmunity()
		{
			Weapon magicalShortsword = Weapon.buildMagicalShortSword();
			Weapon shortsword = Weapon.buildShortSword();
			// vs.
			Character player = CharacterBuilder.BuildTestDruid();
			const double startingHitPoints = 100;
			player.hitPoints = startingHitPoints;
			player.AddDamageImmunity(DamageType.Piercing, AttackKind.Any);
			Assert.IsTrue(player.IsImmuneTo(DamageType.Piercing, AttackKind.Magical));
			Assert.IsTrue(player.IsImmuneTo(DamageType.Piercing, AttackKind.NonMagical));
			Assert.IsTrue(player.IsImmuneTo(DamageType.Piercing, AttackKind.Any));
			Assert.IsFalse(player.IsImmuneTo(DamageType.Poison, AttackKind.Any));
			Assert.IsFalse(player.IsImmuneTo(DamageType.Psychic, AttackKind.Magical));
			Assert.IsFalse(player.IsImmuneTo(DamageType.Radiant, AttackKind.NonMagical));
			shortsword.Attack("Stab", player);
			magicalShortsword.Attack("Stab", player);
			const double expectedDamage = 0;
			Assert.AreEqual(startingHitPoints - expectedDamage, player.hitPoints);
			Assert.AreEqual(expectedDamage, player.LastDamagePointsTaken);
			Assert.AreEqual(DamageType.None, player.LastDamageTaken);
		}
	}
}
