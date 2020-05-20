using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DndTests
{
	[TestClass]
	public class MonsterTests
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
		public void TestVrock()
		{
			Monster vrock = MonsterBuilder.BuildVrock();
			Assert.IsTrue(vrock.IsResistantTo(DamageType.Cold, AttackKind.NonMagical));
			Assert.IsTrue(vrock.IsResistantTo(DamageType.Fire, AttackKind.NonMagical));
			Assert.IsTrue(vrock.IsResistantTo(DamageType.Lightning, AttackKind.NonMagical));
			Assert.IsTrue(vrock.IsResistantTo(DamageType.Bludgeoning, AttackKind.NonMagical));
			Assert.IsTrue(vrock.IsResistantTo(DamageType.Piercing, AttackKind.NonMagical));
			Assert.IsTrue(vrock.IsResistantTo(DamageType.Slashing, AttackKind.NonMagical));
			Assert.IsTrue(vrock.IsImmuneTo(DamageType.Poison, AttackKind.Any));
			Assert.IsTrue(vrock.IsImmuneTo(Conditions.Poisoned));
		}

			[TestMethod]
		public void TestSkeleton()
		{
			Monster skeleton = AllMonsters.Get("Skeleton");
			Assert.AreEqual(13, skeleton.ArmorClass);
			Assert.AreEqual(13, skeleton.maxHitPoints);
			Assert.AreEqual(CreatureSize.Medium, skeleton.creatureSize);
			Assert.AreEqual(CreatureKinds.Undead, skeleton.kind);
			Assert.AreEqual(Alignment.LawfulEvil, skeleton.Alignment);
			Assert.AreEqual("lawful evil", skeleton.alignmentStr);
			Assert.AreEqual(30, skeleton.baseWalkingSpeed);
			Assert.AreEqual(10, skeleton.baseStrength);
			Assert.AreEqual(0, skeleton.strengthMod);
			Assert.AreEqual(14, skeleton.baseDexterity);
			Assert.AreEqual(2, skeleton.dexterityMod);
			Assert.AreEqual(15, skeleton.baseConstitution);
			Assert.AreEqual(2, skeleton.constitutionMod);
			Assert.AreEqual(6, skeleton.baseIntelligence);
			Assert.AreEqual(-2, skeleton.intelligenceMod);
			Assert.AreEqual(8, skeleton.baseWisdom);
			Assert.AreEqual(-1, skeleton.wisdomMod);
			Assert.AreEqual(5, skeleton.baseCharisma);
			Assert.AreEqual(-3, skeleton.charismaMod);
			Assert.AreEqual(60, skeleton.darkvisionRadius);
			Assert.AreEqual(9, skeleton.passivePerception);
			Assert.AreEqual(1/4, skeleton.challengeRating);
			Assert.AreEqual(DamageType.Poison, skeleton.damageImmunities);
			Assert.AreEqual(Conditions.Fatigued, skeleton.conditionImmunities);
		}

		[TestMethod]
		public void TestGiantBat()
		{
			Monster giantBat = AllMonsters.Get("Giant Bat");
			Assert.AreEqual(13, giantBat.ArmorClass);
			Assert.AreEqual(22, giantBat.maxHitPoints);
			Assert.AreEqual(CreatureSize.Large, giantBat.creatureSize);
			Assert.AreEqual(CreatureKinds.Beast, giantBat.kind);
			Assert.AreEqual(Alignment.Unaligned, giantBat.Alignment);
			Assert.AreEqual("unaligned", giantBat.alignmentStr);
			Assert.AreEqual(10, giantBat.baseWalkingSpeed);
			Assert.AreEqual(60, giantBat.flyingSpeed);
			Assert.AreEqual(15, giantBat.baseStrength);
			Assert.AreEqual(2, giantBat.strengthMod);
			Assert.AreEqual(16, giantBat.baseDexterity);
			Assert.AreEqual(3, giantBat.dexterityMod);
			Assert.AreEqual(11, giantBat.baseConstitution);
			Assert.AreEqual(0, giantBat.constitutionMod);
			Assert.AreEqual(2, giantBat.baseIntelligence);
			Assert.AreEqual(-4, giantBat.intelligenceMod);
			Assert.AreEqual(12, giantBat.baseWisdom);
			Assert.AreEqual(1, giantBat.wisdomMod);
			Assert.AreEqual(6, giantBat.baseCharisma);
			Assert.AreEqual(-2, giantBat.charismaMod);
			Assert.AreEqual(0, giantBat.darkvisionRadius);
			Assert.AreEqual(60, giantBat.blindsightRadius);
			Assert.AreEqual(11, giantBat.passivePerception);
			Assert.AreEqual(1/4, giantBat.challengeRating);
			Assert.AreEqual(DamageType.None, giantBat.damageImmunities);
			Assert.AreEqual(Conditions.None, giantBat.conditionImmunities);
		}

		[TestMethod]
		public void TestGiantApe()
		{
			Monster giantApe = AllMonsters.Get("Giant Ape");
			Assert.AreEqual(12, giantApe.ArmorClass);
			Assert.AreEqual(157, giantApe.maxHitPoints);
			Assert.AreEqual(CreatureSize.Huge, giantApe.creatureSize);
			Assert.AreEqual(CreatureKinds.Beast, giantApe.kind);
			Assert.AreEqual(Alignment.Unaligned, giantApe.Alignment);
			Assert.AreEqual("unaligned", giantApe.alignmentStr);
			Assert.AreEqual(40, giantApe.baseWalkingSpeed);
			Assert.AreEqual(0, giantApe.flyingSpeed);
			Assert.AreEqual(40, giantApe.climbingSpeed);
			Assert.AreEqual(23, giantApe.baseStrength);
			Assert.AreEqual(6, giantApe.strengthMod);
			Assert.AreEqual(14, giantApe.baseDexterity);
			Assert.AreEqual(2, giantApe.dexterityMod);
			Assert.AreEqual(18, giantApe.baseConstitution);
			Assert.AreEqual(4, giantApe.constitutionMod);
			Assert.AreEqual(7, giantApe.baseIntelligence);
			Assert.AreEqual(-2, giantApe.intelligenceMod);
			Assert.AreEqual(12, giantApe.baseWisdom);
			Assert.AreEqual(1, giantApe.wisdomMod);
			Assert.AreEqual(7, giantApe.baseCharisma);
			Assert.AreEqual(-2, giantApe.charismaMod);
			Assert.AreEqual(0, giantApe.darkvisionRadius);
			Assert.AreEqual(0, giantApe.blindsightRadius);
			Assert.AreEqual(14, giantApe.passivePerception);
			Assert.AreEqual(9, giantApe.GetOverridingSkillMod(Skills.athletics));
			Assert.AreEqual(4, giantApe.GetOverridingSkillMod(Skills.perception));
			Assert.AreEqual(7, giantApe.challengeRating);
		}
	}
}
