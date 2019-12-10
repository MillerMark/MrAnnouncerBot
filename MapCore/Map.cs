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
		public double pixelsPerFiveFeet { get; set; } = 20;

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
			Spaces = new List<Space>();
			lastRowIndex = -1;
			lastColumnIndex = -1;
			rightmostColumnIndex = -1;
		}
		void OnNewLine()
		{
			lastRowIndex++;
			lastColumnIndex = -1;
		}
		MapSpaceType GetMapSpaceType(string space)
		{
			if (space == "F")
				return MapSpaceType.Floor;
			// TODO: Add more...
			return MapSpaceType.None;
		}
		void LoadNewSpace(string space)
		{
			OnNewSpace();
			MapSpaceType type = GetMapSpaceType(space);
			if (type != MapSpaceType.None)
				Spaces.Add(new Space(lastColumnIndex, lastRowIndex, type));
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

			MapArray = MapEngine.BuildMapArray(Spaces, rightmostColumnIndex, lastRowIndex);
			Rooms = MapEngine.GetAllRooms(MapArray);
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
		public double Width
		{
			get
			{
				//return (rightmostColumnIndex + 1) * feetPerSquare;
				return (rightmostColumnIndex + 1) * pixelsPerFiveFeet;
			}
		}
		public double Height
		{
			get
			{
				//return (lastRowIndex + 1) * feetPerSquare;
				return (lastRowIndex + 1) * pixelsPerFiveFeet;
			}
		}

		public List<Space> Spaces { get; private set; }
		public List<Room> Rooms { get; private set; }
		public Space[,] MapArray { get; private set; }
	}
}
