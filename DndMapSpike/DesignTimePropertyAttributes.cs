using System;
using System.Linq;

namespace DndMapSpike
{
	public class DesignTimePropertyAttributes
	{
		public string DisplayText { get; set; }
		public int NumDigits { get; set; }
		public string DependentProperty { get; set; }
		public DesignTimePropertyAttributes()
		{

		}
	}
}

