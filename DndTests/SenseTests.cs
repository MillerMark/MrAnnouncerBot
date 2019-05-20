using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DndTests
{
	[TestClass]
	public class SenseTests
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
		public void TestSenses()
		{
			Creature creature = new Creature();
			Assert.IsFalse(creature.HasSense(Senses.Darkvision));
			Assert.IsFalse(creature.HasSense(Senses.Blindsight));
			creature.blindsightRadius = 10;
			Assert.IsTrue(creature.HasSense(Senses.Blindsight));
			creature.activeConditions |= Conditions.Petrified;
			Assert.IsFalse(creature.HasSense(Senses.Blindsight));
		}
	}
}
