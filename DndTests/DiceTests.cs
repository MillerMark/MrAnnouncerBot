using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DndTests
{

	[TestClass]
	public class DiceTests
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
		public void TestStringGeneration()
		{
			Assert.AreEqual("1d8", Dice.d8x1);
			Assert.AreEqual("1d8", Dice.d8x1.Plus(0));
			Assert.AreEqual("11d10+44", Dice.d10x11.Plus(44));
			Assert.AreEqual("2d4+32", Dice.d4x2.Plus(32));
			Assert.AreEqual("2d6+3", Dice.d6x2.Plus(3));
			Assert.AreEqual("2d6+2", Dice.d6x2.Plus(2));
			Assert.AreEqual("2d10+3", Dice.d10x2.Plus(3));
			Assert.AreEqual("4d8+8", Dice.d8x4.Plus(8));

		}
	}
}
