using System;
using System.Linq;
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
			int x = (int)Math.Round(stamp.X - stamp.Image.Source.Width / 2);
			int y = (int)Math.Round(stamp.Y - stamp.Image.Source.Height / 2);
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
	}
}

