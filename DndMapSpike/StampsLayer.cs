using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace DndMapSpike
{
	public class StampsLayer : Layer
	{
		List<Stamp> stamps = new List<Stamp>();
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

		public void AddStamp(Stamp stamp)
		{
			if(stamp.HasNoZOrder())
				stamp.ZOrder = stamps.Count;
			stamps.Add(stamp);
		}

		public void InsertStamp(int i, Stamp stamp)
		{
			if (stamp.HasNoZOrder())
				stamp.ZOrder = i;
			stamps.Insert(i, stamp);
		}

		public void RemoveStamp(Stamp stamp)
		{
			stamps.Remove(stamp);
		}

		public void AddStampNow(Stamp stamp)
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

		void BlendStampImage(Stamp stamp)
		{
			int x = stamp.GetLeft();
			int y = stamp.GetTop();
			BlendImage(stamp.Image, x, y);
		}

		void Refresh()
		{
			ClearAll();
			foreach (Stamp stamp in stamps)
			{
				try
				{
					BlendStampImage(stamp);
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
		public Stamp GetStampAt(Point point)
		{
			for (int i = stamps.Count - 1; i >= 0; i--)
			{
				Stamp stamp = stamps[i];
				if (stamp.ContainsPoint(point))
					return stamp;
			}
			return null;
		}

		public void RemoveAllStamps(List<Stamp> stamps)
		{
			foreach (Stamp stamp in stamps)
				RemoveStamp(stamp);
		}

		public void AddStamps(List<Stamp> stamps)
		{
			foreach (Stamp stamp in stamps)
			{
				stamp.ResetZOrder();
				AddStamp(stamp);
			}
		}

		public void InsertStamps(int startIndex, List<Stamp> stamps)
		{
			for (int i = 0; i < stamps.Count; i++)
			{
				Stamp stamp = stamps[i];
				stamp.ResetZOrder();
				InsertStamp(startIndex + i, stamp);
			}
		}
	}
}

