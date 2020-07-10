using System;
using System.Linq;

namespace DndCore
{
	public class PropertyMod
	{
		public double Offset { get; set; } = 0;
		public double Multiplier { get; set; } = 1;  //! This initialization to 1 is important!
		
		public PropertyMod(double offset, double multiplier = 1)
		{
			Set(offset, multiplier);
		}

		public void Set(double offset, double multiplier = 1)
		{
			Offset = offset;
			Multiplier = multiplier;
		}
	}
}