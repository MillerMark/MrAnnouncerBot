using System;
using System.Collections.Generic;
using System.Linq;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{

	[TestClass]
	public class DamageTests
	{
		static DamageTests()
		{
			Folders.UseTestData = true;
		}

		[TestMethod]
		public void TestToHitBonusAndDamageDie()
		{
			

			List<PlayerActionShortcut> battleaxes = AllActionShortcuts.Get(PlayerID.Ava, "Battleaxe");
			PlayerActionShortcut battleaxe2H = battleaxes.FirstOrDefault(x => x.Name.IndexOf("(2H)") > 0);
			PlayerActionShortcut battleaxe1H = battleaxes.FirstOrDefault(x => x.Name.IndexOf("(1H)") > 0);
			Assert.IsNotNull(battleaxe1H);
			Assert.IsNotNull(battleaxe2H);
			Assert.AreEqual("1d8+3(slashing)", battleaxe1H.Dice);
			Assert.AreEqual("1d10+3(slashing)", battleaxe2H.Dice);
			Assert.AreEqual(6, battleaxe1H.ToHitModifier);
			Assert.AreEqual(6, battleaxe2H.ToHitModifier);

			PlayerActionShortcut greatsword = AllActionShortcuts.Get(PlayerID.Ava, "Greatsword")[0];
			Assert.IsNotNull(greatsword);
			Assert.AreEqual("2d6+4(slashing)", greatsword.Dice);
			Assert.AreEqual(7, greatsword.ToHitModifier);
		}

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
