using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	
	[TestClass]
	public class SkillTests
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
		public void TestStringToSkillConversion()
		{
			Assert.AreEqual(Skills.nature | Skills.religion, DndUtils.ToSkill("nature,religion"));
			Assert.AreEqual(Skills.sleightOfHand, DndUtils.ToSkill("sleightOfHand"));
			Assert.AreEqual(Skills.sleightOfHand, DndUtils.ToSkill("sleight of hand"));
			Assert.AreEqual(Skills.animalHandling, DndUtils.ToSkill("animal handling"));
		}
	}
}
