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
			//LoadMap("The Hive of the Vampire Princess.txt");
			LoadMap("The Dark Chambers of Ages.txt");
		}

		private Rectangle GetRoomFloor()
		{
			return GetTileRectangle(Color.FromRgb(255, 161, 161));
		}

		private Rectangle GetCorridorFloor()
		{
			return GetTileRectangle(Color.FromRgb(161, 183, 255));
		}
		private Rectangle GetTileRectangle(Color color)
		{
			Rectangle floor = new Rectangle();
			floor.Width = Map.pixelsPerFiveFeet + 1;
			floor.Height = Map.pixelsPerFiveFeet + 1;
			floor.Fill = new SolidColorBrush(color);
			return floor;
		}

		void LoadMap(string fileName)
		{
			cvsMap.Children.Clear();
			Map.Load(fileName);
			foreach (FloorSpace space in Map.Spaces)
			{
				UIElement element;
				if (space.SpaceType == SpaceType.Room)
					element = GetRoomFloor();
				else if (space.SpaceType == SpaceType.Corridor)
					element = GetCorridorFloor();
				else
					continue;

				Canvas.SetLeft(element, space.Column * Map.pixelsPerFiveFeet);
				Canvas.SetTop(element, space.Row * Map.pixelsPerFiveFeet);
				cvsMap.Children.Add(element);
			}

			cvsMap.Width = Map.Width;
			cvsMap.Height = Map.Height;
		}
	}
}
