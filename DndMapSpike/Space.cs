using System;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;

namespace DndMapSpike
{
	public class Space
	{
		public MapSpaceType Type { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }
		public UIElement UiElement { get; set; }
		public Space(int column, int row, MapSpaceType type, UIElement uiElement)
		{
			UiElement = uiElement;
			Type = type;
			Column = column;
			Row = row;
		}
	}
}
