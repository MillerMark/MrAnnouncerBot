using NUnit.Framework;
using System.Collections.Generic;
using MapCore;

namespace Tests
{
	public class RoomTests
	{
		[SetUp]
		public void Setup()
		{
		}

		public void TestRoom(string mapData, int expectedRoomCount, int roomTestIndex = -1, int expectedWidth = 0, int expectedHeight = 0)
		{
			Map map = new Map();
			map.LoadData(mapData);
			List<Room> allRooms = map.Rooms;
			Assert.AreEqual(expectedRoomCount, allRooms.Count);
			if (roomTestIndex >= 0)
			{
				Assert.AreEqual(expectedWidth, allRooms[roomTestIndex].Width);
				Assert.AreEqual(expectedHeight, allRooms[roomTestIndex].Height);
			}
		}

		[Test]
		public void TestMapLoadSimpleRoom()
		{
			const string simple3x4 = @"
		F	F	F
		F	F	F
		F	F	F
		F	F	F
";
			const string oneRoomHallwayOnBottomLeft = @"
		F	F	F
		F	F	F
		F	F	F
		F	F	F
		F
		F
		F
";
			TestRoom(oneRoomHallwayOnBottomLeft, 1, 0, 3, 4);
			TestRoom(simple3x4, 1, 0, 3, 4);
		}

		[Test]
		public void TestMapLoadTwoRoomsOneCorridor()
		{
			const string twoRoomsConnectedByVerticalHallway = @"
		F	F	F
		F	F	F
		F	F	F
		F
		F
		F
	F	F	F	F
	F	F	F	F
	F	F	F	F
	F	F	F	F";
			TestRoom(twoRoomsConnectedByVerticalHallway, 2, 0, 3, 3);
			TestRoom(twoRoomsConnectedByVerticalHallway, 2, 1, 4, 4);
		}

		[Test]
		public void TestMapArray()
		{
			const string twoRoomsConnectedByVerticalHallway = @"
		F	F	F
		F	F	F
		F	F	F
		F
		F
		F
	F	F	F	F
	F	F	F	F
	F	F	F	F
F	F	F	F	F";
			Map map = new Map();
			map.LoadData(twoRoomsConnectedByVerticalHallway);
			Assert.AreEqual(5, map.NumColumns);
			Assert.AreEqual(10, map.NumRows);
		}

		void TestMapArray(string mapData, int expectedMapWidth, int expectedMapHeight)
		{
			Map map = new Map();
			map.LoadData(mapData);
			Assert.AreEqual(expectedMapWidth, map.AllSpacesArray.GetLength(0));
			Assert.AreEqual(expectedMapHeight, map.AllSpacesArray.GetLength(1));
		}

		[Test]
		public void TestMapArray1x5()
		{
			TestMapArray(@"
F	F	F	F	F
", 5, 1);

			TestMapArray("F	F	F	F	F", 5, 1);
		}
	}
}