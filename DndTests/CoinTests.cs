using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class CoinTests
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
		public void TestCoinsFrom1_25()
		{
			Coins coins = new Coins();
			coins.SetFromText("1.25");
			Assert.AreEqual(1, coins.NumGold);
			Assert.AreEqual(2, coins.NumSilver);
			Assert.AreEqual(5, coins.NumCopper);
			Assert.AreEqual(0, coins.NumElectrum);
			Assert.AreEqual(0, coins.NumPlatinum);
		}

		[TestMethod]
		public void TestCoinsFrom1_75()
		{
			Coins coins = new Coins();
			coins.SetFromText("1.75");
			Assert.AreEqual(1, coins.NumGold);
			Assert.AreEqual(1, coins.NumElectrum);
			Assert.AreEqual(2, coins.NumSilver);
			Assert.AreEqual(5, coins.NumCopper);
			Assert.AreEqual(0, coins.NumPlatinum);
		}

		[TestMethod]
		public void TestCoinsFromNegative41_99Str()
		{
			Coins coins = new Coins();
			coins.SetFromText("-41.99");
			Assert.AreEqual(-1, coins.NumGold);
			Assert.AreEqual(-1, coins.NumElectrum);
			Assert.AreEqual(-4, coins.NumSilver);
			Assert.AreEqual(-9, coins.NumCopper);
			Assert.AreEqual(-4, coins.NumPlatinum);
		}

		[TestMethod]
		public void TestNegative41_99()
		{
			Coins coins = new Coins();
			coins.SetFromGold(-41.99);
			Assert.AreEqual(-1, coins.NumGold);
			Assert.AreEqual(-1, coins.NumElectrum);
			Assert.AreEqual(-4, coins.NumSilver);
			Assert.AreEqual(-9, coins.NumCopper);
			Assert.AreEqual(-4, coins.NumPlatinum);
		}

		[TestMethod]
		public void TestTotalGold()
		{
			Coins coins = new Coins();
			coins.NumCopper = 3;
			coins.NumSilver = 4;
			coins.NumElectrum = 1;
			coins.NumGold = 3;
			coins.NumPlatinum = 5;
			Assert.AreEqual(53.93, coins.TotalGold);
		}
	}
}
