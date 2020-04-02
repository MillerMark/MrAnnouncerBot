using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{

	[TestClass]
	public class DieRollDetailTests
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
		public void TestRoll()
		{
			Roll roll = Roll.From("1d4");
			Assert.AreEqual(1, roll.Count);
			Assert.AreEqual(0, roll.Offset);
			Assert.AreEqual(4, roll.Sides);
			Assert.AreEqual("", roll.Descriptor);

			roll = Roll.From("3d8+3(cold)");
			Assert.AreEqual(3, roll.Count);
			Assert.AreEqual(3, roll.Offset);
			Assert.AreEqual(8, roll.Sides);
			Assert.AreEqual("(cold)", roll.Descriptor);
			
			roll = Roll.From("0.166667d6");
			Assert.AreEqual(0.166667, roll.Count);
			Assert.AreEqual(0, roll.Offset);
			Assert.AreEqual(6, roll.Sides);
			Assert.AreEqual("", roll.Descriptor);
		}

		[TestMethod]
		public void TestToStringMethods()
		{
			Assert.AreEqual("1d4", DieRollDetails.From("1d4").ToString());
			Assert.AreEqual("3d6", DieRollDetails.From("3d6").ToString());
			Assert.AreEqual("5d8+1", DieRollDetails.From("5d8+1").ToString());
			Assert.AreEqual("7d10+3", DieRollDetails.From("7d10+3").ToString());
			Assert.AreEqual("9d12(cold)", DieRollDetails.From("9d12(cold)").ToString());
			Assert.AreEqual("6d12(fire),3d4+2(cold)", DieRollDetails.From("6d12(fire),3d4+2(cold)").ToString());
			Assert.AreEqual("3d12(necrotic/radiant),3d4+2(cold),1d20", DieRollDetails.From("3d12(necrotic/radiant),3d4+2(cold),1d20").ToString());
		}

		[TestMethod]
		public void TestDieRollDetails()
		{
			Roll roll = DieRollDetails.From("3d12").Rolls[0];
			Assert.AreEqual("3d12", roll.ToString());
			roll.Offset = 4;
			Assert.AreEqual("3d12+4", roll.ToString());
			roll.Descriptor = "(necrotic)";
			Assert.AreEqual("3d12+4(necrotic)", roll.ToString());

			DieRollDetails rollDetails = DieRollDetails.From("1d4");
			Assert.AreEqual(1, rollDetails.Rolls.Count);
			Assert.AreEqual(0, rollDetails.Rolls[0].Offset);
			Assert.AreEqual(4, rollDetails.Rolls[0].Sides);
			Assert.AreEqual("", rollDetails.Rolls[0].Descriptor);
			Assert.AreEqual("1d4", rollDetails.ToString());

			rollDetails = DieRollDetails.From("3d8+3(cold)");
			Assert.AreEqual(1, rollDetails.Rolls.Count);
			Assert.AreEqual(3, rollDetails.Rolls[0].Count);
			Assert.AreEqual(3, rollDetails.Rolls[0].Offset);
			Assert.AreEqual(8, rollDetails.Rolls[0].Sides);
			Assert.AreEqual("(cold)", rollDetails.Rolls[0].Descriptor);
			Assert.AreEqual("3d8+3(cold)", rollDetails.ToString());
			

			rollDetails = DieRollDetails.From("2d10+3(cold),5d6(fire)");
			Assert.AreEqual(2, rollDetails.Rolls.Count);
			
			Assert.AreEqual(2, rollDetails.Rolls[0].Count);
			Assert.AreEqual(3, rollDetails.Rolls[0].Offset);
			Assert.AreEqual(10, rollDetails.Rolls[0].Sides);
			Assert.AreEqual("(cold)", rollDetails.Rolls[0].Descriptor);
			Assert.AreEqual("2d10+3(cold),5d6(fire)", rollDetails.ToString());
			
			Assert.AreEqual(5, rollDetails.Rolls[1].Count);
			Assert.AreEqual(0, rollDetails.Rolls[1].Offset);
			Assert.AreEqual(6, rollDetails.Rolls[1].Sides);
			Assert.AreEqual("(fire)", rollDetails.Rolls[1].Descriptor);
		}

		[TestMethod]
		public void TestSpellcastingAbilityModifier()
		{
			DieRollDetails rollDetails = DieRollDetails.From("1d4+SAM(health)", +3);
			Assert.AreEqual(1, rollDetails.Rolls.Count);
			Assert.AreEqual(3, rollDetails.Rolls[0].Offset);
			Assert.AreEqual(4, rollDetails.Rolls[0].Sides);
			Assert.AreEqual("(health)", rollDetails.Rolls[0].Descriptor);
			Assert.AreEqual("1d4+3(health)", rollDetails.ToString());
		}

		[TestMethod]
		public void TestCureWounds()
		{
			Assert.AreEqual("1d8+3(healing)", AllSpells.Get("Cure Wounds", 1, 5, +3).DieStr);
			Assert.AreEqual("2d8+3(healing)", AllSpells.Get("Cure Wounds", 2, 5, +3).DieStr);
			Assert.AreEqual("3d8+3(healing)", AllSpells.Get("Cure Wounds", 3, 5, 3).DieStr);
			Assert.AreEqual("4d8+3(healing)", AllSpells.Get("Cure Wounds", 4, 5, 3).DieStr);
			Assert.AreEqual("5d8+3(healing)", AllSpells.Get("Cure Wounds", 5, 5, 3).DieStr);
			Assert.AreEqual("6d8+3(healing)", AllSpells.Get("Cure Wounds", 6, 5, 3).DieStr);
			Assert.AreEqual("7d8+3(healing)", AllSpells.Get("Cure Wounds", 7, 5, 3).DieStr);
		}
	}
}
