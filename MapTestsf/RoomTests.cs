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


		[Test]
		public void TestSegmentOverlap()
		{
			RoomSegment roomSegment1 = new RoomSegment(10, 12, 0);
			RoomSegment roomSegment2 = new RoomSegment(10, 12, 1);
			RoomSegment roomSegment3 = new RoomSegment(10, 12, 2);
			Room room = new Room(0);
			room.Segments.Add(roomSegment1);
			room.Segments.Add(roomSegment2);
			SegmentAnalysis segmentAnalysis = room.AnalyzeSegment(roomSegment3);
			Assert.IsTrue(segmentAnalysis.OverlapsLastSegment);
			Assert.AreEqual(SegmentPosition.OverlapsRoom, segmentAnalysis.Position);
		}

		public void TestRoom(string mapData, int expectedRoomCount, int roomTestIndex = -1, int expectedWidth = 0, int expectedHeight = 0)
		{
			Map map = new Map();
			map.LoadData(mapData);
			List<Room> allRooms = MapEngine.GetAllRooms(map.MapArray);
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
			Assert.AreEqual(5, map.MapArray.GetLength(0));
			Assert.AreEqual(10, map.MapArray.GetLength(1));
		}

		void TestMapArray(string mapData, int expectedMapWidth, int expectedMapHeight)
		{
			Map map = new Map();
			map.LoadData(mapData);
			Assert.AreEqual(expectedMapWidth, map.MapArray.GetLength(0));
			Assert.AreEqual(expectedMapHeight, map.MapArray.GetLength(1));
		}

		[Test]
		public void TestMapArray1x5()
		{
			TestMapArray(@"
F	F	F	F	F
", 5, 1);

			TestMapArray("F	F	F	F	F", 5, 1);
		}

		[Test]
		public void RoomTest()
		{
			Room room = new Room(2);
			room.AddSegment(0, 6, 2);
			room.AddSegment(0, 3, 3);
			room.AddSegment(0, 3, 4);
			Assert.AreEqual(3, room.Width);
			Assert.AreEqual(4, room.Height);
		}
	}
}