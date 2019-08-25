using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class SpellTests
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
		private const int Cantrip = 0;
		[TestMethod]
		public void TestRayOfFrost()
		{
			Spell rayOfFrost = AllSpells.Get("Ray of Frost");
			Assert.IsNotNull(rayOfFrost);
			Assert.AreEqual(DndTimeSpan.OneAction, rayOfFrost.CastingTime);
			Assert.AreEqual(DndTimeSpan.Zero, rayOfFrost.Duration);
			Assert.IsTrue(rayOfFrost.HasRange(60, SpellRangeType.DistanceFeet));
			Assert.IsFalse(rayOfFrost.RequiresConcentration);
			Assert.AreEqual(Cantrip, rayOfFrost.Level);
			Assert.AreEqual(SpellComponents.Somatic | SpellComponents.Verbal, rayOfFrost.Components);
			Assert.AreEqual("1d8(cold)", rayOfFrost.DieStr);
		}

		[TestMethod]
		public void TestPlayerLevels()
		{
			Assert.AreEqual("1d8(cold)", AllSpells.Get("Ray of Frost", 1, 1).DieStr);
			Assert.AreEqual("1d8(cold)", AllSpells.Get("Ray of Frost", 1, 4).DieStr);
			Assert.AreEqual("2d8(cold)", AllSpells.Get("Ray of Frost", 1, 5).DieStr);
			Assert.AreEqual("2d8(cold)", AllSpells.Get("Ray of Frost", 1, 10).DieStr);
			Assert.AreEqual("3d8(cold)", AllSpells.Get("Ray of Frost", 1, 11).DieStr);
			Assert.AreEqual("3d8(cold)", AllSpells.Get("Ray of Frost", 1, 16).DieStr);
			Assert.AreEqual("4d8(cold)", AllSpells.Get("Ray of Frost", 1, 17).DieStr);
			Assert.AreEqual("4d8(cold)", AllSpells.Get("Ray of Frost", 1, 22).DieStr);
		}

		[TestMethod]
		public void TestSpellSlotLevels()
		{
			Assert.AreEqual("2d8,1d6", AllSpells.Get("Chaos Bolt", 1).DieStr);
			Assert.AreEqual("2d8,2d6", AllSpells.Get("Chaos Bolt", 2).DieStr);
			Assert.AreEqual("2d8,3d6", AllSpells.Get("Chaos Bolt", 3).DieStr);
			Assert.AreEqual("2d8,4d6", AllSpells.Get("Chaos Bolt", 4).DieStr);
			Assert.AreEqual("2d8,5d6", AllSpells.Get("Chaos Bolt", 5).DieStr);
			Assert.AreEqual("2d8,6d6", AllSpells.Get("Chaos Bolt", 6).DieStr);
			Assert.AreEqual("2d8,7d6", AllSpells.Get("Chaos Bolt", 7).DieStr);
		}

		[TestMethod]
		public void TestSpellcastingAbilityModifier()
		{
			AllSpells.Get("Cure Wounds", 1);
		}


		[TestMethod]
		public void TestChaosBolt()
		{
			Spell chaosBolt = AllSpells.Get("Chaos Bolt");
			Assert.IsNotNull(chaosBolt);
			Assert.AreEqual(DndTimeSpan.OneAction, chaosBolt.CastingTime);
			Assert.AreEqual(DndTimeSpan.Zero, chaosBolt.Duration);
			Assert.IsTrue(chaosBolt.HasRange(120, SpellRangeType.DistanceFeet));
			Assert.IsFalse(chaosBolt.RequiresConcentration);
			Assert.AreEqual(1, chaosBolt.Level);
			Assert.AreEqual(SpellComponents.Somatic | SpellComponents.Verbal, chaosBolt.Components);
		}
	}
}
