using System;
using System.Linq;

namespace MapCore
{
	public class DependentPropertyAttribute : DesignTimeAttribute
	{
		public DependentPropertyAttribute(string dependentProperty)
		{
			DependentProperty = dependentProperty;
		}

		public string DependentProperty { get; set; }
	}
}
