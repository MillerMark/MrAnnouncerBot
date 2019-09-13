using System;
using System.Linq;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class TurnTests
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
		public void TestTurnPartConversion()
		{
			Assert.AreEqual(TurnPart.Action, PlayerActionShortcut.GetTurnPart("1A"));
			Assert.AreEqual(TurnPart.BonusAction, PlayerActionShortcut.GetTurnPart("1BA"));
			Assert.AreEqual(TurnPart.Reaction, PlayerActionShortcut.GetTurnPart("1R"));
			Assert.AreEqual(TurnPart.Special, PlayerActionShortcut.GetTurnPart("*"));
			Assert.AreEqual(TurnPart.Action, PlayerActionShortcut.GetTurnPart(""));
		}
	}
}
