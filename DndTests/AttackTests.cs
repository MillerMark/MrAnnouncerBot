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
		public void TestModifierCalculation()
		{
			Character barbarian = CharacterBuilder.BuildTestBarbarian("Joe");
			barbarian.SetAbilities(16, 11, 14, 8, 8, 18);
			Assert.AreEqual(3, barbarian.GetAttackingAbilityModifier(WeaponProperties.Finesse, AttackType.Range));
			Assert.AreEqual(3, barbarian.GetAttackingAbilityModifier(WeaponProperties.Martial | WeaponProperties.Versatile, AttackType.Melee));
			Assert.AreEqual(3, barbarian.GetAttackingAbilityModifier(WeaponProperties.Martial | WeaponProperties.TwoHanded | WeaponProperties.Heavy, AttackType.Melee));
		}

		[TestMethod]
		public void TestDamageConversion()
		{
			Assert.AreEqual(DamageType.Fire, DndUtils.ToDamage("fire"));
			Assert.AreEqual(DamageType.Force, DndUtils.ToDamage("force"));
			Assert.AreEqual(DamageType.Lightning, DndUtils.ToDamage("lightning"));
			Assert.AreEqual(DamageType.Necrotic, DndUtils.ToDamage("necrotic"));
			Assert.AreEqual(DamageType.Piercing, DndUtils.ToDamage("piercing"));
			Assert.AreEqual(DamageType.Poison, DndUtils.ToDamage("poison"));
			Assert.AreEqual(DamageType.Psychic, DndUtils.ToDamage("psychic"));
			Assert.AreEqual(DamageType.Radiant, DndUtils.ToDamage("radiant"));
			Assert.AreEqual(DamageType.Slashing, DndUtils.ToDamage("slashing"));
			Assert.AreEqual(DamageType.Thunder, DndUtils.ToDamage("thunder"));
			Assert.AreEqual(DamageType.Condition, DndUtils.ToDamage("condition"));
			Assert.AreEqual(DamageType.Bane, DndUtils.ToDamage("bane"));
			Assert.AreEqual(DamageType.DamageAdd, DndUtils.ToDamage("damageadd"));
			Assert.AreEqual(DamageType.DamageSubtract, DndUtils.ToDamage("damagesubtract"));
			Assert.AreEqual(DamageType.Bless, DndUtils.ToDamage("bless"));
			Assert.AreEqual(DamageType.Acid, DndUtils.ToDamage("acid"));
			Assert.AreEqual(DamageType.Bludgeoning, DndUtils.ToDamage("bludgeoning"));
			Assert.AreEqual(DamageType.Acid | DamageType.Bludgeoning, DndUtils.ToDamage("acid, bludgeoning"));
			Assert.AreEqual(DamageType.Cold, DndUtils.ToDamage("cold"));
			Assert.AreEqual(DamageType.None, DndUtils.ToDamage("scooby doo"));
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
}
