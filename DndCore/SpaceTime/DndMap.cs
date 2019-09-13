using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class DndMap
	{
		public const int SquareSide = 5; // feet

		DndRoom activeRoom;

		List<DndRoom> rooms = new List<DndRoom>();

		public DndMap()
		{
		}

		public DndMap(string name)
		{
			Name = name;
		}

		public DndRoom ActiveRoom
		{
			get => activeRoom;
			set
			{
				ActivateRoom(value);
			}
		}
		public DndGame Game { get; set; }

		public string Name { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public DndRoom ActivateRoom(DndRoom dndRoom)
		{
			activeRoom = rooms.FirstOrDefault(x => x == dndRoom);
			return activeRoom;
		}

		public DndRoom AddRoom(DndRoom dndRoom)
		{
			dndRoom.Map = this;
			rooms.Add(dndRoom);
			return dndRoom;
		}

		public Vector GetWorldPosition(int x, int y)
		{
			return new Vector(X + x, Y + y);
		}
	}
}
