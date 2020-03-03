using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using MapCore;

namespace DndMapSpike
{
	public class StampsLayer : Layer
	{
		public IItemsManager Map { get; set; }
		public List<IItemProperties> Stamps { get => Map.Stamps; set => Map.Stamps = value; }

		int updateCount;

		public StampsLayer()
		{
		}

		public void BlendStampImage(IFloatingItem floatingItem, double xOffset = 0, double yOffset = 0)
		{
			double x = floatingItem.GetLeft() + xOffset;
			double y = floatingItem.GetTop() + yOffset;
			BlendImage(floatingItem.Image, x, y);
		}

		void Refresh()
		{
			ClearAll();
			foreach (IItemProperties stamp in Map.Stamps)
			{
				try
				{
					if (stamp is IFloatingItem floatingItem)
						floatingItem.BlendStampImage(this);
				}
				catch
				{
					// TODO: Error placing stamp (likely out of bounds) - should we save it/remove it?
				}
			}
		}

		public void BeginUpdate()
		{
			updateCount++;
		}

		public void EndUpdate()
		{
			updateCount--;
			if (updateCount == 0)
			{
				Map.SortStampsByZOrder();
				Map.NormalizeZOrder();
				Refresh();
			}
		}

		/// <summary>
		/// Gets the item at the specified point if the item contains non-transparent 
		/// image data at this point.
		/// </summary>
		/// <returns>Returns the item if found, or null.</returns>
		public IItemProperties GetItemAt(double x, double y)
		{
			return Map.GetItemAt(x, y);
		}
	}
}

