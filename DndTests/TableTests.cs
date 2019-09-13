using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class TableTests
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
		public void TestGetTableData()
		{
			Assert.AreEqual(2, AllTables.GetData("Barbarian", "Rages", "Level", 1));  // 2 Rages at levels 1 & 2
			Assert.AreEqual(2, AllTables.GetData("Barbarian", "Rages", "Level", 2));

			Assert.AreEqual(3, AllTables.GetData("Barbarian", "Rages", "Level", 3));
			Assert.AreEqual(3, AllTables.GetData("Barbarian", "Rages", "Level", 4));
			Assert.AreEqual(3, AllTables.GetData("Barbarian", "Rages", "Level", 5));

			Assert.AreEqual(4, AllTables.GetData("Barbarian", "Rages", "Level", 6));
			Assert.AreEqual(4, AllTables.GetData("Barbarian", "Rages", "Level", 7));
			Assert.AreEqual(4, AllTables.GetData("Barbarian", "Rages", "Level", 8));
			Assert.AreEqual(4, AllTables.GetData("Barbarian", "Rages", "Level", 9));
			Assert.AreEqual(4, AllTables.GetData("Barbarian", "Rages", "Level", 10));
			Assert.AreEqual(4, AllTables.GetData("Barbarian", "Rages", "Level", 11));

			Assert.AreEqual(5, AllTables.GetData("Barbarian", "Rages", "Level", 12));
			Assert.AreEqual(5, AllTables.GetData("Barbarian", "Rages", "Level", 13));
			Assert.AreEqual(5, AllTables.GetData("Barbarian", "Rages", "Level", 14));
			Assert.AreEqual(5, AllTables.GetData("Barbarian", "Rages", "Level", 15));
			Assert.AreEqual(5, AllTables.GetData("Barbarian", "Rages", "Level", 16));

			Assert.AreEqual(6, AllTables.GetData("Barbarian", "Rages", "Level", 17));
			Assert.AreEqual(6, AllTables.GetData("Barbarian", "Rages", "Level", 18));
			Assert.AreEqual(6, AllTables.GetData("Barbarian", "Rages", "Level", 19));

			Assert.AreEqual(int.MaxValue, AllTables.GetData("Barbarian", "Rages", "Level", 20));
		}
	}
}
