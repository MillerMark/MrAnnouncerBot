using System;
using System.Linq;

namespace DndCore
{
	public class CarriedItem
	{
		public DndItem Item { get; set; }
		public int Count { get; set; }
		public bool Equipped { get; set; }

		public CarriedItem()
		{

		}
	}
}