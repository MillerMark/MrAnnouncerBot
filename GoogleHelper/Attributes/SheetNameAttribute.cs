using System;
using System.Linq;

namespace GoogleHelper
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class SheetNameAttribute : Attribute
	{
		public SheetNameAttribute(string sheetName)
		{
			SheetName = sheetName;
		}

		public string SheetName { get; set; }
	}
}
