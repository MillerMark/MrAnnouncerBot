using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace DndMapSpike
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<Space> spaces;
		int pixelsPerFiveFeet = 20;
		int row;
		int column;
		private const string MapFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Maps";
		public MainWindow()
		{
			InitializeComponent();
			LoadMap("The Delve of Lamprica.txt");
		}
		void ResetMap()
		{
			spaces = new List<Space>();
			row = -1;
			column = -1;
			cvsMap.Children.Clear();
		}
		void OnNewLine()
		{
			row++;
			column = 0;
		}
		void OnNewSpace()
		{
			column++;
		}

		void LoadNewSpace(string space)
		{
			OnNewSpace();
			if (space == "F")
			{
				UIElement element = GetFloor();
				Canvas.SetLeft(element, column * pixelsPerFiveFeet);
				Canvas.SetTop(element, row * pixelsPerFiveFeet);
				cvsMap.Children.Add(element);
			}
		}

		private Rectangle GetFloor()
		{
			Rectangle floor = new Rectangle();
			floor.Width = pixelsPerFiveFeet + 1;
			floor.Height = pixelsPerFiveFeet + 1;
			floor.Fill = new SolidColorBrush(Color.FromRgb(150, 110, 90));
			spaces.Add(new Space(column, row, MapSpaceType.Floor, floor));
			return floor;
		}

		void LoadNewLine(string line)
		{
			OnNewLine();
			var spaces5x5 = line.Split('\t');
			foreach (var space in spaces5x5)
			{
				LoadNewSpace(space);
			}

		}

		Space[,] BuildMapArray(int column, int row)
		{
			Space[,] spaceArray;

			spaceArray = new Space[column + 1, row + 1];
			foreach (var space in spaces)
			{
				spaceArray[space.Column, space.Row] = space;
			}
			return spaceArray;
		}

		void LoadMap(string fileName)
		{
			ResetMap();
			var lines = GetLines(fileName);
			foreach (var line in lines)
			{
				LoadNewLine(line);
			}
			cvsMap.Width = (column + 1) * pixelsPerFiveFeet;
			cvsMap.Height = (row + 1) * pixelsPerFiveFeet;

			Space[,] mapArray = BuildMapArray(column, row);
			FindRooms(mapArray);
		}

		List<RoomSegment> FindRoomSegmentsInColumn(Space[,] mapArray, int column)
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
							roomSegments.Add(new RoomSegment(lastSegmentStart, row, column));
					}
				}
			}
			if (inFloorColumn)
				roomSegments.Add(new RoomSegment(lastSegmentStart, numRows - 1, column));
			return roomSegments;
		}
		void ShowRoomSegments(List<List<RoomSegment>> allRoomSegments)
		{
			for (int column = 0; column < allRoomSegments.Count; column++)
			{
				List<RoomSegment> columnSegments = allRoomSegments[column];
				foreach (RoomSegment roomSegment in columnSegments)
				{
					const int segmentIndicatorOffset = 5;
					Rectangle segmentIndicator = new Rectangle();
					Canvas.SetLeft(segmentIndicator, column * pixelsPerFiveFeet + segmentIndicatorOffset);
					Canvas.SetTop(segmentIndicator, roomSegment.StartRow * pixelsPerFiveFeet);
					segmentIndicator.Width = 10;
					segmentIndicator.Height = (roomSegment.EndRow - roomSegment.StartRow) * pixelsPerFiveFeet;
					segmentIndicator.Fill = new SolidColorBrush(Color.FromRgb(215, 204, 255));
					cvsMap.Children.Add(segmentIndicator);
				}
			}
		}

		Room GetRoom(List<RoomSegment> lastLineSegments, RoomSegment compareRoomSegment)
		{
			foreach (var roomSegment in lastLineSegments)
			{
				if (roomSegment.Matches(compareRoomSegment))
				{
					var room = new Room(roomSegment.Column);
					room.Segments.Add(roomSegment);
					room.Segments.Add(compareRoomSegment);
					return room;
				}
			}
			return null;
		}

		bool SegmentExtendsExistingRoom(List<Room> inProgressRooms, RoomSegment roomSegment)
		{
			foreach (var room in inProgressRooms)
			{
				if (room.SegmentExtends(roomSegment))
					return true;
			}
			return false;
		}

		List<Room> FindRooms(List<List<RoomSegment>> allRoomSegments)
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
					if (SegmentExtendsExistingRoom(inProgressRooms, roomSegment))
						continue;
					Room room = GetRoom(lastLineSegments, roomSegment);
					if (room != null)
						inProgressRooms.Add(room);
					else
						currentLineSegments.Add(roomSegment);
				}

				FinishRooms(inProgressRooms, column, foundRooms);
				lastLineSegments = currentLineSegments;
				currentLineSegments = new List<RoomSegment>();
			}
			return foundRooms;
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

		void ShowRooms(List<Room> foundRooms)
		{
			
		}

		void FindRooms(Space[,] mapArray)
		{
			var allRoomSegments = new List<List<RoomSegment>>();
			var numColumns = mapArray.GetLength(0);
			
			for (int column = 0; column < numColumns; column++)
			{
				List<RoomSegment> roomSegmentsInColumn = FindRoomSegmentsInColumn(mapArray, column);
				allRoomSegments.Add(roomSegmentsInColumn);
			}

			ShowRoomSegments(allRoomSegments);
			var foundRooms = FindRooms(allRoomSegments);
			ShowRooms(foundRooms);
		}

		private static string[] GetLines(string fileName)
		{
			return File.ReadAllText(System.IO.Path.Combine(MapFolder, fileName)).Split('\n');
		}
	}
}
