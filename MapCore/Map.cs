using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MapCore;

namespace MapCore
{
	public class Map
	{
		private const string MapFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Maps";
		


		// TODO: Should pixelsPerFiveFeet really be here?
		public static int TileSizePx { get; set; } = 60;

		//public double pixelsPerFoot { get; set; } = 4;
		//public int feetPerSquare { get; set; } = 5;




		int lastRowIndex;
		int lastColumnIndex;
		int rightmostColumnIndex;
		public Map()
		{

		}
		void Reset()
		{
			Spaces = new List<Tile>();
			lastRowIndex = -1;
			lastColumnIndex = -1;
			rightmostColumnIndex = -1;
		}
		void OnNewLine()
		{
			lastRowIndex++;
			lastColumnIndex = -1;
		}
		bool IsFloor(string space)
		{
			return space == "F" || space.StartsWith("D") || space.StartsWith("S");
		}
		void LoadNewSpace(string space)
		{
			OnNewSpace();
			if (IsFloor(space))
				Spaces.Add(new FloorSpace(lastColumnIndex, lastRowIndex, space));
			else
				Spaces.Add(new EmptySpace(lastColumnIndex, lastRowIndex));
		}
		void OnNewSpace()
		{
			lastColumnIndex++;
			if (lastColumnIndex > rightmostColumnIndex)
				rightmostColumnIndex = lastColumnIndex;
		}

		public void LoadData(string mapData)
		{
			ProcessLines(SplitLines(mapData));
		}

		public void Load(string fileName)
		{
			LoadData(GetDataFromFile(fileName));
		}

		private void ProcessLines(string[] lines)
		{
			Reset();
			foreach (var line in lines)
				LoadNewLine(line);

			SetMapSize();
			BuildMapArrays();
			GetAllRoomsAndCorridors();
		}

		public void BuildMapArrays()
		{
			AllTiles = new Tile[NumColumns, NumRows];
			foreach (Tile space in Spaces)
			{
				AllTiles[space.Column, space.Row] = space;
			}
		}

		private void SetMapSize()
		{
			NumColumns = rightmostColumnIndex + 1;
			NumRows = lastRowIndex + 1;
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

			int endIndex = lines.Length - endLinesToSkip - startLinesToSkip- 1;
			string[] result = lines.SkipWhile(x => string.IsNullOrWhiteSpace(x)).TakeWhile((line, index) => index <= endIndex).ToArray();
			return result;
		}


		void LoadNewLine(string line)
		{
			line = line.Trim('\r');
			OnNewLine();
			var spaces5x5 = line.Split('\t');
			foreach (var space in spaces5x5)
				LoadNewSpace(space);
		}

		static void ClearAllSpaces(Tile[,] mapArray, int numColumns, int numRows)
		{
			for (int column = 0; column < numColumns; column++)
				for (int row = 0; row < numRows; row++)
				{
					FloorSpace activeSpace = mapArray[column, row] as FloorSpace;
					if (activeSpace != null)
					{
						activeSpace.Parent = null;
						activeSpace.SpaceType = SpaceType.None;
					}
				}
		}

		public FloorSpace GetFloorSpace(int column, int row)
		{
			if (column < 0 || row < 0 || column >= NumColumns || row >= NumRows)
				return null;
			return AllTiles[column, row] as FloorSpace;
		}

		public Tile GetBaseSpace(int column, int row)
		{
			if (column < 0 || row < 0 || column >= NumColumns || row >= NumRows)
				return null;
			return AllTiles[column, row];
		}

		bool FloorSpaceExists(int column, int row)
		{
			return GetFloorSpace(column, row) != null;
		}

		bool HasThreeAdjacentSpaces(int column, int row)
		{
			FloorSpace space = GetFloorSpace(column, row);
			if (space == null)
				return false;

			//`![](8F1819625E627683BB9C334475F3CEAB.png)
			if (FloorSpaceExists(column - 1, row - 1) &&
					FloorSpaceExists(column, row - 1) &&
					FloorSpaceExists(column - 1, row))
				return true;

			//`![](EA83D3AD6427364DB9E7C9A58DF3AB67.png)
			if (FloorSpaceExists(column + 1, row - 1) &&
					FloorSpaceExists(column, row - 1) &&
					FloorSpaceExists(column + 1, row))
				return true;

			//`![](2BFF98EE7BEAF2BFB29187399968E481.png)
			if (FloorSpaceExists(column + 1, row + 1) &&
					FloorSpaceExists(column, row + 1) &&
					FloorSpaceExists(column + 1, row))
				return true;

			//`![](EC9B46DCDCC425B9664795DEB9997596.png)
			if (FloorSpaceExists(column - 1, row + 1) &&
					FloorSpaceExists(column, row + 1) &&
					FloorSpaceExists(column - 1, row))
				return true;

			return false;
		}

		void AddIfAvailable(FloorSpace floorSpace, MapRegion region, FloorSpace activeSpace)
		{
			if (floorSpace == null)
				return;
			if (floorSpace.SpaceType == activeSpace.SpaceType && floorSpace.Parent == null)
			{
				floorSpace.Parent = region;
				AddAdjacentSpaces(region, floorSpace);
			}
		}

		void AddAdjacentSpaces(MapRegion region, FloorSpace activeSpace)
		{
			region.Spaces.Add(activeSpace);

			FloorSpace left = GetFloorSpace(activeSpace.Column - 1, activeSpace.Row);
			AddIfAvailable(left, region, activeSpace);

			FloorSpace top = GetFloorSpace(activeSpace.Column, activeSpace.Row - 1);
			AddIfAvailable(top, region, activeSpace);

			FloorSpace right = GetFloorSpace(activeSpace.Column + 1, activeSpace.Row);
			AddIfAvailable(right, region, activeSpace);

			FloorSpace bottom = GetFloorSpace(activeSpace.Column, activeSpace.Row + 1);
			AddIfAvailable(bottom, region, activeSpace);
		}

		void AddSpaceToRegion(MapRegion region, FloorSpace activeSpace)
		{
			activeSpace.Parent = region;
			AddAdjacentSpaces(region, activeSpace);
		}

		Room CreateRoomFromSpace(FloorSpace activeSpace)
		{
			Room room = new Room();
			AddSpaceToRegion(room, activeSpace);
			return room;
		}

		Corridor CreateCorridorFromSpace(FloorSpace activeSpace)
		{
			Corridor corridor = new Corridor();
			AddSpaceToRegion(corridor, activeSpace);
			return corridor;
		}

		public void GetAllRoomsAndCorridors()
		{
			Rooms.Clear();
			Corridors.Clear();
			ClearAllSpaces(AllTiles, NumColumns, NumRows);
			DetermineSpaceTypes();


			for (int column = 0; column < NumColumns; column++)
				for (int row = 0; row < NumRows; row++)
				{
					FloorSpace activeSpace = GetFloorSpace(column, row);
					if (activeSpace == null)
						continue;

					if (activeSpace.SpaceType == SpaceType.Room)
					{
						if (activeSpace.Parent == null)
							Rooms.Add(CreateRoomFromSpace(activeSpace));
					}
					else if (activeSpace.SpaceType == SpaceType.Corridor)
					{
						if (activeSpace.Parent == null)
							Corridors.Add(CreateCorridorFromSpace(activeSpace));
					}
				}
		}

		private void DetermineSpaceTypes()
		{
			for (int column = 0; column < NumColumns; column++)
				for (int row = 0; row < NumRows; row++)
				{
					FloorSpace activeSpace = GetFloorSpace(column, row);
					if (activeSpace == null || activeSpace.SpaceType != SpaceType.None)
						continue;

					if (HasThreeAdjacentSpaces(column, row))
						activeSpace.SpaceType = SpaceType.Room;
					else
						activeSpace.SpaceType = SpaceType.Corridor;
				}
		}

		public void PixelsToColumnRow(double x, double y, out int column, out int row)
		{
			column = (int)(x / TileSizePx);
			row = (int)(y / TileSizePx);
		}

		public List<Tile> GetTilesInPixelRect(int left, int top, int width, int height)
		{
			PixelsToColumnRow(left, top, out int leftColumn, out int topRow);
			PixelsToColumnRow(left + width - 1, top + height - 1, out int rightColumn, out int bottomRow);
			List<Tile> results = new List<Tile>();
			foreach (Tile baseSpace in Spaces)
			{
				if (baseSpace.Column >= leftColumn && baseSpace.Column <= rightColumn &&
						baseSpace.Row >= topRow && baseSpace.Row <= bottomRow)
					results.Add(baseSpace);
			}
			return results;
		}

		public List<Tile> GetTilesOutsidePixelRect(int left, int top, int width, int height)
		{
			PixelsToColumnRow(left, top, out int leftColumn, out int topRow);
			PixelsToColumnRow(left + width - 1, top + height - 1, out int rightColumn, out int bottomRow);
			List<Tile> results = new List<Tile>();
			foreach (Tile baseSpace in Spaces)
			{
				if (baseSpace.Column < leftColumn || baseSpace.Column > rightColumn ||
						baseSpace.Row < topRow || baseSpace.Row > bottomRow)
					results.Add(baseSpace);
			}
			return results;
		}

		public void ClearSelection()
		{
			foreach (Tile baseSpace in Spaces)
				baseSpace.Selected = false;
		}

		public double WidthPx
		{
			get
			{
				return (rightmostColumnIndex + 1) * TileSizePx;
			}
		}
		public double HeightPx
		{
			get
			{
				return (lastRowIndex + 1) * TileSizePx;
			}
		}

		public List<Tile> Spaces { get; private set; }
		public List<Room> Rooms { get; private set; } = new List<Room>();
		public List<Corridor> Corridors { get; private set; } = new List<Corridor>();
		public Tile[,] AllTiles { get; private set; }
		public int NumColumns { get; set; }
		public int NumRows { get; set; }
	}
}
