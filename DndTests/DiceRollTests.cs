using System;
using System.Collections.Generic;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class DiceRollTests
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
		public void TestChaosBolt()
		{
			AllPlayers.Invalidate();
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			Character merkin = AllPlayers.GetFromId(PlayerID.Merkin);
			game.AddPlayer(merkin);

			List<PlayerActionShortcut> actionShortcuts = AllActionShortcuts.Get(AllPlayers.GetPlayerIdFromName("Merkin"), "Chaos Bolt");
			Assert.AreEqual(3, actionShortcuts.Count);
			DiceRoll chaosBoltLevel3 = DiceRoll.GetFrom(actionShortcuts[2]);
			Assert.IsTrue(chaosBoltLevel3.IsMagic);
			Assert.AreEqual(DiceRollType.ChaosBolt, chaosBoltLevel3.Type);
			Assert.AreEqual("2d8(),3d6()", chaosBoltLevel3.DamageHealthExtraDice);
		}
	}
}
