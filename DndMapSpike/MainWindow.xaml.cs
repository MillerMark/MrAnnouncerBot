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
using MapCore;

namespace DndMapSpike
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public Map Map { get; set; }
		public MainWindow()
		{
			InitializeComponent();
			//LoadMap("The Delve of Lamprica.txt");
			//LoadMap("The Pit of Alan the Necromancer.txt");
			Map = new Map();
			//LoadMap("The Barrow of Elemental Horror.txt");
			LoadMap("The Hive of the Vampire Princess.txt");
		}

		private Rectangle GetFloor()
		{
			Rectangle floor = new Rectangle();
			floor.Width = Map.pixelsPerFiveFeet + 1;
			floor.Height = Map.pixelsPerFiveFeet + 1;
			floor.Fill = new SolidColorBrush(Color.FromRgb(150, 110, 90));
			return floor;
		}

		void LoadMap(string fileName)
		{
			cvsMap.Children.Clear();
			Map.Load(fileName);
			foreach (Space space in Map.Spaces)
			{
				if (space.Type == MapSpaceType.Floor)
				{
					UIElement element = GetFloor();
					Canvas.SetLeft(element, space.Column * Map.pixelsPerFiveFeet);
					Canvas.SetTop(element, space.Row * Map.pixelsPerFiveFeet);
					cvsMap.Children.Add(element);
				}
			}
			ShowRooms(Map.Rooms);
			cvsMap.Width = Map.Width;
			cvsMap.Height = Map.Height;
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
					Canvas.SetLeft(segmentIndicator, column * Map.pixelsPerFiveFeet + segmentIndicatorOffset);
					Canvas.SetTop(segmentIndicator, roomSegment.StartRow * Map.pixelsPerFiveFeet);
					segmentIndicator.Width = 10;
					segmentIndicator.Height = (roomSegment.EndRow - roomSegment.StartRow) * Map.pixelsPerFiveFeet;
					segmentIndicator.Fill = new SolidColorBrush(Color.FromRgb(215, 204, 255));
					cvsMap.Children.Add(segmentIndicator);
				}
			}
		}

		void ShowRooms(List<Room> foundRooms)
		{
			foreach (Room room in foundRooms)
			{
				const int RoomExpansion = 5;
				Rectangle segmentIndicator = new Rectangle();
				Canvas.SetLeft(segmentIndicator, room.LeftColumn * Map.pixelsPerFiveFeet - RoomExpansion);
				Canvas.SetTop(segmentIndicator, room.StartRow * Map.pixelsPerFiveFeet - RoomExpansion);
				segmentIndicator.Width = room.Width * Map.pixelsPerFiveFeet + RoomExpansion * 2;
				segmentIndicator.Height = room.Height * Map.pixelsPerFiveFeet + RoomExpansion * 2;
				segmentIndicator.Stroke = new SolidColorBrush(Color.FromRgb(10, 120, 100));
				segmentIndicator.StrokeThickness = 5;
				cvsMap.Children.Add(segmentIndicator);
			}
		}
	}
}
