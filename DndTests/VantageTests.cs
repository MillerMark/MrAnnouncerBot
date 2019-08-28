using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class VantageTests
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
		public void TestStringToVantageConversion()
		{
			Assert.AreEqual(VantageKind.Advantage, DndUtils.ToVantage("advantage"));
			Assert.AreEqual(VantageKind.Disadvantage, DndUtils.ToVantage("disadvantage"));
			Assert.AreEqual(VantageKind.Normal, DndUtils.ToVantage("normal"));
			Assert.AreEqual(VantageKind.Normal, DndUtils.ToVantage(""));
			Assert.AreEqual(VantageKind.Normal, DndUtils.ToVantage("anything else"));
		}
	}
}
