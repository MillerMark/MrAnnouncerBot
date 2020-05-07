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
			Assert.AreEqual(Conditions.Fatigued, skeleton.damageImmunities);
			
			
		}
	}
}
