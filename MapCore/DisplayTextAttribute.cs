using System;
using System.Linq;

namespace MapCore
{
	public class DisplayTextAttribute : Attribute
	{

		public DisplayTextAttribute(string displayText)
		{
			DisplayText = displayText;
		}

		public string DisplayText { get; set; }
	}
}
