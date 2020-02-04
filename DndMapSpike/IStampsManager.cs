using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using MapCore;

namespace DndMapSpike
{
	public interface IStampsManager
	{
		void AddStamp(IStampProperties stamp);
		void AddStamps(List<IStampProperties> stamps);

		/// <summary>
		/// Gets the stamp at the specified point if the stamp contains non-transparent 
		/// image data at this point.
		/// </summary>
		/// <param name="point">The coordinates to check (on the layer).</param>
		/// <returns>Returns the stamp if found, or null.</returns>
		IStampProperties GetStampAt(Point point);
		void InsertStamp(int i, IStampProperties stamp);
		void InsertStamps(int startIndex, List<IStampProperties> stamps);
		void RemoveAllStamps(List<IStampProperties> stamps);
		void RemoveStamp(IStampProperties stamp);
		void SortStampsByZOrder(int zOrderOffset = 0);
	}
}

