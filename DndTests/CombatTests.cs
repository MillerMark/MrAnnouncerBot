using DndCore;
using DndCore.CoreClasses;
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
		public void TestMapAndRoomActivation()
		{
			DndGame dndGame = new DndGame();
			DndMap map = dndGame.AddMap(new DndMap("Caves of the Wandering Winds"));
			DndRoom wizardsWorkshop = map.AddRoom(new DndRoom("Wizard's Workshop"));
			Assert.IsNull(dndGame.ActiveMap);
			dndGame.ActivateMap(map);
			Assert.IsNull(dndGame.ActiveRoom);
			Assert.IsNull(map.ActiveRoom);
			dndGame.ActivateRoom(wizardsWorkshop);
			Assert.AreEqual(map, dndGame.ActiveMap);
			Assert.AreEqual(wizardsWorkshop, dndGame.ActiveRoom);
			Assert.AreEqual(wizardsWorkshop, map.ActiveRoom);
		}

		[TestMethod]
		public void TestPositionCreatures()
		{
			DndGame dndGame = new DndGame();
			Character alice = dndGame.AddPlayer(CharacterBuilder.BuildTestBarbarian("a"));
			Character betty = dndGame.AddPlayer(CharacterBuilder.BuildTestDruid("b"));
			Character charlie = dndGame.AddPlayer(CharacterBuilder.BuildTestElf("c"));
			Character david = dndGame.AddPlayer(CharacterBuilder.BuildTestWizard("d"));
			DndMap map = dndGame.AddMap(new DndMap("Caves of the Wandering Winds"));
			DndRoom dndRoom = map.AddRoom(new DndRoom("Wizard's Workshop"));
			Assert.AreEqual(new Vector(0, 0), alice.WorldPosition);
			Assert.AreEqual(Vector.zero, betty.WorldPosition);
			dndRoom.PositionCreatures(@"
┌───────────────┐
│               │
│               │
│    a          │
│            c  │
│               └───────────────┐
│       b                       │
│               				        │
│                               │
│                               │
│               ┌───────────────┘
│           d   │
│               │
│    e          │
│               │
│               │
└───────────────┘");
			dndGame.ActivateMap(map);
			dndGame.ActivateRoom(dndRoom);
			Assert.AreEqual(new Vector(5, 3), alice.WorldPosition);
			Assert.AreEqual(new Vector(8, 6), betty.WorldPosition);
			Assert.AreEqual(new Vector(13, 4), charlie.WorldPosition);
			Assert.AreEqual(new Vector(12, 11), david.WorldPosition);

			//dndGame.EnterCombat(true);
			//ergo.QueueAction(new ActionAttack(alice, AttackNames.Constrict));
		}
	}
}
