using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using MapCore;

namespace MapCore
{
	public interface IItemsManager
	{
		void AddItem(IItemProperties item);
		void AddStamps(List<IItemProperties> items);

		/// <summary>
		/// Gets the item at the specified point if the item contains non-transparent 
		/// image data at this point.
		/// </summary>
		/// <returns>Returns the stamp if found, or null.</returns>
		IItemProperties GetItemAt(double x, double y);
		void InsertStamp(int startIndex, IItemProperties items);
		void InsertStamps(int startIndex, List<IItemProperties> items);
		void RemoveAllStamps(List<IItemProperties> items);
		void RemoveItem(IItemProperties item);
		void SortStampsByZOrder(int zOrderOffset = 0);
		void NormalizeZOrder(int zOrderOffset = 0);
		List<IItemProperties> Items { get; set; }
	}
}

