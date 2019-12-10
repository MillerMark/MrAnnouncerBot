using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public static class MapEngine
	{
		static bool AddSegmentToExistingRooms(List<Room> inProgressRooms, RoomSegment roomSegment)
		{
			bool result = false;

			foreach (var room in inProgressRooms)
			{
				SegmentAnalysis segmentPosition = room.AnalyzeSegment(roomSegment);
				if (segmentPosition.Position == SegmentPosition.OverlapsRoom)
				{
					room.AddSegment(roomSegment);
					result = true;
				}
			}
			return result;
		}

		private static void FinishRooms(List<Room> inProgressRooms, int column, List<Room> foundRooms)
		{
			for (int i = inProgressRooms.Count - 1; i >= 0; i--)
			{
				Room inProgressRoom = inProgressRooms[i];
				if (inProgressRoom.RightColumn < column)
				{
					foundRooms.Add(inProgressRoom);
					inProgressRooms.RemoveAt(i);
				}
			}
		}


		static List<RoomSegment> FindRoomSegmentsInColumn(Space[,] mapArray, int column)
		{
			var roomSegments = new List<RoomSegment>();
			var numRows = mapArray.GetLength(1);
			bool inFloorColumn = false;
			int lastSegmentStart = -1;
			for (int row = 0; row < numRows; row++)
			{
				if (mapArray[column, row] != null)
				{
					if (!inFloorColumn)
					{
						inFloorColumn = true;
						lastSegmentStart = row;
					}
				}
				else
				{
					if (inFloorColumn)
					{
						inFloorColumn = false;
						if (row > lastSegmentStart + 1)  // Only collect segments two squares tall or greater
							roomSegments.Add(new RoomSegment(lastSegmentStart, row - 1, column));
					}
				}
			}
			if (inFloorColumn)
				roomSegments.Add(new RoomSegment(lastSegmentStart, numRows - 1, column));
			return roomSegments;
		}

		static Room GetRoom(List<RoomSegment> lastLineSegments, RoomSegment compareRoomSegment)
		{
			foreach (var roomSegment in lastLineSegments)
			{
				if (roomSegment.IsAdjacent(compareRoomSegment))
				{
					var room = new Room(roomSegment.Column);
					room.AddSegment(roomSegment);
					room.AddSegment(compareRoomSegment);
					return room;
				}
			}
			return null;
		}

		static List<Room> FindRooms(List<List<RoomSegment>> allRoomSegments)
		{
			var lastLineSegments = new List<RoomSegment>();
			var currentLineSegments = new List<RoomSegment>();
			var inProgressRooms = new List<Room>();
			var foundRooms = new List<Room>();
			for (int column = 0; column < allRoomSegments.Count; column++)
			{
				List<RoomSegment> columnSegments = allRoomSegments[column];
				foreach (RoomSegment roomSegment in columnSegments)
				{
					currentLineSegments.Add(roomSegment);
					if (AddSegmentToExistingRooms(inProgressRooms, roomSegment))
						continue;
					Room room = GetRoom(lastLineSegments, roomSegment);
					if (room != null)
						inProgressRooms.Add(room);
				}

				FinishRooms(inProgressRooms, column, foundRooms);
				lastLineSegments = currentLineSegments;
				currentLineSegments = new List<RoomSegment>();
			}
			FinishRooms(inProgressRooms, allRoomSegments.Count + 1, foundRooms);
			return foundRooms;
		}

		//static List<Room> FindRooms(List<List<RoomSegment>> allRoomSegments)
		//{
		//	var lastLineSegments = new List<RoomSegment>();
		//	var currentLineSegments = new List<RoomSegment>();
		//	var inProgressRooms = new List<Room>();
		//	var foundRooms = new List<Room>();
		//	for (int column = 0; column < allRoomSegments.Count; column++)
		//	{
		//		List<RoomSegment> columnSegments = allRoomSegments[column];
		//		foreach (RoomSegment roomSegment in columnSegments)
		//		{
		//			if (AddSegmentToExistingRooms(inProgressRooms, roomSegment))
		//				continue;
		//			Room room = GetRoom(lastLineSegments, roomSegment);
		//			if (room != null)
		//				inProgressRooms.Add(room);
		//			else
		//				currentLineSegments.Add(roomSegment);
		//		}

		//		FinishRooms(inProgressRooms, column, foundRooms);
		//		lastLineSegments = currentLineSegments;
		//		currentLineSegments = new List<RoomSegment>();
		//	}
		//	FinishRooms(inProgressRooms, allRoomSegments.Count + 1, foundRooms);
		//	return foundRooms;
		//}

		public static Space[,] BuildMapArray(List<Space> spaces, int column, int row)
		{
			Space[,] spaceArray;

			spaceArray = new Space[column + 1, row + 1];
			foreach (Space space in spaces)
			{
				spaceArray[space.Column, space.Row] = space;
			}
			return spaceArray;
		}

		public static List<Room> GetAllRooms(Space[,] mapArray)
		{
			var allRoomSegments = new List<List<RoomSegment>>();
			var numColumns = mapArray.GetLength(0);

			for (int column = 0; column < numColumns; column++)
			{
				List<RoomSegment> roomSegmentsInColumn = FindRoomSegmentsInColumn(mapArray, column);
				allRoomSegments.Add(roomSegmentsInColumn);
			}

			//ShowRoomSegments(allRoomSegments);
			return FindRooms(allRoomSegments);
		}
		static MapEngine()
		{

		}
	}
}
