using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

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
			DndGame dndGame = DndGame.Instance;
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
			DndGame dndGame = DndGame.Instance;
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
			Assert.AreEqual(new Vector(DndMap.SquareSide * 5, DndMap.SquareSide * 3), alice.WorldPosition);
			Assert.AreEqual(new Vector(DndMap.SquareSide * 8, DndMap.SquareSide * 6), betty.WorldPosition);
			Assert.AreEqual(new Vector(DndMap.SquareSide * 13, DndMap.SquareSide * 4), charlie.WorldPosition);
			Assert.AreEqual(new Vector(DndMap.SquareSide * 12, DndMap.SquareSide * 11), david.WorldPosition);

			//dndGame.EnterCombat(true);
			//ergo.QueueAction(new ActionAttack(alice, AttackNames.Constrict));
		}

		[TestMethod]
		public void TestTurnEvents()
		{
			AllPlayers.LoadData();
			DndGame dndGame = DndGame.Instance;
			dndGame.Reset();
			Character ava = dndGame.AddPlayer(AllPlayers.GetFromId(PlayerID.Ava));
			Monster joe = dndGame.AddMonster(MonsterBuilder.BuildVineBlight("Joe"));
			PlayerActionShortcut greatsword = ava.GetShortcut("Greatsword, +1");

			dndGame.EnteringCombat();
			Assert.IsNull(dndGame.ActiveCreature);
			ava.StartTurn();
			Assert.AreEqual(ava, dndGame.ActiveCreature);
			ava.Use(greatsword);
			ava.Target(joe);
			dndGame.SetHiddenThreshold(12);
			//DiceRoll diceRoll = ava.GetRoll();
			
			dndGame.ExitingCombat();
		}
	}
}
