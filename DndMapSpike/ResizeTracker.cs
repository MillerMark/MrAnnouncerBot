using MapCore;
using System;
using System.Linq;
using System.Windows.Shapes;

namespace DndMapSpike
{
	public class ResizeTracker
	{
		public const double ResizeHandleDiameter = 10d;
		public IItemProperties Item { get; set; }
		public Ellipse NorthWest { get; set; }
		public Ellipse NorthEast { get; set; }
		public Ellipse SouthWest { get; set; }
		public Ellipse SouthEast { get; set; }
		public Rectangle SelectionRect { get; set; }

		public ResizeTracker(IItemProperties item)
		{
			Item = item;
		}

		public SizeDirection GetDirection(Ellipse ellipse)
		{
			if (ellipse == NorthEast)
				return SizeDirection.NorthEast;
			if (ellipse == SouthWest)
				return SizeDirection.SouthWest;
			if (ellipse == SouthEast)
				return SizeDirection.SouthEast;
			return SizeDirection.NorthWest;
		}

		public void AddCorner(Ellipse ellipse, SizeDirection sizeDirection)
		{
			switch (sizeDirection)
			{
				case SizeDirection.NorthWest:
					NorthWest = ellipse;
					break;
				case SizeDirection.SouthEast:
					SouthEast = ellipse;
					break;
				case SizeDirection.SouthWest:
					SouthWest = ellipse;
					break;
				case SizeDirection.NorthEast:
					NorthEast = ellipse;
					break;
			}
		}
		public void Reposition(double scale, double left, double top, double right, double bottom)
		{
			MoveResizeCornerTo(NorthWest, left, top, scale);
			MoveResizeCornerTo(SouthEast, right, bottom, scale);
			MoveResizeCornerTo(NorthEast, right, top, scale);
			MoveResizeCornerTo(SouthWest, left, bottom, scale);
			System.Windows.Controls.Canvas.SetLeft(SelectionRect, left);
			System.Windows.Controls.Canvas.SetTop(SelectionRect, top);
			SelectionRect.Width = right - left;
			SelectionRect.Height = bottom - top;
		}

		private void MoveResizeCornerTo(System.Windows.FrameworkElement element, double left, double top, double scale)
		{
			System.Windows.Controls.Canvas.SetLeft(element, left - ResizeHandleDiameter / (2 * scale));
			System.Windows.Controls.Canvas.SetTop(element, top - ResizeHandleDiameter / (2 * scale));
		}
	}
}

