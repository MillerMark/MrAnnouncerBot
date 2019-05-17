using System;
using System.Collections.Generic;
using System.Linq;

namespace DndCore
{
	public class DndMap
	{
		public int X { get; set; }
		public int Y { get; set; }

		List<DndRoom> rooms = new List<DndRoom>();
		DndRoom activeRoom;

		public DndMap()
		{
		}

		public DndMap(string name)
		{
			Name = name;
		}

		public DndRoom AddRoom(DndRoom dndRoom)
		{
			dndRoom.Map = this;
			rooms.Add(dndRoom);
			return dndRoom;
		}
		public DndRoom ActivateRoom(DndRoom dndRoom)
		{
			activeRoom = rooms.FirstOrDefault(x => x == dndRoom);
			return activeRoom;
		}

		public Vector GetWorldPosition(int x, int y)
		{
			return new Vector(X + x, Y + y);
		}

		public string Name { get; set; }
		public DndRoom ActiveRoom { get => activeRoom;
			set
			{
				ActivateRoom(value);
			}
		}
		public DndGame Game { get; set; }
	}
}
