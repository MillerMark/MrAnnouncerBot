using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class MultiClassTests
	{
		static MultiClassTests()
		{
			Folders.UseTestData = true;
		}
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
		public void TestRaceClass()
		{
			AllPlayers.LoadData();
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			Assert.AreEqual("Lizardfolk Fighter 4 / Barbarian 1", fred.raceClass);

			Character ava = AllPlayers.GetFromId(PlayerID.Ava);
			Assert.AreEqual("Human Paladin 5", ava.raceClass);
		}
	}
}
