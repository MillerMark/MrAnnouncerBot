using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using MapCore;

namespace DndMapSpike
{
	public class StampsLayer : Layer
	{
		public IStampsManager Map { get; set; }
		public List<IStampProperties> Stamps { get => Map.Stamps; set => Map.Stamps = value; }

		int updateCount;

		public StampsLayer()
		{
		}

		public void SortStampsByZOrder(int zOrderOffset = 0)
		{
			Map.SortStampsByZOrder(zOrderOffset);
		}

		public void AddStamp(IStampProperties stamp)
		{
			Map.AddStamp(stamp);
		}

		public void InsertStamp(int startIndex, IStampProperties stamp)
		{
			Map.InsertStamp(startIndex, stamp);
		}

		public void RemoveStamp(IStampProperties stamp)
		{
			Map.RemoveStamp(stamp);
		}

		public void AddStampNow(IStampProperties stamp)
		{
			BeginUpdate();
			try
			{
				AddStamp(stamp);
			}
			finally
			{
				EndUpdate();
			}
		}

		public void BlendStampImage(Stamp stamp, int xOffset = 0, int yOffset = 0)
		{
			int x = stamp.GetLeft() + xOffset;
			int y = stamp.GetTop() + yOffset;
			BlendImage(stamp.Image, x, y);
		}

		void Refresh()
		{
			ClearAll();
			foreach (IStampProperties stamp in Map.Stamps)
			{
				try
				{
					if (stamp is IStamp wpfStamp)
						wpfStamp.BlendStampImage(this);
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
				SortStampsByZOrder();
				Refresh();
			}
		}

		/// <summary>
		/// Gets the stamp at the specified point if the stamp contains non-transparent 
		/// image data at this point.
		/// </summary>
		/// <returns>Returns the stamp if found, or null.</returns>
		public IStampProperties GetStampAt(double x, double y)
		{
			return Map.GetStampAt(x, y);
		}

		public void RemoveAllStamps(List<IStampProperties> stamps)
		{
			Map.RemoveAllStamps(stamps);
		}

		public void AddStamps(List<IStampProperties> stamps)
		{
			Map.AddStamps(stamps);
		}

		public void InsertStamps(int startIndex, List<IStampProperties> stamps)
		{
			Map.InsertStamps(startIndex, stamps);
		}
	}
}

