using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace MapCore
{
	public class Tile : IFlyweight
	{
		public string TypeName { get; set; } = typeof(Tile).FullName;
		[JsonIgnore]
		public IMapInterface Map { get; set; }

		// TODO: Hmm.. 120 is a UI=specific implementation. Maybe change to a static property? 
		public const int Width = 120;
		public const int Height = 120;

		[JsonIgnore]
		public object UIElementFloor { get; set; }
		[JsonIgnore]
		public object UIElementOverlay { get; set; }
		[JsonIgnore]
		public object SelectorPanel { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }
		public SpaceType SpaceType { get; set; }
		public bool Selected { get; set; }
		public int PixelX
		{
			get
			{
				return Column * Width;
			}
		}
		public int PixelY
		{
			get
			{
				return Row * Height;
			}
		}

		public void GetPixelCoordinates(out int left, out int top, out int right, out int bottom)
		{
			left = PixelX;
			top = PixelY;
			right = left + Width - 1;
			bottom = top + Height - 1;
		}

		public void CopyFrom(Tile tile)
		{
			Column = tile.Column;
			Map = tile.Map;
			Row = tile.Row;
			Selected = tile.Selected;
			SelectorPanel = tile.SelectorPanel;
			SpaceType = tile.SpaceType;
			UIElementFloor = tile.UIElementFloor;
			UIElementOverlay = tile.UIElementOverlay;
		}

		List<Door> doors = new List<Door>();
		public string Code { get; set; }

		MapRegion parent;
		[JsonIgnore]
		public MapRegion Parent
		{
			get
			{
				if (parent == null && parentGuid != Guid.Empty && Map != null)
					parent = Map.GetFlyweight<MapRegion>(parentGuid);
				return parent;
			}
			set
			{
				parent = value;
			}
		}

		Guid parentGuid = Guid.Empty;
		public Guid ParentGuid
		{
			get
			{
				if (parent != null)
					return parent.Guid;

				return parentGuid;
			}
			set
			{
				parentGuid = value;
			}
		}

		public void Reconstitute()
		{
			Parent = Map.GetFlyweight<MapRegion>(ParentGuid);
		}

		public void PrepareForSerialization()
		{
			if (Parent != null)
				ParentGuid = Parent.Guid;
		}

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

		bool isFloor;
		public bool IsFloor
		{
			get
			{
				return isFloor;
			}
			set
			{
				if (isFloor == value)
				{
					return;
				}

				isFloor = value;
				if (Map != null)
					Map.FloorTypeChanged(this);
			}
		}

		Guid guid = Guid.Empty;
		public Guid Guid
		{
			get
			{
				return guid;
			}
			set
			{
				guid = value;
			}
		}

		public bool NeedsGuid()
		{
			return Guid == Guid.Empty;
		}

		public string ImageFileName { get; set; }
		public string BaseTextureName { get; set; }

		public Tile(int column, int row, IMapInterface iMap)
		{
			Map = iMap;
			Row = row;
			Column = column;
		}

		public Tile()  // Called by deserialization
		{
		}

		public Tile(int column, int row, string code, IMapInterface iMap, bool isFloor) : this(column, row, iMap)
		{
			IsFloor = isFloor;
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
		public bool HasThreeAdjacentSpaces()
		{
			//`![](8F1819625E627683BB9C334475F3CEAB.png)
			if (HasFloorUpperLeft() && HasFloorLeft() && HasFloorAbove())
				return true;

			//`![](EA83D3AD6427364DB9E7C9A58DF3AB67.png)
			if (HasFloorAbove() && HasFloorUpperRight() && HasFloorRight())
				return true;

			//`![](2BFF98EE7BEAF2BFB29187399968E481.png)
			if (HasFloorRight() && HasFloorLowerRight() && HasFloorBelow())
				return true;

			//`![](EC9B46DCDCC425B9664795DEB9997596.png)
			if (HasFloorLowerLeft() && HasFloorBelow() && HasFloorLeft())
				return true;

			return false;
		}

		private bool HasFloorLowerLeft()
		{
			return Map.FloorSpaceExists(Column - 1, Row + 1);
		}

		private bool HasFloorLowerRight()
		{
			return Map.FloorSpaceExists(Column + 1, Row + 1);
		}

		private bool HasFloorUpperRight()
		{
			return Map.FloorSpaceExists(Column + 1, Row - 1);
		}

		private bool HasFloorUpperLeft()
		{
			return Map.FloorSpaceExists(Column - 1, Row - 1);
		}

		bool HasWallOrDoor(int deltaColumn, int deltaRow)
		{

			foreach (Door door in doors)
			{
				if (deltaColumn == -1 && door.Position == DoorPosition.Left)
					return true;
				if (deltaColumn == 1 && door.Position == DoorPosition.Right)
					return true;
				if (deltaRow == -1 && door.Position == DoorPosition.Top)
					return true;
				if (deltaRow == 1 && door.Position == DoorPosition.Bottom)
					return true;
			}

			if (deltaColumn == -1 && Map.HasWallLeft(Column, Row))
				return true;
			if (deltaColumn == 1 && Map.HasWallRight(Column, Row))
				return true;
			if (deltaRow == -1 && Map.HasWallTop(Column, Row))
				return true;
			if (deltaRow == 1 && Map.HasWallBottom(Column, Row))
				return true;

			return false;
		}

		bool HasAdjacentFloor(int deltaColumn, int deltaRow)
		{
			int otherColumn = Column + deltaColumn;
			int otherRow = Row + deltaRow;
			if (!Map.FloorSpaceExists(otherColumn, otherRow))
				return false;
			Tile otherTile = Map.GetTile(otherColumn, otherRow);
			return !HasWallOrDoor(deltaColumn, deltaRow) && !otherTile.HasWallOrDoor(-deltaColumn, -deltaRow);
		}

		private bool HasFloorLeft()
		{
			//return Map.FloorSpaceExists(Column - 1, Row);
			return HasAdjacentFloor(-1, 0);
		}

		private bool HasFloorAbove()
		{
			//return Map.FloorSpaceExists(Column, Row - 1);
			return HasAdjacentFloor(0, -1);
		}

		private bool HasFloorBelow()
		{
			//return Map.FloorSpaceExists(Column, Row + 1);
			return HasAdjacentFloor(0, 1);
		}

		private bool HasFloorRight()
		{
			//return Map.FloorSpaceExists(Column + 1, Row);
			return HasAdjacentFloor(1, 0);
		}
	}
}
