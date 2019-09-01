using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class CurrencyTests
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
		public void TestCurrencyConversion()
		{
			Assert.AreEqual(1, DndUtils.GetGoldPieces("1 gp"));
			Assert.AreEqual(1.0 / 100, DndUtils.GetGoldPieces("1 cp"));
			Assert.AreEqual(1.0 / 10, DndUtils.GetGoldPieces("1 sp"));
			Assert.AreEqual(1.0 / 2, DndUtils.GetGoldPieces("1 ep"));
			Assert.AreEqual(10, DndUtils.GetGoldPieces("1 pp"));
		}

	}
}
