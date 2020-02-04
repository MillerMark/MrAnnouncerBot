using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class Map : IMapInterface, IStampsManager
	{
		List<IStampProperties> stamps = new List<IStampProperties>();
		private const string MapFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Maps";

		int lastColumnIndex;
		public int lastRowIndex;
		bool needToRecalculateRoomsAndCorridors;
		public int rightmostColumnIndex;
		int wallChangeCount;
		bool wallsChanged;

		public Map()
		{
		}

		/* 
		 * Store:
		 

		 * Reconstitute:
						
		 
			 */

		public bool[,] _allHorizontalWalls { get; set; }

		public bool[,] _allVerticalWalls { get; set; }

		[JsonIgnore]
		public Tile[,] TileMap { get; private set; }

		[JsonIgnore]
		public List<MapRegion> Corridors { get; private set; } = new List<MapRegion>();

		public int HeightPx { get { return (lastRowIndex + 1) * Tile.Height; } }

		public int NumColumns { get; set; }

		public int NumRows { get; set; }

		[JsonIgnore]
		public List<MapRegion> Rooms { get; private set; } = new List<MapRegion>();

		[JsonIgnore]
		public List<Tile> Tiles { get; private set; } = new List<Tile>();

		public int WidthPx { get { return (rightmostColumnIndex + 1) * Tile.Width; } }

		[JsonIgnore]
		public string FileName { get; set; }

		public List<IStampProperties> Stamps { get => stamps; set => stamps = value; }

		static void ClearAllSpaces(Tile[,] mapArray, int numColumns, int numRows)
		{
			for (int column = 0; column < numColumns; column++)
				for (int row = 0; row < numRows; row++)
				{
					Tile activeSpace = mapArray[column, row];
					if (activeSpace != null)
					{
						activeSpace.Parent = null;
						activeSpace.SpaceType = SpaceType.None;
					}
				}
		}

		private static string GetDataFromFile(string fileName)
		{
			return File.ReadAllText(Path.Combine(MapFolder, fileName));
		}

		private static string[] SplitLines(string text)
		{
			return TrimEmptyLines(text.Split('\n'));
		}

		static string[] TrimEmptyLines(string[] lines)
		{
			int endLinesToSkip = 0;
			for (int i = lines.Length - 1; i >= 0; i--)
			{
				if (string.IsNullOrWhiteSpace(lines[i]))
					endLinesToSkip++;
				else
					break;
			}
			int startLinesToSkip = lines.TakeWhile(x => string.IsNullOrWhiteSpace(x)).ToList().Count;

			int endIndex = lines.Length - endLinesToSkip - startLinesToSkip - 1;
			string[] result = lines.SkipWhile(x => string.IsNullOrWhiteSpace(x))
				.TakeWhile((line, index) => index <= endIndex)
				.ToArray();
			return result;
		}

		void AddAdjacentSpaces(MapRegion region, Tile activeSpace)
		{
			region.AddTile(activeSpace);

			Tile left = GetFloorSpace(activeSpace.Column - 1, activeSpace.Row);
			AddIfAvailable(left, region, activeSpace);

			Tile top = GetFloorSpace(activeSpace.Column, activeSpace.Row - 1);
			AddIfAvailable(top, region, activeSpace);

			Tile right = GetFloorSpace(activeSpace.Column + 1, activeSpace.Row);
			AddIfAvailable(right, region, activeSpace);

			Tile bottom = GetFloorSpace(activeSpace.Column, activeSpace.Row + 1);
			AddIfAvailable(bottom, region, activeSpace);
		}

		void AddIfAvailable(Tile floorSpace, MapRegion region, Tile activeSpace)
		{
			if (floorSpace == null)
				return;
			if (floorSpace.SpaceType == activeSpace.SpaceType && floorSpace.Parent == null)
			{
				floorSpace.Parent = region;
				AddAdjacentSpaces(region, floorSpace);
			}
		}

		void AddSpaceToRegion(MapRegion region, Tile activeSpace)
		{
			activeSpace.Parent = region;
			AddAdjacentSpaces(region, activeSpace);
		}

		MapRegion CreateCorridorFromSpace(Tile activeSpace)
		{
			MapRegion corridor = new MapRegion(this);
			corridor.RegionType = RegionType.Corridor;
			AddSpaceToRegion(corridor, activeSpace);
			return corridor;
		}

		MapRegion CreateRoomFromSpace(Tile activeSpace)
		{
			MapRegion room = new MapRegion(this);
			room.RegionType = RegionType.Room;
			AddSpaceToRegion(room, activeSpace);
			return room;
		}

		private void DetermineSpaceTypes()
		{
			for (int column = 0; column < NumColumns; column++)
				for (int row = 0; row < NumRows; row++)
				{
					Tile tile = GetFloorSpace(column, row);
					if (tile == null || tile.SpaceType != SpaceType.None)
						continue;

					if (tile.HasThreeAdjacentSpaces())
						tile.SpaceType = SpaceType.Room;
					else
						tile.SpaceType = SpaceType.Corridor;
				}
		}

		bool HasHorizontalTouchingWall(int column, int row)
		{
			return HasHorizontalTouchingWallRight(column, row) || HasHorizontalTouchingWallLeft(column, row);
		}

		private bool HasHorizontalTouchingWallLeft(int column, int row)
		{
			return HasHorizontalWall(column, row);
		}

		private bool HasHorizontalTouchingWallRight(int column, int row)
		{
			return HasHorizontalWall(column + 1, row);
		}

		bool HasHorizontalWall(int column, int row)
		{
			column++;
			row++;
			if (column < 0 || row < 0 || column > NumColumns || row > NumRows)
				return false;
			return _allHorizontalWalls[column, row];
		}

		public bool HasWallLeft(int column, int row)
		{
			return HasVerticalWall(column - 1, row);
		}

		public bool HasWallRight(int column, int row)
		{
			return HasVerticalWall(column, row);
		}

		public bool HasWallTop(int column, int row)
		{
			return HasHorizontalWall(column, row - 1);
		}

		public bool HasWallBottom(int column, int row)
		{
			return HasHorizontalWall(column, row);
		}

		bool HasVerticalTouchingWall(int column, int row)
		{
			return HasVerticalTouchingWallBottom(column, row) || HasVerticalTouchingWallTop(column, row);
		}

		private bool HasVerticalTouchingWallBottom(int column, int row)
		{
			return HasVerticalWall(column, row + 1);
		}

		private bool HasVerticalTouchingWallTop(int column, int row)
		{
			return HasVerticalWall(column, row);
		}

		bool HasVerticalWall(int column, int row)
		{
			column++;
			row++;
			if (column < 0 || row < 0 || column > NumColumns || row > NumRows)
				return false;
			return _allVerticalWalls[column, row];
		}

		bool IsFloor(string space)
		{
			// TODO: Check for code for Portcullis.
			return space == "F" || space.StartsWith("D") || space.StartsWith("S");
		}

		void LoadNewLine(string line)
		{
			line = line.Trim('\r');
			OnNewLine();
			var spaces5x5 = line.Split('\t');
			foreach (var space in spaces5x5)
				LoadNewSpace(space);
		}

		void LoadNewSpace(string space)
		{
			OnNewSpace();
			Tiles.Add(new Tile(lastColumnIndex, lastRowIndex, space, this, IsFloor(space)));
		}

		void OnNewLine()
		{
			lastRowIndex++;
			lastColumnIndex = -1;
		}

		void OnNewSpace()
		{
			lastColumnIndex++;
			if (lastColumnIndex > rightmostColumnIndex)
				rightmostColumnIndex = lastColumnIndex;
		}

		private void ProcessLines(string[] lines)
		{
			Reset();
			foreach (var line in lines)
				LoadNewLine(line);

			SetMapSize();
			AllocateWallsArrays();

			BuildMapArrays();
			GetAllRoomsAndCorridors();
		}

		private void AllocateWallsArrays()
		{
			_allVerticalWalls = new bool[NumColumns + 1, NumRows + 1];
			_allHorizontalWalls = new bool[NumColumns + 1, NumRows + 1];
		}

		void Reset()
		{
			Tiles = new List<Tile>();
			lastRowIndex = -1;
			lastColumnIndex = -1;
			rightmostColumnIndex = -1;
		}

		private void SetMapSize()
		{
			NumColumns = rightmostColumnIndex + 1;
			NumRows = lastRowIndex + 1;
		}

		protected virtual void OnWallsChanged()
		{
			WallsChanged?.Invoke(this, EventArgs.Empty);
		}

		public void BeginWallUpdate()
		{
			if (wallChangeCount == 0)
				wallsChanged = false;
			wallChangeCount++;
			needToRecalculateRoomsAndCorridors = true;
		}

		void GetTilesAndRoomsFromFlyweights()
		{
			foreach (Guid guid in flyweights.Keys)
			{
				JObject jObject = flyweights[guid] as JObject;
				JToken typeNameToken = jObject.GetValue("TypeName");
				string typeName = typeNameToken.ToString();
				switch (typeName)
				{
					case "MapCore.Tile":
						Tile tile = jObject.ToObject<Tile>();
						Tiles.Add(tile);
						reconstitutedFlyweights[guid] = tile;
						break;
					case "MapCore.MapRegion":
						MapRegion mapRegion = jObject.ToObject<MapRegion>();
						reconstitutedFlyweights[guid] = mapRegion;
						if (mapRegion.RegionType == RegionType.Room)
							Rooms.Add(mapRegion);
						else if (mapRegion.RegionType == RegionType.Corridor)
							Corridors.Add(mapRegion);
						break;
				}
			}
		}

		public void BuildMapArrays()
		{
			TileMap = new Tile[NumColumns, NumRows];
			// TODO: Find walls for rooms that are loaded.

			GetTilesAndRoomsFromFlyweights();

			foreach (Tile tile in Tiles)
			{
				TileMap[tile.Column, tile.Row] = tile;
				tile.Map = this;
			}
		}

		public void ClearSelection()
		{
			foreach (Tile tile in Tiles)
				tile.Selected = false;
		}

		public WallData CollectHorizontalWall(int column, int row)
		{
			WallData result = new WallData(WallOrientation.Horizontal);
			result.StartColumn = column - 1;
			result.StartRow = row;
			int startX = column * Tile.Width;
			int startY = row * Tile.Height;

			while (column < NumColumns && HasHorizontalWall(column + 1, row) && !HasVerticalTouchingWall(column, row))
			{
				column++;
			}
			result.EndColumn = column;
			result.EndRow = row;
			int endX = column * Tile.Width;
			result.X = startX;
			result.Y = startY;
			result.WallLength = endX + Tile.Width - startX;
			return result;
		}

		public WallData CollectVerticalWall(int column, int row)
		{
			WallData result = new WallData(WallOrientation.Vertical);
			result.StartColumn = column;
			result.StartRow = row - 1;
			int startX = column * Tile.Width;
			int startY = row * Tile.Height;

			while (row < NumRows && HasVerticalWall(column, row + 1) && !HasHorizontalTouchingWall(column, row))
			{
				row++;
			}
			result.EndColumn = column;
			result.EndRow = row;
			int endY = row * Tile.Height;
			result.X = startX;
			result.Y = startY;
			result.WallLength = endY + Tile.Height - startY;
			return result;
		}

		public void EndWallUpdate()
		{
			wallChangeCount--;
			if (wallChangeCount == 0 && wallsChanged)
			{
				wallsChanged = false;
				UpdateIfNeeded();
				OnWallsChanged();
			}
		}

		public bool FloorSpaceExists(int column, int row)
		{
			return GetFloorSpace(column, row) != null;
		}

		public void FloorTypeChanged(Tile tile)
		{
			needToRecalculateRoomsAndCorridors = true;
		}

		public List<Tile> GetAllMatchingTiles(RegionType regionType)
		{
			List<Tile> results = new List<Tile>();
			foreach (Tile tile in Tiles)
				if (tile is Tile floorSpace && floorSpace.Parent != null && floorSpace.Parent.RegionType == regionType)
					results.Add(tile);
			return results;
		}

		public List<Tile> GetAllOtherSpaces(List<Tile> compareSpaces)
		{
			List<Tile> results = new List<Tile>();
			foreach (Tile tile in Tiles)
				if (compareSpaces.IndexOf(tile) < 0)
					results.Add(tile);
			return results;
		}

		public void GetAllRoomsAndCorridors()
		{
			Rooms.Clear();
			Corridors.Clear();
			ClearAllSpaces(TileMap, NumColumns, NumRows);
			DetermineSpaceTypes();


			for (int column = 0; column < NumColumns; column++)
				for (int row = 0; row < NumRows; row++)
				{
					Tile activeSpace = GetFloorSpace(column, row);
					if (activeSpace == null)
						continue;

					if (activeSpace.SpaceType == SpaceType.Room)
					{
						if (activeSpace.Parent == null)
							Rooms.Add(CreateRoomFromSpace(activeSpace));
					} else if (activeSpace.SpaceType == SpaceType.Corridor)
					{
						if (activeSpace.Parent == null)
							Corridors.Add(CreateCorridorFromSpace(activeSpace));
					}
				}
		}

		public EndCapKind GetEndCapKind(int column, int row)
		{
			bool wallLeft = HasHorizontalTouchingWallLeft(column, row);
			bool wallRight = HasHorizontalTouchingWallRight(column, row);
			bool wallTop = HasVerticalTouchingWallTop(column, row);
			bool wallBottom = HasVerticalTouchingWallBottom(column, row);
			int count = 0;
			if (wallLeft)
				count++;
			if (wallTop)
				count++;
			if (wallRight)
				count++;
			if (wallBottom)
				count++;

			if (count == 4)
				return EndCapKind.FourWayIntersection;

			if (count == 0)
				return EndCapKind.None;

			if (count == 1)
			{
				if (wallLeft)
					return EndCapKind.Right;
				if (wallTop)
					return EndCapKind.Bottom;
				if (wallRight)
					return EndCapKind.Left;
				if (wallBottom)
					return EndCapKind.Top;

				return EndCapKind.None;
			}

			if (count == 2) // Corners
			{
				if (wallLeft && wallTop)
					return EndCapKind.BottomRightCorner;
				if (wallTop && wallRight)
					return EndCapKind.BottomLeftCorner;
				if (wallRight && wallBottom)
					return EndCapKind.TopLeftCorner;
				if (wallBottom && wallLeft)
					return EndCapKind.TopRightCorner;

				return EndCapKind.None;
			}

			// Tees...
			if (!wallLeft)
				return EndCapKind.LeftTee;
			if (!wallTop)
				return EndCapKind.TopTee;
			if (!wallRight)
				return EndCapKind.RightTee;
			if (!wallBottom)
				return EndCapKind.BottomTee;

			return EndCapKind.None;
		}

		public Tile GetFloorSpace(int column, int row)
		{
			if (column < 0 || row < 0 || column >= NumColumns || row >= NumRows)
				return null;
			Tile tile = TileMap[column, row];
			if (tile == null)
				return null;
			if (!tile.IsFloor)
				return null;
			return TileMap[column, row];
		}

		public List<Tile> GetSelection()
		{
			return Tiles.Where(x => x.Selected).ToList();
		}

		public void GetSelectionBoundaries(out int left, out int top, out int right, out int bottom)
		{
			left = int.MaxValue;
			top = int.MaxValue;
			right = 0;
			bottom = 0;
			List<Tile> selection = GetSelection();
			foreach (Tile tile in selection)
			{
				int tileLeft = tile.PixelX;
				int tileTop = tile.PixelY;
				int tileRight = tileLeft + Tile.Width;
				int tileBottom = tileTop + Tile.Height;
				left = Math.Min(tileLeft, left);
				top = Math.Min(tileTop, top);
				right = Math.Max(tileRight, right);
				bottom = Math.Max(tileBottom, bottom);
			}
		}

		public Tile GetTile(int column, int row)
		{
			if (column < 0 || row < 0 || column >= NumColumns || row >= NumRows)
				return null;
			return TileMap[column, row];
		}

		public List<Tile> GetTilesInPixelRect(int left, int top, int width, int height)
		{
			PixelsToColumnRow(left, top, out int leftColumn, out int topRow);
			PixelsToColumnRow(left + width - 1, top + height - 1, out int rightColumn, out int bottomRow);
			List<Tile> results = new List<Tile>();
			foreach (Tile baseSpace in Tiles)
			{
				if (baseSpace.Column >= leftColumn &&
					baseSpace.Column <= rightColumn &&
					baseSpace.Row >= topRow &&
					baseSpace.Row <= bottomRow)
					results.Add(baseSpace);
			}
			return results;
		}

		public List<Tile> GetTilesOutsidePixelRect(int left, int top, int width, int height)
		{
			PixelsToColumnRow(left, top, out int leftColumn, out int topRow);
			PixelsToColumnRow(left + width - 1, top + height - 1, out int rightColumn, out int bottomRow);
			List<Tile> results = new List<Tile>();
			foreach (Tile baseSpace in Tiles)
			{
				if (baseSpace.Column < leftColumn ||
					baseSpace.Column > rightColumn ||
					baseSpace.Row < topRow ||
					baseSpace.Row > bottomRow)
					results.Add(baseSpace);
			}
			return results;
		}

		public VoidCornerKind GetVoidKind(int column, int row)
		{
			bool hasUpperLeftTile = FloorSpaceExists(column, row);
			bool hasUpperRightTile = FloorSpaceExists(column + 1, row);
			bool hasLowerLeftTile = FloorSpaceExists(column, row + 1);
			bool hasLowerRightTile = FloorSpaceExists(column + 1, row + 1);
			int tileCount = 0;
			if (hasUpperLeftTile)
				tileCount++;
			if (hasUpperRightTile)
				tileCount++;
			if (hasLowerLeftTile)
				tileCount++;
			if (hasLowerRightTile)
				tileCount++;

			if (tileCount == 3)
			{
				if (!hasLowerRightTile)
					return VoidCornerKind.InsideTopLeft;
				if (!hasUpperRightTile)
					return VoidCornerKind.InsideBottomLeft;
				if (!hasLowerLeftTile)
					return VoidCornerKind.InsideTopRight;
				if (!hasUpperLeftTile)
					return VoidCornerKind.InsideBottomRight;
			}

			if (tileCount == 1)
			{
				if (hasLowerRightTile)
					return VoidCornerKind.OutsideTopLeft;
				if (hasUpperRightTile)
					return VoidCornerKind.OutsideBottomLeft;
				if (hasLowerLeftTile)
					return VoidCornerKind.OutsideTopRight;
				if (hasUpperLeftTile)
					return VoidCornerKind.OutsideBottomRight;
			}

			if (tileCount == 2)
			{
				if (hasLowerRightTile && hasUpperRightTile)
					return VoidCornerKind.TLeft;
				if (hasUpperLeftTile && hasUpperRightTile)
					return VoidCornerKind.TBottom;
				if (hasUpperLeftTile && hasLowerLeftTile)
					return VoidCornerKind.TRight;
				if (hasLowerRightTile && hasLowerLeftTile)
					return VoidCornerKind.TTop;
			}

			return VoidCornerKind.None;
		}

		public bool HasHorizontalWallStart(int column, int row)
		{
			if (!HasHorizontalWall(column, row))
				return false;
			if (HasHorizontalWall(column - 1, row))
			{
				return HasVerticalTouchingWall(column - 1, row);
			}
			return true;
		}

		public bool HasVerticalWallStart(int column, int row)
		{
			if (!HasVerticalWall(column, row))
				return false;
			if (HasVerticalWall(column, row - 1))
			{
				return HasHorizontalTouchingWall(column, row - 1);
			}
			return true;
		}

		public void Load(string fileName)
		{
			LoadData(GetDataFromFile(fileName));
		}

		public void LoadData(string mapData)
		{
			ProcessLines(SplitLines(mapData));
		}

		public void PixelsToColumnRow(double x, double y, out int column, out int row)
		{
			column = (int)(x / Tile.Width);
			row = (int)(y / Tile.Height);
		}

		public bool SelectionExists()
		{
			foreach (Tile tile in Tiles)
				if (tile.Selected)
					return true;
			return false;
		}

		public void SetWall(int column, int row, WallOrientation wallOrientation, bool isThere)
		{
			column++;  // To allow for -1, -1 coordinates
			row++;
			switch (wallOrientation)
			{
				case WallOrientation.Horizontal:
					if (_allHorizontalWalls[column, row] != isThere)
					{
						_allHorizontalWalls[column, row] = isThere;
						wallsChanged = true;
					}
					break;
				case WallOrientation.Vertical:
					if (_allVerticalWalls[column, row] != isThere)
					{
						_allVerticalWalls[column, row] = isThere;
						wallsChanged = true;
					}
					break;
			}
		}

		public bool TileExists(int column, int row)
		{
			return column >= 0 && row >= 0 && column < NumColumns && row < NumRows;
		}

		public void UpdateIfNeeded()
		{
			if (!needToRecalculateRoomsAndCorridors)
				return;
			GetAllRoomsAndCorridors();
		}

		public bool VoidIsAbove(int column, int row)
		{
			return !FloorSpaceExists(column + 1, row);
		}

		public bool VoidIsBelow(int column, int row)
		{
			return !FloorSpaceExists(column + 1, row + 1);
		}

		public bool VoidIsLeft(int column, int row)
		{
			return !FloorSpaceExists(column, row + 1);
		}

		public bool VoidIsRight(int column, int row)
		{
			return !FloorSpaceExists(column + 1, row + 1);
		}

		public void Reconstitute()
		{
			BuildMapArrays();

			foreach (MapRegion room in Rooms)
			{
				room.ParentMap = this;
				room.Reconstitute();
			}

			foreach (MapRegion corridor in Corridors)
			{
				corridor.ParentMap = this;
				corridor.Reconstitute();
			}
		}

		public void Save()
		{
			if (FileName == null)
				return;
			PrepareForSerialization();
			string output = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(FileName, output);
		}

		public Dictionary<Guid, object> flyweights = new Dictionary<Guid, object>();

		[JsonIgnore]
		public Dictionary<Guid, object> reconstitutedFlyweights = new Dictionary<Guid, object>();

		public T GetFlyweight<T>(Guid guid) where T : class
		{
			if (guid == Guid.Empty)
				return null;

			if (reconstitutedFlyweights.ContainsKey(guid))
			{
				if (reconstitutedFlyweights[guid] as T == null)
				{

				}
				return reconstitutedFlyweights[guid] as T;
			}

			if (flyweights.ContainsKey(guid))
			{
				JObject jObject = flyweights[guid] as JObject;  // Thanks Rory (and S.O.)!!! 
				T instance = jObject.ToObject<T>() as T;
				reconstitutedFlyweights[guid] = instance;
				return instance;
			}
			return null;
		}

		public void AddFlyweight(Guid key, IFlyweight instance)
		{
			if (key == Guid.Empty)
				throw new Exception($"What? Cannot add flyweight {instance} with an empty key!!!");

			instance.Guid = key;
			if (!flyweights.ContainsKey(key))
				flyweights.Add(key, instance);
			else  // TODO: Should we throw an exception here?
				flyweights[key] = instance;
		}

		public void PrepareForSerialization()
		{
			flyweights.Clear();
			foreach (Tile tile in Tiles)
			{
				//if (tile.NeedsGuid())
				AddFlyweight(Guid.NewGuid(), tile);
				tile.PrepareForSerialization();
			}

			foreach (MapRegion corridor in Corridors)
			{
				//if (corridor.NeedsGuid())
				AddFlyweight(Guid.NewGuid(), corridor);
				corridor.PrepareForSerialization();
			}

			foreach (MapRegion room in Rooms)
			{
				//if (room.NeedsGuid())
				AddFlyweight(Guid.NewGuid(), room);
				room.PrepareForSerialization();
			}
		}

		public void AddStamp(IStampProperties stamp)
		{
			if (stamp.HasNoZOrder())
				stamp.ZOrder = stamps.Count;
			stamps.Add(stamp);
		}

		public void AddStamps(List<IStampProperties> stamps)
		{
			foreach (IStampProperties stamp in stamps)
			{
				stamp.ResetZOrder();
				AddStamp(stamp);
			}
		}

		public IStampProperties GetStampAt(double x, double y)
		{
			for (int i = stamps.Count - 1; i >= 0; i--)
			{
				IStampProperties stamp = stamps[i];
				if (stamp.ContainsPoint(x, y))
					return stamp;
			}
			return null;
		}

		public void InsertStamp(int startIndex, IStampProperties stamp)
		{
			if (stamp.HasNoZOrder())
				stamp.ZOrder = startIndex;
			stamps.Insert(startIndex, stamp);
		}

		public void InsertStamps(int startIndex, List<IStampProperties> stamps)
		{
			for (int i = 0; i < stamps.Count; i++)
			{
				IStampProperties stamp = stamps[i];
				stamp.ResetZOrder();
				InsertStamp(startIndex + i, stamp);
			}
		}

		public void RemoveAllStamps(List<IStampProperties> stamps)
		{
			foreach (IStampProperties stamp in stamps)
				RemoveStamp(stamp);
		}

		public void RemoveStamp(IStampProperties stamp)
		{
			stamps.Remove(stamp);
		}

		public void SortStampsByZOrder(int zOrderOffset = 0)
		{
			stamps = stamps.OrderBy(o => o.ZOrder).ToList();
			for (int i = 0; i < stamps.Count; i++)
				stamps[i].ZOrder = i + zOrderOffset;
		}

		public event EventHandler WallsChanged;
	}
}
