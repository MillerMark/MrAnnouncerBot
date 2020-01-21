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
		static CombatTests()
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
		public void TestMapAndRoomActivation()
		{
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			DndMap map = game.AddMap(new DndMap("Caves of the Wandering Winds"));
			DndRoom wizardsWorkshop = map.AddRoom(new DndRoom("Wizard's Workshop"));
			Assert.IsNull(game.ActiveMap);
			game.ActivateMap(map);
			Assert.IsNull(game.ActiveRoom);
			Assert.IsNull(map.ActiveRoom);
			game.ActivateRoom(wizardsWorkshop);
			Assert.AreEqual(map, game.ActiveMap);
			Assert.AreEqual(wizardsWorkshop, game.ActiveRoom);
			Assert.AreEqual(wizardsWorkshop, map.ActiveRoom);
		}

		[TestMethod]
		public void TestPositionCreatures()
		{
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			Character alice = game.AddPlayer(CharacterBuilder.BuildTestBarbarian("a"));
			Character betty = game.AddPlayer(CharacterBuilder.BuildTestDruid("b"));
			Character charlie = game.AddPlayer(CharacterBuilder.BuildTestElf("c"));
			Character david = game.AddPlayer(CharacterBuilder.BuildTestWizard("d"));
			DndMap map = game.AddMap(new DndMap("Caves of the Wandering Winds"));
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
			game.ActivateMap(map);
			game.ActivateRoom(dndRoom);
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
			AllPlayers.Invalidate();
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			Character ava = game.AddPlayer(AllPlayers.GetFromId(PlayerID.LilCutie));
			Monster joe = game.AddMonster(MonsterBuilder.BuildVineBlight("Joe"));
			PlayerActionShortcut greatsword = ava.GetShortcut("Greatsword");

			game.EnteringCombat();
			Assert.IsNull(game.ActiveCreature);
			ava.TestStartTurn();
			Assert.AreEqual(ava, game.ActiveCreature);
			ava.WillAttack(joe, greatsword);
			game.SetHiddenThreshold(ava, 12, DiceRollType.Attack);
			//DiceRoll diceRoll = ava.GetRoll();
			
			game.ExitingCombat();
		}
	}
}
