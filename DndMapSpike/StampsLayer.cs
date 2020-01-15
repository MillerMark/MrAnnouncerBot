using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace DndMapSpike
{
	public class StampsLayer : Layer
	{
		List<IStamp> stamps = new List<IStamp>();
		int updateCount;

		public StampsLayer()
		{
		}

		public void SortStampsByZOrder(int zOrderOffset = 0)
		{
			stamps = stamps.OrderBy(o => o.ZOrder).ToList();
			for(int i = 0; i < stamps.Count; i++)
				stamps[i].ZOrder = i + zOrderOffset;
		}

		public void AddStamp(IStamp stamp)
		{
			if (stamp.HasNoZOrder())
				stamp.ZOrder = stamps.Count;
			stamps.Add(stamp);
		}

		public void InsertStamp(int i, IStamp stamp)
		{
			if (stamp.HasNoZOrder())
				stamp.ZOrder = i;
			stamps.Insert(i, stamp);
		}

		public void RemoveStamp(IStamp stamp)
		{
			stamps.Remove(stamp);
		}

		public void AddStampNow(IStamp stamp)
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
			foreach (IStamp stamp in stamps)
			{
				try
				{
					stamp.BlendStampImage(this);
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
			if(updateCount == 0)
			{
				SortStampsByZOrder();
				Refresh();
			}
		}

		/// <summary>
		/// Gets the stamp at the specified point if the stamp contains non-transparent 
		/// image data at this point.
		/// </summary>
		/// <param name="point">The coordinates to check (on the layer).</param>
		/// <returns>Returns the stamp if found, or null.</returns>
		public IStamp GetStampAt(Point point)
		{
			for (int i = stamps.Count - 1; i >= 0; i--)
			{
				IStamp stamp = stamps[i];
				if (stamp.ContainsPoint(point))
					return stamp;
			}
			return null;
		}

		public void RemoveAllStamps(List<IStamp> stamps)
		{
			foreach (IStamp stamp in stamps)
				RemoveStamp(stamp);
		}

		public void AddStamps(List<IStamp> stamps)
		{
			foreach (IStamp stamp in stamps)
			{
				stamp.ResetZOrder();
				AddStamp(stamp);
			}
		}

		public void InsertStamps(int startIndex, List<IStamp> stamps)
		{
			for (int i = 0; i < stamps.Count; i++)
			{
				IStamp stamp = stamps[i];
				stamp.ResetZOrder();
				InsertStamp(startIndex + i, stamp);
			}
		}
	}
}

