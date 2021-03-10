using System;
using System.Linq;

namespace DndCore
{
	public class CarriedItemDto
	{
		public string Name { get; set; }
		public double Weight { get; set; }
		public bool Equipped { get; set; }
		public double CostValue { get; set; }
		public CarriedItemDto()
		{

		}
	}
}