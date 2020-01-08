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

		void SortStampsByZOrder()
		{
			stamps = stamps.OrderBy(o => o.ZOrder).ToList();
			for(int i = 0; i < stamps.Count; i++)
				stamps[i].ZOrder = i;
		}

		public void AddStamp(Stamp stamp)
		{
			if(stamp.ZOrder == -1)
				stamp.ZOrder = stamps.Count;
			stamps.Add(stamp);
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

		public void BeginUpdate()
		{
			updateCount++;
		}

		void PlaceStamp(Stamp stamp)
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
				PlaceStamp(stamp);
			}
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
	}
}

