using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DndTests
{
	[TestClass]
	public class CombatTests
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
		public void TestInitiative()
		{
			//DndGame dndGame = new DndGame();
			//Character alice = dndGame.AddPlayer(CharacterBuilder.BuildTestBarbarian("a"));
			//Character betty = dndGame.AddPlayer(CharacterBuilder.BuildTestDruid("b"));
			//Character charlie = dndGame.AddPlayer(CharacterBuilder.BuildTestElf("c"));
			//Character david = dndGame.AddPlayer(CharacterBuilder.BuildTestWizard("d"));
			//Monster ergo = dndGame.AddMonster(MonsterBuilder.BuildVineBlight("e"));
			//Map map = dndGame.AddMap(new DndMap("Caves of Windraft"));
			//map.PositionCreatures(@"
   //    ┌───────────────┐
			// │               │
			// │               │
			// │    a          │
			// │            c  │
			// │               │
			// │       b       │
			// │               │
			// │               │
			// │               │
			// │               │
			// │           d   │
			// │               │
			// │    e          │
			// │               │
			// │               │
			// └───────────────┘");
			//dndGame.ActivateMap(map);
			//dndGame.EnterCombat(true);
			//dndGame.QueueAction(alice, new ActionAttack(ergo, ""));
		}
	}
}
