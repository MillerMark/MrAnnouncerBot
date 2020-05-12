using System;
using System.Linq;

namespace GoogleHelper
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class TabNameAttribute : Attribute
	{
		public TabNameAttribute(string tabName)
		{
			TabName = tabName;
		}

		public string TabName { get; set; }
	}
}
