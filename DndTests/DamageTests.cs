using System;
using System.Collections.Generic;
using DndCore;
using DndCore.Enums;
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
			Character barbarian1 = CharacterBuilder.BuildTestBarbarian();
			Character barbarian2 = CharacterBuilder.BuildTestBarbarian();
			Character elf = CharacterBuilder.BuildTestElf();
			Character druid = CharacterBuilder.BuildTestDruid();
			elf.creatureSize = CreatureSize.Small;
			barbarian1.creatureSize = CreatureSize.Huge;
			Assert.IsFalse(barbarian1.HasCondition(Conditions.Grappled));
			Assert.IsFalse(elf.HasCondition(Conditions.Grappled));
			Assert.IsFalse(druid.HasCondition(Conditions.Grappled));
			int attackRoll = vineBlight.GetAttackRoll(12, AttackNames.Constrict);
			Assert.AreEqual(16, attackRoll);
			barbarian1.baseArmorClass = 13;
			barbarian2.baseArmorClass = 17;
			DamageResult elfDamage = vineBlight.GetDamageFromAttack(elf, AttackNames.Constrict, 10, attackRoll);
			DamageResult barbarian1Damage = vineBlight.GetDamageFromAttack(barbarian1, AttackNames.Constrict, 10, attackRoll);
			DamageResult barbarian2Damage = vineBlight.GetDamageFromAttack(barbarian2, AttackNames.Constrict, 10, attackRoll);
			Assert.IsNull(barbarian2Damage);  // Barbarian 2's AC is 17, which should result in a miss - no damage.
			DamageResult druidDamage = vineBlight.GetDamageFromAttack(druid, AttackNames.Constrict, 12, attackRoll);
			Assert.IsFalse(barbarian1Damage.HasCondition(Conditions.Grappled));  // barbarian 1 is huge => no damage.
			Assert.IsTrue(elfDamage.HasCondition(Conditions.Grappled));
			Assert.IsFalse(druidDamage.HasCondition(Conditions.Grappled));  // druid saving throw avoids the grapple.
			Assert.AreEqual(-9, elfDamage.hitPointChange);
			Assert.AreEqual(-9, barbarian1Damage.hitPointChange);
			Assert.AreEqual(-9, druidDamage.hitPointChange);
			Assert.AreEqual(DamageType.Bludgeoning | DamageType.Condition, elfDamage.damageTypes);
			Assert.AreEqual(DamageType.Bludgeoning, barbarian1Damage.damageTypes);
			Assert.AreEqual(DamageType.Bludgeoning, druidDamage.damageTypes);
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
			shortSword.Attack(AttackNames.Stab, player);
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
			shortSword.Attack(AttackNames.Stab, player);
			shortSword.Attack(AttackNames.Stab, player);
			shortSword.Attack(AttackNames.Stab, player);
			shortSword.Attack(AttackNames.Stab, player);
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
			shortSword.Attack(AttackNames.Stab, player);
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
			magicalShortsword.Attack(AttackNames.Stab, player);
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
			shortsword.Attack(AttackNames.Stab, player);
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
			shortsword.Attack(AttackNames.Stab, player);
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
			shortsword.Attack(AttackNames.Stab, player);
			magicalShortsword.Attack(AttackNames.Stab, player);
			const double expectedDamage = 0;
			Assert.AreEqual(startingHitPoints - expectedDamage, player.hitPoints);
			Assert.AreEqual(expectedDamage, player.LastDamagePointsTaken);
			Assert.AreEqual(DamageType.None, player.LastDamageTaken);
		}
	}
}
