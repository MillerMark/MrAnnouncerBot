using DndCore;
using DndCore.Enums;
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
	}
}
