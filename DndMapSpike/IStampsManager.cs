using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace DndMapSpike
{
	public interface IStampsManager
	{
		void AddStamp(IStamp stamp);
		void AddStamps(List<IStamp> stamps);
		
		/// <summary>
		/// Gets the stamp at the specified point if the stamp contains non-transparent 
		/// image data at this point.
		/// </summary>
		/// <param name="point">The coordinates to check (on the layer).</param>
		/// <returns>Returns the stamp if found, or null.</returns>
		IStamp GetStampAt(Point point);
		void InsertStamp(int i, IStamp stamp);
		void InsertStamps(int startIndex, List<IStamp> stamps);
		void RemoveAllStamps(List<IStamp> stamps);
		void RemoveStamp(IStamp stamp);
		void SortStampsByZOrder(int zOrderOffset = 0);
	}
}

