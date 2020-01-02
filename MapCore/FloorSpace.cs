using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace MapCore
{
	public class FloorSpace : Tile
	{
		List<Door> doors = new List<Door>();
		public string Code { get; set; }
		public MapRegion Parent { get; set; }
		public List<Door> Doors
		{
			get
			{
				if (doors == null)
					doors = new List<Door>();
				return doors;
			}
			private set => doors = value;
		}

		public bool HasDoors
		{
			get
			{
				return doors != null && doors.Count > 0; ;
			}
		}

		public FloorSpace(int column, int row, string code, IMapInterface iMap) : base(column, row, iMap)
		{
			AnalyzeCode(code);
			// TODO: Consider removing Code property once we've got analysis complete.
			Code = code;
		}
		void AddDoorFromCode(string doorCode)
		{
			if (string.IsNullOrWhiteSpace(doorCode))
				return;
			string location = doorCode.Substring(0, 1);
			switch (location)
			{
				case "L":
					Doors.Add(new Door(DoorPosition.Left));
					break;
				case "B":
					Doors.Add(new Door(DoorPosition.Bottom));
					break;
				case "T":
					Doors.Add(new Door(DoorPosition.Top));
					break;
				case "R":
					Doors.Add(new Door(DoorPosition.Right));
					break;
			}
		}
		void AnalyzeCode(string code)
		{
			if (code.StartsWith("D"))
			{
				AddDoorFromCode(code.Substring(1));
			}
		}

		void RemoveExistingDoor(DoorPosition doorPosition)
		{
			Door foundDoor = Doors.FirstOrDefault(x => x.Position == doorPosition);
			if (foundDoor == null)
				return;
			Doors.Remove(foundDoor);
		}

		public void SetDoor(DoorPosition doorPosition, bool adding)
		{
			RemoveExistingDoor(doorPosition);
			if (adding)
				Doors.Add(new Door(doorPosition));
		}
	}
}
